/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Listener for ObjectiveGameEventSO. Will trigger unityEvent on raise

Author: Jake Gendreau
Date:   5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveGameEventListener: MonoBehaviour, IGameEventListener<Objective>
{
    [SerializeField] private ObjectiveGameEventSO _event;
    [SerializeField] private UnityEvent<Objective> _response;

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

    public void OnEventRaised(Objective obj)
    {
        // PrintObjective(obj);
        _response?.Invoke(obj);
    }

    public void PrintObjective(Objective obj)
    {
        string debugText = obj.GetObjectiveText() + "\n";
        debugText += "Is Activated: " + obj.IsActivated() + "\n";
        debugText += "Is Complete: " + obj.IsComplete() + "\n";
        debugText += "Is Failed: " + obj.IsFailed() + "\n";

        Debug.Log(debugText);   
    }
}
