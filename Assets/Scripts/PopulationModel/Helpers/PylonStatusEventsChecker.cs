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
    [SerializeField] private float _stablePopMean;
    [SerializeField] private float _stablePopVar;
    [SerializeField] private float _stableMycorrhisAmt;
    private List<Microbe> _newMicrobes;
    private List<Microbe> _oldMicrobes = null;
    private MicrobePopSim _mps;
    private bool _isStable = false;

    void Start()
    {
        GetMPS();
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
        // Get variables from MicrobePopSim
        Vector2 mycorrhisVector = _mps.GetMycorrhisStats();
        Vector2 stabilityVector = _mps.GetBioActivity();

        // Pick out the components
        float bioMean = stabilityVector.x;
        float bioVar = stabilityVector.y;

        // Pick out the components of mycorrhis
        float mycorrhisMean = mycorrhisVector.x;
        float mycorrhisVar = mycorrhisVector.y;

        if (bioMean >= _stablePopMean && bioVar <= _stablePopVar && mycorrhisMean > _stableMycorrhisAmt && mycorrhisVar < 5.0f)
        {
            // Trigger the event
            if (!_isStable)
            {
                _stableEvent.Invoke(_mps.GetEnvSO().envName);
            }
            _isStable = true;
        }

        else
        {
            // Trigger the event
            if (_isStable)
            {
                _unstableEvent.Invoke(_mps.GetEnvSO().envName);
            }
            _isStable = false;
        }
    }

    public float GetStableMycorrhisAmt()
    {
        return _stableMycorrhisAmt;
    }

    public void SetStableState(Vector3 vals)
    {
        _stablePopMean = vals.x;
        _stablePopVar = vals.y;
        _stableMycorrhisAmt = vals.z;
    }
}
