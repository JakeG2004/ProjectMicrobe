// TimeScaleController.cs
// A script for managing time-related thigns in the game
// Author:  Jake Gendreau
// Date:    6/16/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeScaleController : MonoBehaviour
{
    private float _elapsedTime = 0.0f;
    private float _updatePeriod = 15.0f;
    private int _curTimeIdx = 0;
    [SerializeField] private UnityEvent<float> _onChangeUpdatePeriod;

    // Pylon update periods
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _updatePeriod)
        {
            _elapsedTime = 0.0f;
            AdvancePylons();
            AdvanceVats();
        }
    }

    public void AdvancePylons()
    {
        MicrobePopSim[] popSims = FindObjectsOfType<MicrobePopSim>();

        foreach (MicrobePopSim sim in popSims)
        {
            sim.AdvanceSimulation();
        }
    }

    public void AdvanceVats()
    {
        AddMicrobeToggler[] vatTogglers = FindObjectsOfType<AddMicrobeToggler>();

        foreach (AddMicrobeToggler amt in vatTogglers)
        {
            amt.FillPopulation();
        }
    }

    public void ChangeTimeScale()
    {
        // Set pylon time scale
        _curTimeIdx++;
        _curTimeIdx = (_curTimeIdx % 5);

        // Set pylon time scale
        _updatePeriod = 15.0f / (_curTimeIdx + 1);

        // Set the sun time scale
        Sun sun = FindObjectOfType<Sun>();
        float sunTimeScale = (_curTimeIdx + 1) * 20;
        if ((_curTimeIdx + 1) == 1)
        {
            sunTimeScale = 1;
        }

        sun.SetRotationScaler(sunTimeScale);

        _onChangeUpdatePeriod.Invoke(_updatePeriod);
    }

    public void ResetTimeCounter()
    {
        _elapsedTime = 0.0f;
    }
}
