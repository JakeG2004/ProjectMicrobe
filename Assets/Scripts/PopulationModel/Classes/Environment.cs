using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment
{
    public Dictionary<string, float> resources;
    public Dictionary<string, float> resourceRefreshRate;
    public Dictionary<string, List<float>> resourceHistory = new Dictionary<string, List<float>>();
    public int histLen = 0;

    public Environment(Dictionary<string, float> initResources, Dictionary<string, float> initResourceRefreshRate)
    {
        resources = initResources;
        resourceRefreshRate = initResourceRefreshRate;
    }

    // Log and refresh resources
    public void UpdateResourceHistory()
    {
        Dictionary<string, float> newResourceAmounts = new Dictionary<string, float>();

        foreach(var res in resources)
        {
            // If no entry found, create one
            if(!resourceHistory.TryGetValue(res.Key, out List<float> value))
            {
                resourceHistory.Add(res.Key, new List<float>());

                // Backlog history as 0
                for(int i = 0; i < histLen; i++)
                {
                    resourceHistory[res.Key].Add(0);
                }
            }

            // Add current amount to history
            resourceHistory[res.Key].Add(res.Value);

            // Add resources from refresh rate
            newResourceAmounts.Add(res.Key, res.Value + resourceRefreshRate[res.Key]);

            // Prevent negative
            if(res.Value < 0)
            {
                newResourceAmounts[res.Key] = 0;
            }
        }

        foreach(var res in newResourceAmounts)
        {
            resources[res.Key] = res.Value;
        }

        // Update history lenth
        histLen++;
    }

    // Adds resources from external source
    public void AddResources(Dictionary<string, float> addedResources)
    {
        foreach(var res in addedResources)
        {
            resources[res.Key] += res.Value;
        }
    }
}
