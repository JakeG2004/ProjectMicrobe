using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceAmount
{
    public string resourceName;
    public float amount;   
}

public class ResourceConverter
{
    public static Dictionary<string, float> ConvertToDictionary(List<ResourceAmount> resourceList)
    {
        Dictionary<string, float> resourceDictionary = new Dictionary<string, float>();

        foreach (var resource in resourceList)
        {
            // Add the resource to the dictionary
            if (!resourceDictionary.ContainsKey(resource.resourceName))
            {
                resourceDictionary.Add(resource.resourceName, resource.amount);
            }
            else
            {
                // Optionally, handle duplicates by adding the amounts
                resourceDictionary[resource.resourceName] += resource.amount;
            }
        }

        return resourceDictionary;
    }
}