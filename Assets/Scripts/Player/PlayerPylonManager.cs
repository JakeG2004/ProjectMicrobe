using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPylonManager : MonoBehaviour
{
    private CarriedMicrobes _cm;
    [SerializeField] private UnityEvent _onTakePylon;

    void Start()
    {
        _cm = Object.FindObjectOfType<CarriedMicrobes>();
    }

    public void GivePylon()
    {
        _cm.SetHasPylon(true);
    }

    public void TakePylon()
    {
        if (!_cm.HasPylon())
        {
            return;
        }

        _cm.SetHasPylon(false);
        _onTakePylon?.Invoke();
    }
}
