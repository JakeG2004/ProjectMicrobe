using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Toxin
{
    public float toxicity = 0.0f;
    public float minSafeDensity = 0.0f;
    public float maxSafeDensity = 0.0f;
    public float lethalDensity = 0.0f;

    public Toxin(float initToxicity, float initMinSafeDensity, float initMaxSafeDensity, float initLethalDensity)
    {
        toxicity = initToxicity;
        minSafeDensity = initMinSafeDensity;
        maxSafeDensity = initMaxSafeDensity;
        lethalDensity = initLethalDensity;
    }
}
