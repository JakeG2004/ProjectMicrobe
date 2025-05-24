// CarriedPylon.cs
// A script to manage the pylon carry state
// Author:  Jake Gendreau
// Date:    5/24/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedPylon : MonoBehaviour
{
    [SerializeField] private bool _hasPylon = false;
    [SerializeField] private bool _validRegion = false;

    public void SetHasPylon(bool state)
    {
        _hasPylon = state;
    }

    public bool HasPylon()
    {
        return _hasPylon;
    }

    public bool IsInValidRegion()
    {
        return _validRegion;
    }

    public bool IsPlaceable()
    {
        return(_hasPylon && _validRegion);
    }

    // For detecting whether the player is in a pylon placeable region
    public void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "PylonRegion")
        {
            _validRegion = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "PylonRegion")
        {
            _validRegion = false;
        }
    }
}
