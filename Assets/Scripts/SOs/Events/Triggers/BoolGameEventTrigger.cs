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

public class BoolGameEventTrigger : MonoBehaviour
{
    [SerializeField] private BoolGameEventSO _event;

    public void TriggerEvent(bool item)
    {
        if (_event == null)
        {
            return;
        }

        _event.Raise(item);
    }
}
