/*
Based on tutorial: https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Raisable event which notifies all registered listeners upon being raised

Author: Jake Gendreau
Date: 5/14/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseGameEvent", menuName = "ScriptableObjects/Events/BaseGameEventSO")]

public class BaseGameEventSO : ScriptableObject
{
    // List of listeners to be notified upon event raise
    private readonly List<IGameEventListener> m_eventListeners = new List<IGameEventListener>();

    // Raise the event for all registered listeners
    public void Raise()
    {
        for(int i = m_eventListeners.Count - 1; i >= 0; i--)
        {
            m_eventListeners[i].OnEventRaised();
        }
    }

    // Registers a new listener iff it is not already in the list
    public void RegisterListener(IGameEventListener listener)
    {
        if(!m_eventListeners.Contains(listener))
        {
            m_eventListeners.Add(listener);
        }
    }

    // Removes a listener from the event iff it is in the list
    public void UnregisterListener(IGameEventListener listener)
    {
        if(m_eventListeners.Contains(listener))
        {
            m_eventListeners.Remove(listener);
        }
    }
}
