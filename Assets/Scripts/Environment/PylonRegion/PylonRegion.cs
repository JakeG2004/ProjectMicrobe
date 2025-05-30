using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonRegion : MonoBehaviour
{
    [SerializeField] private EnvironmentSO _envSO;
    [SerializeField] private GameObject _regionPylon;

    public EnvironmentSO GetEnvSO()
    {
        return _envSO;
    }

    public void SetEnvSO(EnvironmentSO newEnvSO)
    {
        _envSO = newEnvSO;
    }

    public void SetRegionPylon(GameObject pylon)
    {
        _regionPylon = pylon;
    }

    public bool HasPylon()
    {
        return (_regionPylon != null);
    }
}
