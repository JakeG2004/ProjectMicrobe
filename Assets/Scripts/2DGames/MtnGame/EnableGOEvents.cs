// EnableGOEvents.cs
// A script for triggering unity events on different occasiosn
// Author:  Jake Gendreau
// Date:    6/9/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableGOEvents : MonoBehaviour
{
    public enum EventType
    {
        AWAKE,
        START,
        ENABLE,
        DISABLE,
        MANUAL,
    };

    [SerializeField] private UnityEvent _response;
    [SerializeField] private EventType _eventType;

    void Awake()
    {
        if (_eventType == EventType.AWAKE)
        {
            _response.Invoke();
        }
    }

    void Start()
    {
        if (_eventType == EventType.START)
        {
            _response.Invoke();
        }
    }

    void OnEnable()
    {
        if (_eventType == EventType.ENABLE)
        {
            _response.Invoke();
        }
    }

    void OnDisable()
    {
        if (_eventType == EventType.DISABLE)
        {
            _response.Invoke();
        }
    }

    public void InvokeEvent()
    {
        _response.Invoke();
    }
}
