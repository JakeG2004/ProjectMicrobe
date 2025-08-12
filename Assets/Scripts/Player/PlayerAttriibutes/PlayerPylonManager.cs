using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPylonManager : BoolGameEventTrigger
{
    private CarriedPylon _cp;
    [SerializeField] private UnityEvent _onTakePylon;
    [SerializeField] private UnityEvent _onGetPylon;

    void Start()
    {
        _cp = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedPylon>();
    }

    public void GivePylon()
    {
        _cp.SetHasPylon(true);
        _onGetPylon.Invoke();
    }

    public void TakePylon()
    {
        if (!_cp.HasPylon() || !_cp.IsInValidRegion())
        {
            return;
        }

        _cp.SetHasPylon(false);
        _onTakePylon?.Invoke();
    }
}
