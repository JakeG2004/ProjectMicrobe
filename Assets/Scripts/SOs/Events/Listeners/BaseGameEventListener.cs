/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Listener for BaseGameEventSO. Will trigger unityEvent on raise

Author: Jake Gendreau
Date:   5/15/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseGameEventListener<T, E, UER> : MonoBehaviour, IGameEventListener<T>
    where E : BaseGameEventSO<T>
    where UER : UnityEvent<T>
{
    [SerializeField] private E _event;
    [SerializeField] private UER _response;

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

    public void OnEventRaised(T value)
    {
        _response?.Invoke(value);
    }
}
