using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicrobePopSim : MonoBehaviour
{
    public EnvironmentSO envSO;
    public Environment env;

    public List<MicrobeSO> microbeSOs = new List<MicrobeSO>();
    public List<Microbe> microbes = new List<Microbe>();
    public int currentStep = 0;

    [SerializeField] private float _updatePeriod = 15.0f;
    private float _elapsedTime = 0.0f;

    [SerializeField] private UnityEvent _onSimAdvance;
    [SerializeField] private bool _advanceOnStart = true;

    [Space(10)]
    [SerializeField] private float _stableActivityMean = 1.0f;
    [SerializeField] private float _stableActivityVariance = 0.5f;

    private float[] _consumptionArr = new float[10];
    private float _bioActivityVariance = 0.0f;
    private float _bioActivityMean = 0.0f;
    private Vector2 _bioActivity;
    private PylonRegion _region;

    // Start is called before the first frame update
    void Start()
    {
        // Get environment from the region
        _region = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedPylon>().GetCurrentRegion();
        _region.SetRegionPylon(this.gameObject);
        envSO = _region.GetEnvSO();

        // Set up envirocnment
        if (!envSO)
        {
            Debug.LogWarning("No environment SO!");
            env = new Environment(new Dictionary<string, float>(), new Dictionary<string, float>());
        }

        else
        {
            // We do this because Unity has no native serialization for dictionaries??? bizarre
            Dictionary<string, float> initialResources = ResourceConverter.ConvertToDictionary(envSO.initialResources);

            Dictionary<string, float> resourceRefresh = ResourceConverter.ConvertToDictionary(envSO.resourceRefresh);

            env = new Environment(initialResources, resourceRefresh);
        }

        // Check for microbeSOs
        if (microbeSOs.Count == 0)
        {
            Debug.LogWarning("No microbe SOs!");
        }

        // Convert each microbeSO into a new microbe in the simulation
        foreach (var microbeSO in microbeSOs)
        {
            microbes.Add(new Microbe(
                initName: microbeSO.microbeName,
                initPop: microbeSO.population,
                initGrowthRate: microbeSO.growthRate,
                initCompetitors: new Dictionary<string, float>(),
                initRequiredResources: ResourceConverter.ConvertToDictionary(microbeSO.requiredResources),
                initProducedResources: ResourceConverter.ConvertToDictionary(microbeSO.producedResources),
                initToxins: ToxinConverter.ConvertToDictionary(microbeSO.toxins)
            ));
        }

        // Initialize array to -1s to indicate no consumption
        for (int i = 0; i < _consumptionArr.Length; i++)
        {
            _consumptionArr[i] = -1;
        }

        if (_advanceOnStart)
        {
            AdvanceSimulation();
        }
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        // Only do time step when count > 0
        if (_elapsedTime >= _updatePeriod)// && microbes.Count > 0)
        {
            AdvanceSimulation();
            _elapsedTime = 0.0f;
        }
    }

    public void AdvanceSimulation()
    {
        // Early return when no resources or microbes
        if (env.resources.Count == 0)// || microbes.Count == 0)
        {
            return;
        }

        // Count the number of resources that we have, also preventing running with no resources
        int resCounter = 0;
        foreach (var res in env.resources)
        {
            if (res.Value > 0)
            {
                resCounter++;
            }
        }

        if (resCounter == 0)
        {
            return;
        }

        // Set up dict to track resource usage
        Dictionary<string, float> totalResourceUsage = new Dictionary<string, float>();

        // Simulation
        if (microbes.Count != 0)
        {
            // Calculate competition coefficients at every time step
            foreach (var m1 in microbes)
            {
                // Reset competitors dict
                m1.competitors = new Dictionary<string, float>();

                foreach (var m2 in microbes)
                {
                    // Prevent microbes from competing with themselves
                    if (m1 == m2)
                    {
                        continue;
                    }

                    m1.AddCompetitor(m2);
                }
            }

            // Process each microbe
            foreach (var microbe in microbes)
            {
                // Get carry capacity of microbe
                microbe.ComputeCarryCapacity(env.resources);

                // Get resource changes due to microbe
                Dictionary<string, float> netResourceUsage = microbe.ProduceConsumeResources();

                // Append changes to total resource usage
                foreach (var resource in netResourceUsage)
                {
                    // If the resource already exists
                    if (totalResourceUsage.TryGetValue(resource.Key, out float value))
                    {
                        totalResourceUsage[resource.Key] += resource.Value;
                        continue;
                    }

                    // If resource doesnt already exist
                    totalResourceUsage.Add(resource.Key, resource.Value);
                }

                // Calculate new microbe pop
                float popChange = microbe.ComputeGrowth();
                microbe.UpdatePopulation(popChange);
            }
        }

        float curConsumption = 0.0f;

        // Add to the bioActivity list
        foreach (var resource in totalResourceUsage)
        {
            curConsumption += Mathf.Abs(resource.Value);
        }
        _consumptionArr[currentStep % 10] = curConsumption;

        CalculateBioActivity();

        // Log resource history
        env.AddResources(totalResourceUsage);
        env.UpdateResourceHistory();

        currentStep++;

        _onSimAdvance.Invoke();

        // Update the graphs
        GetComponent<GraphUpdater>().UpdateGraphs();

        // Broadcast if stable
        if(IsStable())
        {
            //Debug.Log($"`{envSO.envName}` is stable!");
            GetComponent<StringGameEventTrigger>().TriggerEvent(envSO.envName);
        }
    }

    public void FastForward(int n)
    {
        for (int i = 0; i < n; i++)
        {
            AdvanceSimulation();
        }
    }

    public void AddMicrobe(Microbe newMicrobe)
    {
        foreach (var microbe in microbes)
        {
            if (microbe.microbeName == newMicrobe.microbeName)
            {
                return;
            }
        }

        microbes.Add(newMicrobe);

        // Backfill population
        for(int i = 0; i < currentStep - 1; i++)
        {
            newMicrobe.popHistory.Add(0.0f);
        }
    }

    public void RemoveMicrobe(string name)
    {
        foreach (var microbe in microbes)
        {
            if (microbe.microbeName == name)
            {
                microbes.Remove(microbe);
                return;
            }
        }
    }

    public float GetMicrobePopulation(string microbeNameQuery)
    {
        // Go through each microbe
        foreach (var microbe in microbes)
        {
            // If its found, return its population
            if (microbe.microbeName == microbeNameQuery)
            {
                return microbe.population;
            }
        }

        // Otherwise, return -1
        return -1.0f;
    }

    public void IncreaseMicrobePopulation(string microbeName, float amount)
    {
        foreach (var microbe in microbes)
        {
            if (microbe.microbeName == microbeName)
            {
                microbe.population += amount;
            }
        }
    }

    public List<Microbe> GetMicrobes()
    {
        return microbes;
    }

    public Environment GetEnv()
    {
        return env;
    }

    public void SetEnv(EnvironmentSO newEnv)
    {
        envSO = newEnv;
    }

    // Calculate the biological activity
    // This will be expressed as a Vector2
    // Mean of consumption and variance of consumption
    // This can ensure certain level of activity is identifiable
    // As well as a consistent level of activity
    public void CalculateBioActivity()
    {
        // Ensure that the array is full
        for (int i = 0; i < _consumptionArr.Length; i++)
        {
            if (_consumptionArr[i] == -1)
            {
                _bioActivity = new Vector2(0, 0);
            }
        }

        // Calculate the mean
        _bioActivityMean = 0.0f;
        for (int i = 0; i < _consumptionArr.Length; i++)
        {
            _bioActivityMean += _consumptionArr[i];
        }

        _bioActivityMean /= _consumptionArr.Length;

        // Calculate variance
        _bioActivityVariance = 0;
        for (int i = 0; i < _consumptionArr.Length; i++)
        {
            _bioActivityVariance += ((_consumptionArr[i] - _bioActivityMean) * (_consumptionArr[i] - _bioActivityMean));
        }

        _bioActivityVariance /= (_consumptionArr.Length - 1);

        // Assign the new bioactivity
        _bioActivity = new Vector2(_bioActivityMean, _bioActivityVariance);
    }

    public Vector2 GetBioActivity()
    {
        return _bioActivity;
    }

    public bool IsStable()
    {
        return (_bioActivityVariance < _stableActivityVariance && _bioActivityMean > _stableActivityMean);
    }
}
