using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NewInputController))]
public class WorldAndMenuControls : MonoBehaviour
{
    [SerializeField] private UnityEvent _onMenuPressed;
    [SerializeField] private UnityEvent _onInteractPressed;
    [SerializeField] private UnityEvent _onTimePressed;
    private PlayerInputActions _pia;

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        _pia.Player.Menu.started += ctx => _onMenuPressed.Invoke();
        _pia.Player.Interact.started += ctx => _onInteractPressed.Invoke();
        _pia.Player.Time.started += ctx => _onTimePressed.Invoke();
    }
}
