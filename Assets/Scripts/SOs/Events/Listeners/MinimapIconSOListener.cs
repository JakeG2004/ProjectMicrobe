/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Listener for StringGameEventSO. Will trigger unityEvent on raise

Author: Jake Gendreau
Date:   1/22/26
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinimapIconSOListener : MonoBehaviour, IGameEventListener<MinimapIcon>
{
    [SerializeField] private MinimapIconSO _event;
    [SerializeField] private UnityEvent<MinimapIcon> _response;
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

    public virtual void OnEventRaised(MinimapIcon obj)
    {
        _response?.Invoke(obj);
    }
}