using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Microbe
{
    // Basics for growth equations
    public string microbeName;
    public float population;
    public float growthRate;
    public Dictionary<string, float> competitors;

    // Resources and toxins
    public Dictionary<string, float> requiredResources;
    public Dictionary<string, float> producedResources;
    public Dictionary<string, Toxin> toxins;

    // Carry capacity
    public Dictionary<string, float> kResources = new Dictionary<string, float>();

    // History tracking
    public List<float> popHistory = new List<float>();
    public List<float> kHistory = new List<float>();

    // Assigns initial values to the microbe
    public Microbe(string initName, float initPop, float initGrowthRate, 
                    Dictionary<string, float> initCompetitors, 
                    Dictionary<string, float> initRequiredResources, 
                    Dictionary<string, float> initProducedResources, 
                    Dictionary<string, Toxin> initToxins)
    {
        // Basics for growth equation
        microbeName = initName;
        population = initPop;
        growthRate = initGrowthRate;
        competitors = initCompetitors;

        // Resources and toxins
        requiredResources = initRequiredResources;
        producedResources = initProducedResources;
        toxins = initToxins;
    }

    // Computes population growth using the Lotka-Volterra Model
    public float ComputeGrowth()
    {
        // Calculate carrying capacity
        float minK = float.MaxValue;
        foreach (var res in kResources)
        {
            if (res.Value < minK)
                minK = res.Value;
        }

        // Handle resource collapse
        if (minK <= 0.01f)
        {
            // Complete extinction if small enough
            if (population <= 2)
                return -population;

            // Otherwise, cut population in half
            return -0.5f * population;
        }

        // Calculate competition
        float competitionEffect = 0.0f;
        foreach (var competitor in competitors)
        {
            //Debug.Log($"FOR MICROBE {microbeName}. {competitor.Key}: {competitor.Value}");
            competitionEffect += Mathf.Max(0, competitor.Value);
        }

        // Compute growth
        float effectiveCompetition = Mathf.Max(competitionEffect, 1.0f);
        float growth = growthRate * population * (1 - (population * effectiveCompetition) / minK);

        // Prevent overshooting
        if (growth < -0.9f * population)
        {
            // Extinct population if sufficiently small
            if (population <= 3)
            {
                return -population;
            }

            // Otherwise, cut population down to 1/4
            return -0.75f * population;
        }

        return growth;
    }


    // Calculat the smallest multiplier to the carry capacity given the environment resources
    public float CalculateToxicityMultiplier(Dictionary<string, float> envResources)
    {
        // Get the total amount of resources
        float totalResources = 0.0f;
        foreach(var res in envResources)
        {
            totalResources += res.Value;
        }

        // Early return for no resources
        if(totalResources == 0.0f)
        {
            return 0.0f;
        }

        // Find the minimum toxicity multiplier (most toxic, since multiplying by small fractions makes small numbers)
        float minMultiplier = 1.0f;

        // Iterate through each toxin
        foreach(var res in toxins)
        {
            if(!envResources.TryGetValue(res.Key, out float value))
            {
                continue;
            }

            // Pull values out of class for better readabiility later
            Toxin curToxin = res.Value;
            float maxSafeDensity = curToxin.maxSafeDensity;
            float minSafeDensity = curToxin.minSafeDensity;
            float lethalDensity = curToxin.lethalDensity;
            float toxinWeightedDensity = (curToxin.toxicity * envResources[res.Key]) / totalResources;

            float curMultiplier = 0.0f;

            // If weighted density is in the safe range, continue to next toxin
            if(toxinWeightedDensity <= maxSafeDensity && toxinWeightedDensity >= minSafeDensity)
            {
                continue;
            }

            // If density is lethal, return 0
            if(toxinWeightedDensity >= lethalDensity)
            {
                return curMultiplier;
            }

            // If density is between max safe density and lethal density, normalize multiplier between the two
            else if(toxinWeightedDensity >= maxSafeDensity && toxinWeightedDensity <= lethalDensity)
            {
                curMultiplier = (toxinWeightedDensity - lethalDensity) / (maxSafeDensity - lethalDensity);
            }

            // If density is less than min safe density, normalize
            else if(toxinWeightedDensity <= minSafeDensity)
            {
                curMultiplier = (toxinWeightedDensity / minSafeDensity);
            }

            // Set the new min multiplier if needed
            if(curMultiplier < minMultiplier)
            {
                minMultiplier = curMultiplier;
            }
        }

        return minMultiplier;
    }

    // Update population of the microbe and track the history
    public void UpdatePopulation(float popGrowth)
    {
        // Track history
        popHistory.Add(population);

        // Prevent population from going negative
        if(population + popGrowth > 0)
        {
            population += popGrowth;
            return;
        }

        population = 0;
    }

    // Calculate the competition coefficient for a given microbe
    public void AddCompetitor(Microbe otherMicrobe)
    {
        // Create empty list for competition coefficients
        List<float> competitionCoefficients = new List<float>();

        // Find the intersection of required resources between this microbe and other microbe
        List<string> sharedResources = new List<string>();
        foreach(var res in requiredResources)
        {
            if(otherMicrobe.requiredResources.TryGetValue(res.Key, out float value))
            {
                sharedResources.Add(res.Key);
            }
        }

        // Calculate competition coefficients and add to list
        foreach(var res in sharedResources)
        {
            competitionCoefficients.Add(otherMicrobe.requiredResources[res] / requiredResources[res]);
        }

        // If no competiition coefficients, then the whole thing is 0
        if(competitionCoefficients.Count == 0)
        {
            competitors.Add(otherMicrobe.microbeName, 0);
            return;
        }

        // Find the max of the compeititon coefficients
        float maxCoefficient = 0.0f;
        foreach(var coef in competitionCoefficients)
        {
            if(coef > maxCoefficient)
            {
                maxCoefficient = coef;
            }
        }

        // Add the competition coefficient
        competitors.Add(otherMicrobe.microbeName, otherMicrobe.population * maxCoefficient);
    }

    // Calculate net resource usage at every time step
    public Dictionary<string, float> ProduceConsumeResources()
    {
        // Dictionary to keep track of resource changes
        Dictionary<string, float> resourceChange = new Dictionary<string, float>();

        // Find the limiting factor
        float limitingFactor = population;
        foreach(var res in requiredResources)
        {
            if(kResources.TryGetValue(res.Key, out float value))
            {
                if(value < limitingFactor)
                {
                    limitingFactor = value;
                }
            }
        }

        // Subtract consumed resources
        foreach(var res in requiredResources)
        {
            resourceChange.Add(res.Key, res.Value * limitingFactor * -1);
        }

        // Add produced resource, adding to dict if necessary
        foreach(var res in producedResources)
        {
            // Already in dictionary
            if(resourceChange.TryGetValue(res.Key, out float value))
            {
                resourceChange[res.Key] += (res.Value * limitingFactor);
            }

            // Not in dictionary
            else
            {
                resourceChange.Add(res.Key, res.Value * limitingFactor);
            }
        }

        return resourceChange;
    }

    // Calculate the carrying capacity of each resource for the microbe, multipliied by the toxicityMultiplier
    public void ComputeCarryCapacity(Dictionary<string, float> envResources)
    {
        // Get the toxicity multiplier
        float toxicityMultiplier = CalculateToxicityMultiplier(envResources);

        // Ensure all required resources have keys in kResources
        foreach (var res in requiredResources)
        {
            // If a required resource is not already in kResources, add it with an initial value of 0
            if (!kResources.ContainsKey(res.Key))
            {
                kResources.Add(res.Key, 0.0f);
            }
        }

        // Set the kResources to be 0
        foreach (var key in new List<string>(kResources.Keys))
        {
            kResources[key] = 0;
        }

        // Find the carry capacity
        foreach(var res in envResources)
        {
            // Get the consumption of each resource
            float resourceConsumption = 0.0f;
            requiredResources.TryGetValue(res.Key, out resourceConsumption);

            // Add the key to kResources if it doesnt already exist, init to inf
            if(!kResources.TryGetValue(res.Key, out float value))
            {
                kResources.Add(res.Key, float.MaxValue);
            }

            // If there is no population, k = 0
            if(population == 0)
            {
                kResources[res.Key] = 0.0f;
            }

            // Microbe doesn't use this resource, so no limit
            else if(resourceConsumption == 0.0f)
            {
                kResources[res.Key] = float.MaxValue;
            }

            // Calculate the carry capacity
            else
            {
                kResources[res.Key] = (res.Value / resourceConsumption) * toxicityMultiplier;
            }
        }

        // Find the minimum of the carry capacities and add it to history
        float minK = float.MaxValue;
        foreach(var res in requiredResources)
        {
            if(!kResources.TryGetValue(res.Key, out float value))
            {
                minK = 0;
                break;
            }

            if(kResources[res.Key] < minK)
            {
                minK = kResources[res.Key];
            }
        }

        kHistory.Add(minK);
    }

    public Microbe Clone()
    {
        // Deep copy of dictionaries and lists
        var clonedCompetitors = new Dictionary<string, float>(competitors);

        var clonedRequiredResources = new Dictionary<string, float>(requiredResources);
        var clonedProducedResources = new Dictionary<string, float>(producedResources);

        var clonedToxins = new Dictionary<string, Toxin>();
        foreach (var kvp in toxins)
        {
            clonedToxins.Add(kvp.Key, kvp.Value.Clone()); // Assuming Toxin class has a Clone method
        }

        // Create new Microbe instance
        Microbe newMicrobe = new Microbe(
            initName: microbeName,
            initPop: population,
            initGrowthRate: growthRate,
            initCompetitors: clonedCompetitors,
            initRequiredResources: clonedRequiredResources,
            initProducedResources: clonedProducedResources,
            initToxins: clonedToxins
        );

        // Deep copy kResources, popHistory, and kHistory
        newMicrobe.kResources = new Dictionary<string, float>(kResources);
        newMicrobe.popHistory = new List<float>(popHistory);
        newMicrobe.kHistory = new List<float>(kHistory);

        return newMicrobe;
    }

    public static Microbe CreateMicrobeFromSO(MicrobeSO microbeSO)
    {
        Microbe newMicrobe = new Microbe(
            initName:microbeSO.microbeName,
            initPop:microbeSO.population,
            initGrowthRate:microbeSO.growthRate,
            initCompetitors:new Dictionary<string, float>(),
            initRequiredResources:ResourceConverter.ConvertToDictionary(microbeSO.requiredResources),
            initProducedResources:ResourceConverter.ConvertToDictionary(microbeSO.producedResources),
            initToxins:ToxinConverter.ConvertToDictionary(microbeSO.toxins)
        );
        return newMicrobe;
    }
}