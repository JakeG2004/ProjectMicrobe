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
    [SerializeField] private Transform _droneLandingPoint;
    private Dictionary<GameObject, Vector3> _plantScales = new();
    [SerializeField] private bool _canPlacePylon = false;

    void Start()
    {
        // Disable the collider on start, this will be enabled when region unlocked
        GetComponent<SphereCollider>().enabled = false;
    }

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

    public void SetCanPlacePylon(bool state)
    {
        _canPlacePylon = state;
    }

    public bool HasPylon()
    {
        return (_regionPylon != null);
    }

    public bool CanPlacePylon()
    {
        return (_canPlacePylon && !HasPylon());
    }

    public GameObject GetPylon()
    {
        return _regionPylon;
    }

    public GameObject GetPylonPrefab()
    {
        return _pylonPrefab;
    }

    public Transform GetDronePoint()
    {
        return _droneLandingPoint;
    }
}
