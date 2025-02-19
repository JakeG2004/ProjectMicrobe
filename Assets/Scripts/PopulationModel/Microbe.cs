using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Microbe
{
    // Basics for growth equation
    public string microbeName = "Microbe";
    public float population = 1.0f;
    public float growthRate = 1.01f;
    public List<Competitor> competitors = new List<Competitor>();

    // Resources and toxins
    public List<ResourceAmount> requiredResources = new List<ResourceAmount>();
    public List<ResourceAmount> producedResources = new List<ResourceAmount>();
    public List<Toxin> toxins = new List<Toxin>();

    // Carry capacity
    public List<ResourceAmount> kResources = new List<ResourceAmount>();
}
