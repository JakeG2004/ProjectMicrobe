/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

ScriptableObject Base for handling game events

Author: Jake Gendreau
Date:   5/15/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BaseGameEventSO", menuName = "ScriptableObjects/Events/BaseGameEventSO")]
public class BaseGameEventSO : ScriptableObject
{
    // The list of listeners to be notified upon event raise
    private readonly List<IGameEventListener> _eventListeners = new List<IGameEventListener>();

    // Function to raise the event, and call the function on all listeners
    public void Raise()
    {
        for(int i = _eventListeners.Count - 1; i >= 0; i--)
        {
            _eventListeners[i].OnEventRaised();
        }
    }

    // Adds a new listener iff it is not already in the list
    public void RegisterListener(IGameEventListener listener)
    {
        if(!_eventListeners.Contains(listener))
        {
            _eventListeners.Add(listener);
        }
    }

    // Removes a listener from the list iff its already in the list
    public void UnregisterListener(IGameEventListener listener)
    {
        if(_eventListeners.Contains(listener))
        {
            _eventListeners.Remove(listener);
        }
    }
}
