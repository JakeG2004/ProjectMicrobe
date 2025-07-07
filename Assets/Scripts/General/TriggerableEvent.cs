// TriiggerableEvent.cs
// A script for triggering general events
// Author:  Jake Gendreau
// Date:    6/1/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerableEvent : MonoBehaviour
{
    [SerializeField] private bool _activateOnStart = false;
    [SerializeField] private bool _oneShot = true;

    [SerializeField] private UnityEvent _event;
    
    private bool _activated = false;

    void Start()
    {
        if(_activateOnStart)
        {
            ActivateEvent();
        }
    }

    public void ActivateEvent()
    {
        if (_activated && _oneShot)
        {
            return;
        }

        _activated = true;

        _event.Invoke();
    }
}
