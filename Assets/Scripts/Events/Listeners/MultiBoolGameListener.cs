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

public class MultiBoolGameEventListener : MonoBehaviour, IGameEventListener<bool>
{
    [SerializeField] private List<BoolGameEventSO> _events = new();
    [SerializeField] private UnityEvent<bool> _response;
    [SerializeField] private UnityEvent _onBoolTrue;
    [SerializeField] private UnityEvent _onBoolFalse;
    [SerializeField] private bool _boolVal;
    public void OnEnable()
    {
        foreach (BoolGameEventSO boolEvent in _events)
        {
            boolEvent.RegisterListener(this);
        }
    }

    public void OnDisable()
    {
        foreach (BoolGameEventSO boolEvent in _events)
        {
            boolEvent.UnregisterListener(this);
        }
    }

    public void OnEventRaised(bool obj)
    {
        // PrintObjective(obj);
        _response?.Invoke(obj);
        _boolVal = obj;

        if(_boolVal)
        {
            _onBoolTrue?.Invoke();
        }

        if(!_boolVal)
        {
            _onBoolFalse?.Invoke();
        }
    }

    public bool GetBoolVal()
    {
        return _boolVal;
    }
}