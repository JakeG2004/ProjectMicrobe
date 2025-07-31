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

public class VoidGameEventListener : MonoBehaviour, IGameEventListener<VoidType>
{
    [SerializeField] private VoidGameEventSO _event;
    [SerializeField] private UnityEvent _response;
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

    public void OnEventRaised(VoidType obj)
    {
        // PrintObjective(obj);
        _response?.Invoke();
    }

    public void AnnounceToDebug()
    {
        Debug.Log("GOT VOID EVENT");
    }
}