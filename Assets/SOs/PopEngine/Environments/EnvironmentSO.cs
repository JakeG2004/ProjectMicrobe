using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentSO", menuName = "ScriptableObjects/PopEngine/EnvironmentSO")]

public class EnvironmentSO : ScriptableObject
{
    public List<ResourceAmount> initialResources = new List<ResourceAmount>();
    public List<ResourceAmount> resourceRefresh = new List<ResourceAmount>();
    public string envName = "";
}
