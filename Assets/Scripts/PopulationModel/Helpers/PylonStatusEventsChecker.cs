// ExtinctionChecker.cs
// A script that will check for extinction and do an event when it happens
// Author:  Jake Gendreau
// Date:    6/2/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PylonStatusEventsChecker : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> _extinctEvent;
    [SerializeField] private UnityEvent<string> _stableEvent;
    [SerializeField] private UnityEvent<string> _unstableEvent;
    [SerializeField] private float _stableMycorrhisMean;
    [SerializeField] private float _stableMycorrhisVar;
    private List<Microbe> _newMicrobes;
    private List<Microbe> _oldMicrobes = null;
    private MicrobePopSim _mps;
    private bool _isStable = false;
    private PylonRegion _pr;

    // Vars for calculating health
    private float _envHealth = 0.0f;
    private const int HEALTH_SAMPLE_SIZE = 5;
    private float[] _healthHist = new float[HEALTH_SAMPLE_SIZE];
    private int _curHealthIdx = 0;

    void Start()
    {
        GetMPS();

        if (_envHealth == 0)
        {
            // InitializeValues
            for (int i = 0; i < HEALTH_SAMPLE_SIZE; i++)
            {
                _healthHist[i] = 0;
            }
        }
    }

    private void GetMPS()
    {
        if (_mps == null)
        {
            _mps = GetComponent<MicrobePopSim>();
        }
    }

    public void CheckNotifications()
    {
        GetMPS();

        // Get the microbes from the simulation
        _newMicrobes = _mps.GetMicrobes();

        CheckExtinction();
        CheckStability();
        UpdateEnvironmentalHealthList();
        UpdateEnvironmentHealth();

        // Set the old microbes
        _oldMicrobes = new List<Microbe>();
        foreach (Microbe m in _newMicrobes)
        {
            _oldMicrobes.Add(m.Clone());
        }
    }

    private void CheckExtinction()
    {
        // Skip first time step
        if (_oldMicrobes == null)
        {
            _oldMicrobes = new List<Microbe>();
            foreach (Microbe m in _newMicrobes)
            {
                _oldMicrobes.Add(m.Clone()); // assumes Clone() returns a copy of the microbe
            }
            return;
        }

        foreach (Microbe newMicrobe in _newMicrobes)
        {
            // Find the corresponding microbe in oldMicrobes
            foreach (Microbe oldMicrobe in _oldMicrobes)
            {
                if (oldMicrobe.microbeName == newMicrobe.microbeName)
                {
                    // If the old pop > 0 and the new pop = 0, then trigger the event
                    if (oldMicrobe.population > 0 && newMicrobe.population <= 0)
                    {
                        _extinctEvent.Invoke(newMicrobe.microbeName);
                    }
                }
            }
        }
    }

    private void CheckStability()
    {
        bool _curStable = (_envHealth == 1.0f);

        // Handle stable -> unstable
        if (_isStable && !_curStable)
        {
            _unstableEvent.Invoke(_mps.GetEnvSO().envName);
            _isStable = false;
            return;
        }

        // unstable -> stable
        if (!_isStable && _curStable)
        {
            _stableEvent.Invoke(_mps.GetEnvSO().envName);
            _isStable = true;
            return;
        }
    }

    public float GetStableMycorrhisAmt()
    {
        return _stableMycorrhisMean;
    }

    public void SetStableState(Vector2 vals)
    {
        _stableMycorrhisMean = vals.x;
        _stableMycorrhisVar = vals.y;
    }

    // Environmental health calculated as the mean of the sample mean of the Mycorrhis population
    public void CalculateEnvironmentalHealth()
    {
        // Start with health at 0
        float healthMean = 0.0f;

        // Check that health array is full
        foreach (float healthStep in _healthHist)
        {
            healthMean += healthStep;
        }

        healthMean /= (HEALTH_SAMPLE_SIZE);

        _envHealth = healthMean;
    }

    public void UpdateEnvironmentalHealthList()
    {
        // Get the stats from the pop sim
        Vector2 stats = _mps.GetMycorrhisStats();

        // Add to the list, 0 if too much variance or mean is less than the expected mean
        // 1 otherwise
        _healthHist[_curHealthIdx] = (stats.x > _stableMycorrhisMean && stats.y < _stableMycorrhisVar) ? 1 : 0;
        _curHealthIdx = (_curHealthIdx + 1) % HEALTH_SAMPLE_SIZE;
    }

    public float GetEnvHealth()
    {
        CalculateEnvironmentalHealth();
        return _envHealth;
    }

    public void SetRegion(PylonRegion pr)
    {
        if (_pr == null)
        {
            _pr = pr;
        }
    }

    public void UpdateEnvironmentHealth()
    {
        CalculateEnvironmentalHealth();
        _pr.SetEnvHealth(_envHealth);
    }

    public float[] GetHealthHist()
    {
        return _healthHist;
    }

    public void SetHealthHist(float[] newHist)
    {
        _healthHist = newHist;
        CalculateEnvironmentalHealth();
        _pr.InstantSetHealth(_envHealth);

        if (_envHealth >= 1.0f)
        {
            this.gameObject.GetComponent<StabilityLightController>().UpdateStability(true);
        }
    }
}
