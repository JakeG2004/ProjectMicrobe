using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassTest : MonoBehaviour
{
    public Environment env;
    public List<Microbe> microbes = new List<Microbe>();

    public int currentStep = 0;

    public KeyCode advanceTime = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, float> initialResources = new Dictionary<string, float>
        {
            {"Oxygen", 10},
            {"Glucose", 10},
            {"Lead", 1}
        };

        Dictionary<string, float> resourceRefresh = new Dictionary<string, float>
        {
            {"Oxygen", 0},
            {"Glucose", 0},
            {"Lead", 1}
        };

        env = new Environment(initialResources, resourceRefresh);

        microbes.Add(new Microbe(
            initName:"OxygenEater",
            initPop:2.0f,
            initGrowthRate:1.2f,
            initCompetitors:new Dictionary<string, float>(),
            initRequiredResources:new Dictionary<string, float>
            {
                {"Oxygen", 1}
            },
            initProducedResources:new Dictionary<string, float>
            {
                {"Glucose", 1}
            },
            initToxins:new Dictionary<string, Toxin>
            {
                {"Lead", new Toxin(1.0f, 0.0f, 0.4f, 0.6f)}
            }
        ));

        microbes.Add(new Microbe(
            initName:"GlucoseEater",
            initPop:2.0f,
            initGrowthRate:1.2f,
            initCompetitors:new Dictionary<string, float>(),
            initRequiredResources:new Dictionary<string, float>
            {
                {"Glucose", 1}
            },
            initProducedResources:new Dictionary<string, float>
            {
                {"Oxygen", 1}
            },
            initToxins:new Dictionary<string, Toxin>()
        ));

        microbes.Add(new Microbe(
            initName:"LeadEater",
            initPop:1.0f,
            initGrowthRate:1.2f,
            initCompetitors:new Dictionary<string, float>(),
            initRequiredResources:new Dictionary<string, float>
            {
                {"Lead", 1}
            },
            initProducedResources:new Dictionary<string, float>(),
            initToxins:new Dictionary<string, Toxin>()
        ));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(advanceTime))
        {
            AdvanceSimulation();
        }
    }

    void AdvanceSimulation()
    {
        // Early return when no resources
        if(env.resources.Count == 0)
        {
            return;
        }

        // Count the number of resources that we have, also preventing running with no resources
        int resCounter = 0;
        foreach(var res in env.resources)
        {
            if(res.Value > 0)
            {
                resCounter++;
            }
        }

        if(resCounter == 0)
        {
            return;
        }

        // Set up dict to track resource usage
        Dictionary<string, float> totalResourceUsage = new Dictionary<string, float>();

        // Simulation
        if(microbes.Count != 0)
        {
            // Calculate competition coefficients at every time step
            foreach(var m1 in microbes)
            {
                // Reset competitors dict
                m1.competitors = new Dictionary<string, float>();

                foreach(var m2 in microbes)
                {
                    m1.AddCompetitor(m2);
                }
            }

            // Process each microbe
            foreach(var microbe in microbes)
            {
                // Get carry capacity of microbe
                microbe.ComputeCarryCapacity(env.resources);

                // Get resource changes due to microbe
                Dictionary<string, float> netResourceUsage = microbe.ProduceConsumeResources();

                // Append changes to total resource usage
                foreach(var resource in netResourceUsage)
                {
                    // If the resource already exists
                    if(totalResourceUsage.TryGetValue(resource.Key, out float value))
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

        // Log resource history
        env.AddResources(totalResourceUsage);
        env.UpdateResourceHistory();

        currentStep++;
    }
}
