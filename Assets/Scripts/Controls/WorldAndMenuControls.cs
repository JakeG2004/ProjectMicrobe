/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WorldAndMenuControls : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTimePressed;
    private NewInputController _controller;

    // Delegate fields for unsubscribing
    private Action _timeDelegate;

    void Start()
    {
        _controller = NewInputController.Instance;


        // Assign delegates to fields so we can unsubscribe later
        _timeDelegate = HandleTimePressed;

        NewInputController.Instance.generalInput.OnTimeDown += _timeDelegate;
    }
}
*/