using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicrobePopSim : MonoBehaviour
{
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
    const int STABILITY_ARR_SIZE = 5;
    private float[] _consumptionArr = new float[STABILITY_ARR_SIZE];
    private float[] _mycorrhisArray = new float[STABILITY_ARR_SIZE];
    private Vector2 _bioActivity;


    // ===== SCRIPT REFERENCES =====
    private GraphUpdater _gu;
    private PylonStatusEventsChecker _psec;


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
        //IncrementTimer();
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

    // Checks that for the last STABILITY_ARR_SIZE time steps, mycorrhis exists
    public Vector2 GetMycorrhisStats()
    {
        // Add the nitrate to the current step of the array
        _mycorrhisArray[_curStep % STABILITY_ARR_SIZE] = GetMicrobePopulation("F. Mycorrhis");

        // Check that the array is full
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            if (_mycorrhisArray[i] <= 0)
            {
                return new Vector2(0, 0);
            }
        }

        float mycorrhisMean = 0.0f;
        // Get the mean
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            mycorrhisMean += _mycorrhisArray[i];
        }

        mycorrhisMean /= _mycorrhisArray.Length;

        // Get the variance
        float mycorrhisVar = 0.0f;
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            mycorrhisVar += ((_mycorrhisArray[i] - mycorrhisMean) * (_mycorrhisArray[i] - mycorrhisMean));
        }

        mycorrhisVar /= (STABILITY_ARR_SIZE - 1);

        return new Vector2(mycorrhisMean, mycorrhisVar);
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
        if (!_envSO)
        {
            // Get the environment SO from the region
            PylonRegion region = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedPylon>().GetCurrentRegion();
            region.SetRegionPylon(this.gameObject);
            _envSO = region.GetEnvSO();
        }

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

        // Add the microbes in the SO to the microbe list
        foreach (MicrobeSOPopPair microbeSOPopPair in _envSO.initialMicrobes)
        {
            Microbe newMicrobe = Microbe.CreateMicrobeFromSO(microbeSOPopPair.microbe);
            newMicrobe.population = microbeSOPopPair.population;
            _microbes.Add(newMicrobe);
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

    public float GetToxinDensity()
    {
        float toxinAmt = 0.0f;

        toxinAmt += GetResourceAmt("Lead");
        toxinAmt += GetResourceAmt("Sulfur Dioxide");

        float totalRes = 0.0f;

        // Get total resources
        foreach (var res in _env.resources)
        {
            totalRes += res.Value;
        }

        return toxinAmt / totalRes;
    }

    public float GetResourceAmt(string resName)
    {
        if (_env.resources.TryGetValue(resName, out float amt))
        {
            return (amt);
        }

        return -1;
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

        // Initialize the mycorrhis array to -1s
        for (int i = 0; i < STABILITY_ARR_SIZE; i++)
        {
            _mycorrhisArray[i] = -1;
        }
    }

    // Bind script reference
    private void InitScriptReferences()
    {
        _gu = GetComponent<GraphUpdater>();
        _psec = GetComponent<PylonStatusEventsChecker>();
        _psec.SetStableState(new Vector2(_envSO.stableMycorrhisAmt, _envSO.stableMycorrhisVar));
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
}