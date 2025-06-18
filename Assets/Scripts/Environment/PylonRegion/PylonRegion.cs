using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PylonRegion : MonoBehaviour
{
    [SerializeField] private EnvironmentSO _envSO;
    [SerializeField] private GameObject _regionPylon;
    [SerializeField] private UnityEvent _getPylonEvent;
    [SerializeField] private GameObject _pylonPrefab;
    private Dictionary<GameObject, Vector3> _plantScales = new();

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
        _getPylonEvent?.Invoke();

    }

    public bool HasPylon()
    {
        return (_regionPylon != null);
    }

    public GameObject GetPylon()
    {
        return _regionPylon;
    }

    public GameObject GetPylonPrefab()
    {
        return _pylonPrefab;
    }
}
