using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentSO", menuName = "ScriptableObjects/PopEngine/EnvironmentSO")]

public class EnvironmentSO : ScriptableObject
{
    public List<ResourceAmount> initialResources = new List<ResourceAmount>();
    public List<ResourceAmount> resourceRefresh = new List<ResourceAmount>();
    public List<MicrobeSOPopPair> initialMicrobes = new List<MicrobeSOPopPair>();
    public string envName = "";
    public float stableMean;
    public float stableVar;
    public float stableMycorrhis;
}

[System.Serializable]
public class MicrobeSOPopPair
{
    [SerializeField]
    public MicrobeSO microbe;

    [SerializeField]
    public float population;
}
