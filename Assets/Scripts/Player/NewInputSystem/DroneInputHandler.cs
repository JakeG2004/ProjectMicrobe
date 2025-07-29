// DroneInputHandler.cs
// A script which manages input while the player is on the drone
// Author:  Jake Gendreau
// Date:    7/28/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class DroneInputHandler : MonoBehaviour
{
    public static DroneInputHandler Instance;
    private PlayerStatesSO _states;
    private PlayerInputActions _pia;

    // Events
    public event System.Action OnVerticalMovePressed;
    public event System.Action OnVerticalMoveCanceled;
    public event System.Action OnDismountPressed;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        _states = GetComponent<PlayerController>().GetStates();
        _pia = NewInputController.Instance.GetPlayerInputActions();

        _pia.Drone.Disable();
        BindInputActions();
    }

    void OnDisable()
    {
        _pia.Drone.Disable();
        UnbindInputActions();
    }

    private void BindInputActions()
    {
        _pia.Drone.VerticalMove.performed += ctx => { _states.verticalMove = ctx.ReadValue<float>(); OnVerticalMovePressed?.Invoke(); };
        _pia.Drone.VerticalMove.canceled += ctx => { _states.verticalMove = 0; OnVerticalMoveCanceled?.Invoke(); };
        _pia.Drone.Dismount.started += ctx => { _states.isFlying = false; OnDismountPressed?.Invoke(); };
    }

    private void UnbindInputActions()
    {
        _pia.Drone.VerticalMove.performed -= ctx => { _states.verticalMove = ctx.ReadValue<float>(); OnVerticalMovePressed?.Invoke(); };
        _pia.Drone.VerticalMove.canceled -= ctx => { _states.verticalMove = 0; OnVerticalMoveCanceled?.Invoke(); };
        _pia.Drone.Dismount.started -= ctx => { _states.isFlying = false; OnDismountPressed?.Invoke(); };
    }
}
