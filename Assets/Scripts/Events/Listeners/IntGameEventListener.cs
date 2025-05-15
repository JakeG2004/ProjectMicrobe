/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Listener for IntGameEventSO. Will trigger unityEvent on raise

Author: Jake Gendreau
Date:   5/15/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntGameEventListener: MonoBehaviour, IGameEventListener<int>
{
    [SerializeField] private IntGameEventSO _event;
    [SerializeField] private UnityEvent<int> _response;

    public void OnEnable()
    {
        if(_event != null)
        {
            _event.RegisterListener(this);
        }
    }

    public void OnDisable()
    {
        _event.UnregisterListener(this);
    }

    public void OnEventRaised(int value)
    {
        _response?.Invoke(value);
    }
}
