// RegionPlantGrower.cs
// A script for growing plants in a region and triggering events as needed
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RegionPlantGrower : MonoBehaviour
{
    // UNITY EVENTS
    [SerializeField] private UnityEvent _onStartGrowth;
    [SerializeField] private UnityEvent _onGrowthComplete;

    // CONTROL FLOW VARIABLES
    [SerializeField] private float _growthTime = 120f;
    private bool _isStarted = false;

    // GROWTH VARIABLES
    [SerializeField] private GameObject _plantParent;
    private float _envHealth = 0;
    private Dictionary<GameObject, Vector3> _plantScales = new();

    void Start()
    {
        // Store the max growth size for each plant in the region
        foreach (Transform child in _plantParent.transform)
        {
            _plantScales.Add(child.gameObject, child.localScale);
            child.localScale = Vector3.zero;
        }
    }

    // Starts the growth routine
    public void StartGrowth()
    {
        // Early return if already started
        if (_isStarted)
        {
            return;
        }

        _isStarted = true;

        _onStartGrowth?.Invoke();
        StartCoroutine(IGrowPlants());
    }

    // Sets the environmental health, starts growth
    // Gets called from PylonStatusEventsChecker
    public void SetEnvHealth(float val)
    {
        _envHealth = val;

        if (_envHealth >= 1)
        {
            StartGrowth();
        }
    }

    public void InstantSetHealth(float envHealth)
    {
        _envHealth = envHealth;

        TerrainBlender tb = GetComponent<TerrainBlender>();

        if (tb == null)
        {
            return;
        }

        UpdatePlantSize(envHealth);

        tb.SetBlendFactor(_envHealth);
        tb.SetDetailDensity(_envHealth);

        if (_envHealth >= 1)
        {
            _isStarted = true;
            _onGrowthComplete.Invoke();
        }
    }

    private IEnumerator IGrowPlants()
    {
        TerrainBlender tb = GetComponent<TerrainBlender>();

        float curTime = 0; //_envHealth * _growthTime;
        while (curTime < _growthTime)
        {
            curTime += Time.deltaTime;
            float growthRatio = curTime / _growthTime;

            _envHealth = growthRatio;

            UpdatePlantSize(growthRatio);

            if (tb != null)
            {
                tb.SetBlendFactor(growthRatio);
                tb.SetDetailDensity(growthRatio);
            }

            yield return null;
        }

        curTime = _growthTime;
        _onGrowthComplete?.Invoke();

        UpdatePlantSize(1);

        if (tb != null)
        {
            tb.SetBlendFactor(1);
            tb.SetDetailDensity(1);
        }
    }

    public void UpdatePlantSize(float size)
    {
        foreach (var kvp in _plantScales)
        {
            kvp.Key.transform.localScale = new Vector3(size * kvp.Value.x, size * kvp.Value.y, size * kvp.Value.z);
        }
    }
}
