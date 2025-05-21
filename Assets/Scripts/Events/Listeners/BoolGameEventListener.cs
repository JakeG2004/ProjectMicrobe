/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Listener for BoolGameEventSO. Will trigger unityEvent on raise

Author: Jake Gendreau
Date:   5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolGameEventListener : MonoBehaviour, IGameEventListener<bool>
{
    [SerializeField] private BoolGameEventSO _event;
    [SerializeField] private UnityEvent<bool> _response;
    [SerializeField] private bool _boolVal;
    public void OnEnable()
    {
        if (_event != null)
        {
            _event.RegisterListener(this);
        }
    }

    public void OnDisable()
    {
        _event.UnregisterListener(this);
    }

    public void OnEventRaised(bool obj)
    {
        // PrintObjective(obj);
        _response?.Invoke(obj);
        _boolVal = obj;
    }

    public bool GetBoolVal()
    {
        return _boolVal;
    }
}