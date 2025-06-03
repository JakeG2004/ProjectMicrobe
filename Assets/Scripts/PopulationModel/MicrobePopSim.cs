using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicrobePopSim : MonoBehaviour
{
    /*
    // Variables regarding the environment
    public EnvironmentSO envSO;
    public Environment env;

    // Variables regarding the microbes
    public List<MicrobeSO> microbeSOs = new List<MicrobeSO>();
    public List<Microbe> microbes = new List<Microbe>();

    // Variables regarding the simulation
    [SerializeField] private float _updatePeriod = 15.0f;
    public int currentStep = 0;

    // Helper variables
    [SerializeField] private bool _advanceOnStart = true;
    private float _elapsedTime = 0.0f;
    private float[] _consumptionArr = new float[6];
    private float[] _ammoniumArr = new float[6];
    private PylonRegion _region;
    private bool _isStable = false;

    // Environment health variables
    [SerializeField] private float _stableActivityMean = 1.0f;
    [SerializeField] private float _stableActivityVariance = 0.5f;
    private float _bioActivityVariance = 0.0f;
    private float _bioActivityMean = 0.0f;
    private Vector2 _bioActivity;
    private float _ammoniumProduced = 0.0f;
    private float _prevAmmon = 0.0f;
    private float _curAmmon = 0.0f;

    // Unity Events
    [SerializeField] private UnityEvent _onSimAdvance;
    [SerializeField] private UnityEvent _OnStableStateReached;

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

        // Initialize array to -1s to indicate no consumption
        for (int i = 0; i < _ammoniumArr.Length; i++)
        {
            _ammoniumArr[i] = -1;
        }

        // Update the stability light
        GetComponent<StabilityLightController>().UpdateStability(IsStable());

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
        _consumptionArr[currentStep % _consumptionArr.Length] = curConsumption;

        CalculateBioActivity();

        // Log resource history
        env.AddResources(totalResourceUsage);
        env.UpdateResourceHistory();

        // Update the graphs
        GetComponent<GraphUpdater>().UpdateGraphs();

        // Update the stability light
        GetComponent<StabilityLightController>().UpdateStability(IsStable());

        // Get whether ammonium produced
        if (env.resources.TryGetValue("Ammonium", out float _curAmmon))
        {
            _ammoniumArr[currentStep % _ammoniumArr.Length] = _curAmmon;

            // Check that the array is full
            bool fullAmmonium = true;
            for (int i = 0; i < _ammoniumArr.Length; i++)
            {
                if (_ammoniumArr[i] == -1)
                {
                    fullAmmonium = false;
                    continue;
                }
            }

            if (fullAmmonium)
            {
                _ammoniumProduced = _curAmmon - _prevAmmon;
            }

            else
            {
                _ammoniumProduced = 0;
            }

            // Reset ammonium
            _prevAmmon = _curAmmon;
        }
        
        // Broadcast if stable
        if (IsStable() && _ammoniumProduced > 0)
        {
            //Debug.Log($"`{envSO.envName}` is stable!");
            GetComponent<StringGameEventTrigger>().TriggerEvent(envSO.envName);

            if (!_isStable)
            {
                _OnStableStateReached?.Invoke();
            }

            _isStable = true;
        }

        else
        {
            _isStable = false;
        }

        currentStep++;
        _onSimAdvance.Invoke();
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
    }*/

    // ===== ENVIRONMENT VARIABLES =====
    [SerializeField] private EnvironmentSO _envSO;
    [SerializeField] private Environment _env;


    // ===== MICROBE VARIABLES =====
    [SerializeField] private List<MicrobeSO> _microbeSOs = new List<MicrobeSO>();
    [SerializeField] private List<Microbe> _microbes = new List<Microbe>();


    // ===== SIMULATION VARIABLES =====
    [SerializeField] private float _updatePeriod = 15.0f;
    [SerializeField] private bool _advanceOnStart = true;
    private float _elapsedTime = 0.0f;
    private int _curStep = 0;


    // ===== STABILITY VARIABLES =====
    const int STABILITY_ARR_SIZE = 6;
    private float[] _consumptionArr = new float[STABILITY_ARR_SIZE];
    private float[] _ammoniumArray = new float[STABILITY_ARR_SIZE];
    private Vector2 _bioActivity;


    // ===== SCRIPT REFERENCES =====
    private GraphUpdater _gu;


    // ===== UNITY EVENTS =====
    [SerializeField] private UnityEvent _onSimAdvance;

    void Start()
    {
        InitEnv();
        InitMicrobes();
        InitStabilityArrays();
        InitScriptReferences();

        // Advance on start if set
        if (_advanceOnStart)
        {
            AdvanceSimulation();
        }
    }

    void Update()
    {
        IncrementTimer();
    }


    // ================================
    // ===== SIMULATION FUNCTIONS =====
    // ================================


    // Advance the simulation by a single step
    public void AdvanceSimulation()
    {
        // Early return if sufficient conditions not met
        if (CheckEarlySimReturn())
        {
            return;
        }

        // Set up a dictionary to track resource usage
        Dictionary<string, float> totalResourceUsage = new Dictionary<string, float>();

        // Perform the microbe portion of the simulation
        SimulateMicrobes(totalResourceUsage);

        // Set the current consumption
        CalculateCurrentConsumption(totalResourceUsage);

        // Calculate the bioactivity
        CalculateBioActivity();

        // Log the resource history
        _env.AddResources(totalResourceUsage);
        _env.UpdateResourceHistory();

        // Update the graphs
        _gu.UpdateGraphs();

        CheckAmmoniumProduced();

        _curStep++;
        _onSimAdvance.Invoke();
    }

    // Advance the simulation by n steps
    public void FastForward(int n)
    {
        for (int i = 0; i < n; i++)
        {
            AdvanceSimulation();
        }
    }


    // =============================
    // ===== MICROBE FUNCTIONS =====
    // =============================


    // Add a microbe to the simulation
    public void AddMicrobe(Microbe newMicrobe)
    {
        // Handle duplicate entries
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == newMicrobe.microbeName)
            {
                return;
            }
        }

        _microbes.Add(newMicrobe);

        // Backfill population
        for (int i = 0; i < _curStep - 1; i++)
        {
            newMicrobe.popHistory.Add(0.0f);
        }
    }

    // Removes a microbe fromt the simulation
    public void RemoveMicrobe(string name)
    {
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == name)
            {
                _microbes.Remove(microbe);
                return;
            }
        }
    }

    // Returns the population of a given microbe
    public float GetMicrobePopulation(string microbeNameQuery)
    {
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == microbeNameQuery)
            {
                return microbe.population;
            }
        }

        return -1.0f;
    }

    // Increase a microbe's population by a given amount
    public void IncreaseMicrobePopulation(string microbeName, float amount)
    {
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == microbeName)
            {
                microbe.population += amount;
            }
        }
    }

    // Get all the microbes in the simulation
    public List<Microbe> GetMicrobes()
    {
        return _microbes;
    }


    // =================================
    // ===== ENVIRONMENT FUNCTIONS =====
    // =================================


    // Get the environment
    public Environment GetEnv()
    {
        return _env;
    }

    // Get the environment so
    public EnvironmentSO GetEnvSO()
    {
        return _envSO;
    }

    // Set the environment
    public void SetEnv(EnvironmentSO newEnv)
    {
        _envSO = newEnv;
    }

    // Set up an environment from the environmentSO
    private void InitEnv()
    {
        // Get the environment SO from the region
        PylonRegion region = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedPylon>().GetCurrentRegion();
        region.SetRegionPylon(this.gameObject);
        _envSO = region.GetEnvSO();

        // Initialize the environment from the SO

        // Create a new environment if one doesnt already exist
        if (!_envSO)
        {
            Debug.LogWarning("No environmentSO!");
            _env = new Environment(new Dictionary<string, float>(), new Dictionary<string, float>());
        }

        // Create new environment from the given envSO
        else
        {
            // Create the initial resources dictionary
            Dictionary<string, float> initialResources = ResourceConverter.ConvertToDictionary(_envSO.initialResources);

            // Create the resource refresh dictionary
            Dictionary<string, float> resourceRefresh = ResourceConverter.ConvertToDictionary(_envSO.resourceRefresh);

            // Set the environment
            _env = new Environment(initialResources, resourceRefresh);
        }
    }


    // =============================
    // ===== MICROBE FUNCTIONS =====
    // =============================


    // Set up the microbes from MicrobeSO list
    private void InitMicrobes()
    {
        // Give warning if no microbe SOs
        if (_microbeSOs.Count == 0)
        {
            Debug.LogWarning("No Microbe SOs!");
        }

        // Convert the microbeSOs into a new Microbe and add it to the list
        foreach (MicrobeSO mso in _microbeSOs)
        {
            Microbe newMicrobe = Microbe.CreateMicrobeFromSO(mso);
            _microbes.Add(newMicrobe);
        }
    }

    // Simulates a step of the microbe simulation
    private void SimulateMicrobes(Dictionary<string, float> totalResourceUsage)
    {
        // Early return if no microbes
        if (_microbes.Count == 0)
        {
            return;
        }

        SetMicrobeCompetitors();
        SimulateMicrobeConsumption(totalResourceUsage);
    }

    // Sets the competition between every pair of microbes
    private void SetMicrobeCompetitors()
    {
        // Calculate the competition coefficients for each microbe
        foreach (Microbe m1 in _microbes)
        {
            // Reset the competitors dict for this microbe
            m1.competitors = new Dictionary<string, float>();

            foreach (Microbe m2 in _microbes)
            {
                // Stop competition with oneself
                if (m1 == m2)
                {
                    continue;
                }

                m1.AddCompetitor(m2);
            }
        }
    }

    // Calculates the consumption of the microbes
    private void SimulateMicrobeConsumption(Dictionary<string, float> totalResourceUsage)
    {
        // ProcessMicrobeConsumption
        foreach (Microbe microbe in _microbes)
        {
            // Get the carry capacity of each microbe
            microbe.ComputeCarryCapacity(_env.resources);

            // Get the resource changes due to the microbe
            Dictionary<string, float> netResourceUsage;
            netResourceUsage = microbe.ProduceConsumeResources();

            // Append changes to the total resource usage
            foreach (var resource in netResourceUsage)
            {
                // If the resource already exists
                if (totalResourceUsage.TryGetValue(resource.Key, out float value))
                {
                    totalResourceUsage[resource.Key] += resource.Value;
                    continue;
                }

                // If the resource doesn't already exist
                totalResourceUsage.Add(resource.Key, resource.Value);
            }

            // Calculate the new microbe population
            float popChange = microbe.ComputeGrowth();
            microbe.UpdatePopulation(popChange);
        }
    }


    // =================================
    // ===== BIOACTIVITY FUNCTIONS =====
    // =================================


    // Calculate mean and variance of bioactivity
    public void CalculateBioActivity()
    {
        // Ensure that the array is full
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            if (_consumptionArr[i] == -1)
            {
                _bioActivity = new Vector2(0, 0);
                return;
            }
        }

        // Calculate the mean
        float bioActivityMean = 0.0f;
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            bioActivityMean += _consumptionArr[i];
        }

        bioActivityMean /= _consumptionArr.Length;

        // Calculate the variance
        float bioActivityVariance = 0.0f;
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            bioActivityVariance += ((_consumptionArr[i] - bioActivityMean) * (_consumptionArr[i] - bioActivityMean));
        }

        bioActivityVariance /= (STABILITY_ARR_SIZE - 1);

        // Assign the new bioactivity
        _bioActivity = new Vector2(bioActivityMean, bioActivityVariance);
    }

    // Get the vector2 of <mean, variance>
    public Vector2 GetBioActivity()
    {
        return _bioActivity;
    }


    // ===========================
    // ===== MISC. FUNCTIONS =====
    // ===========================


    // Initializes the arrays used in stability calculation
    private void InitStabilityArrays()
    {
        // Initialize the consumption array to -1s
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            _consumptionArr[i] = -1;
        }

        // Initialize the ammonium array to -1s
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            _ammoniumArray[i] = -1;
        }
    }

    // Bind script reference
    private void InitScriptReferences()
    {
        _gu = GetComponent<GraphUpdater>();
    }

    // Increment the timer
    private void IncrementTimer()
    {
        // Add to the time
        _elapsedTime += Time.deltaTime;

        // Perform the update if time is passed
        if (_elapsedTime >= _updatePeriod)
        {
            AdvanceSimulation();
            _elapsedTime = 0.0f;
        }
    }

    // Calculate the current consumtion of everything
    private void CalculateCurrentConsumption(Dictionary<string, float> totalResourceUsage)
    {
        float curConsumption = 0.0f;

        // Iterate through every resource and add it to current consumption
        foreach (var resource in totalResourceUsage)
        {
            curConsumption += Mathf.Abs(resource.Value);
        }

        _consumptionArr[_curStep % STABILITY_ARR_SIZE] = curConsumption;
    }

    // Checks for early simulation return
    private bool CheckEarlySimReturn()
    {
        // Early return when no resources
        if (_env.resources.Count == 0)
        {
            return true;
        }

        // Count the number of resources
        int resCounter = 0;
        foreach (var res in _env.resources)
        {
            if (res.Value > 0)
            {
                resCounter++;
            }
        }

        // Early return when the number of resources is 0
        if (resCounter == 0)
        {
            return true;
        }

        return false;
    }

    // Checks that for the last STABILITY_ARR_SIZE time steps, ammonium has been produced
    public bool CheckAmmoniumProduced()
    {
        // Add the ammonium to the current step of the array
        if (_env.resources.TryGetValue("Ammonium", out float curAmmon))
        {
            _ammoniumArray[_curStep % STABILITY_ARR_SIZE] = curAmmon;
        }

        // Check that the array is full
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            if (_ammoniumArray[i] <= 0)
            {
                return false;
            }
        }

        return true;
    }
}
