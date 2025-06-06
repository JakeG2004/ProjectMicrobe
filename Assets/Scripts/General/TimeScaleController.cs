using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    private float _elapsedTime = 0.0f;
    private float _updatePeriod = 15.0f;
    private int _curTimeIdx = 0;

    // Pylon update periods
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _updatePeriod)
        {
            _elapsedTime = 0.0f;
            AdvancePylons();
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
    }
}
