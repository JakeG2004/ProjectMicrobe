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

public class StringGameEventListener : MonoBehaviour, IGameEventListener<string>
{
    [SerializeField] private StringGameEventSO _event;
    [SerializeField] private string _queryString;
    [SerializeField] private UnityEvent<string> _generalResponse;
    [SerializeField] private UnityEvent<string> _specificResponse;
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

    public void OnEventRaised(string obj)
    {
        // PrintObjective(obj);
        _generalResponse?.Invoke(obj);

        if (obj == _queryString)
        {
            _specificResponse?.Invoke(obj);
        }
    }

    public void PrintString(string str)
    {
        Debug.Log(str);
    }
}