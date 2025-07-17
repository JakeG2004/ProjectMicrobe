/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Script for raising floatGameEvents

Author: Jake Gendreau
Date:   5/15/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatGameEventTrigger : MonoBehaviour
{
    [SerializeField] private FloatGameEventSO _event;

    public void TriggerEvent(float item)
    {
        if(_event == null)
        {
            return;
        }
        
        _event.Raise(item);
    }
}
