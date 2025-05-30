using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MicrobeSO", menuName = "ScriptableObjects/PopEngine/MicrobeSO")]
public class MicrobeSO : ScriptableObject
{
    // Basics for growth equations
    public string microbeName;
    public float population;
    public float growthRate;

    // Resources and toxins
    public List<ResourceAmount> requiredResources = new List<ResourceAmount>();
    public List<ResourceAmount> producedResources = new List<ResourceAmount>();
    public List<ToxinAmount> toxins = new List<ToxinAmount>();

    [TextArea(5,10)]
    public string description;
}

[Serializable]
public class ToxinAmount
{
    public string toxinName;
    public Toxin toxin;
}

public class ToxinConverter
{
    public static Dictionary<string, Toxin> ConvertToDictionary(List<ToxinAmount> toxinList)
    {
        Dictionary<string, Toxin> toxinDictionary = new Dictionary<string, Toxin>();

        foreach (var toxin in toxinList)
        {
            // Add the resource to the dictionary
            if (!toxinDictionary.ContainsKey(toxin.toxinName))
            {
                toxinDictionary.Add(toxin.toxinName, toxin.toxin);
            }
        }

        return toxinDictionary;
    }
}
