/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Script for raising ObjectiveGameEvents

Author: Jake Gendreau
Date:   5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidGameEventTrigger : MonoBehaviour
{
    [SerializeField] private VoidGameEventSO _event;

    public void TriggerEvent()
    {
        if (_event == null)
        {
            return;
        }

        _event.Raise(new VoidType());
    }
}
