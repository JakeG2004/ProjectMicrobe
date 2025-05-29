using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonRegion : MonoBehaviour
{
    [SerializeField] private EnvironmentSO _envSO;

    public EnvironmentSO GetEnvSO()
    {
        return _envSO;
    }

    public void SetEnvSO(EnvironmentSO newEnvSO)
    {
        _envSO = newEnvSO;
    }
}
