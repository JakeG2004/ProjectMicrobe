using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Toxin
{
    public float toxicity = 1.0f;
    public float minSafeDensity = 0.0f;
    public float maxSafeDensity = 0.4f;
    public float lethalDensity = 0.6f;
}
