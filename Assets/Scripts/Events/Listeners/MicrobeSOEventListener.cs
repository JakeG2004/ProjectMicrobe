/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Listener for StringGameEventSO. Will trigger unityEvent on raise

Author: Jake Gendreau
Date:   5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicrobeSOEventListener : MonoBehaviour, IGameEventListener<MicrobeSO>
{
    [SerializeField] private MicrobeGameEventSO _event;
    [SerializeField] private UnityEvent<MicrobeSO> _response;
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

    public void OnEventRaised(MicrobeSO obj)
    {
        _response?.Invoke(obj);
    }
}