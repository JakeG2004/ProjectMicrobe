// MultiKeyEvent.cs
// A script for manging input from multiple keys and calling an event
// Author:  Jake Gendreau
// Date:    6/1/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiKeyEvent : MonoBehaviour
{
    [SerializeField] private KeyCode[] _activationKeys;
    [SerializeField] private UnityEvent _event;

    private bool _eventTriggered = false;

    void Update()
    {
        _eventTriggered = false;

        foreach(KeyCode kc in _activationKeys)
        {
            if(Input.GetKeyDown(kc) && !_eventTriggered)
            {
                _eventTriggered = true;
                _event?.Invoke();
            }
        }
    }
}
