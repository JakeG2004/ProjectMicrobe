// DroneInputHandler.cs
// A script which manages input while the player is on the drone
// Author:  Jake Gendreau
// Date:    7/28/25

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInputHandler
{
    private PlayerStatesSO _states;
    private PlayerInputActions _pia;

    // Subscribable events
    public event Action OnVerticalMovePressed;
    public event Action OnVerticalMoveCanceled;
    public event Action OnDismountPressed;

    public DroneInputHandler(PlayerStatesSO states, PlayerInputActions pia)
    {
        _states = states;
        _pia = pia;

        _pia.Drone.Disable();
        BindInputActions();
    }

    public void Dispose()
    {
        _pia.Drone.Disable();
        UnbindInputActions();
    }

    private void HandleVerticalMove(InputAction.CallbackContext ctx)
    {
        _states.verticalMove = ctx.ReadValue<float>();
        OnVerticalMovePressed?.Invoke();
    }

    private void HandleVerticalMoveCancelled(InputAction.CallbackContext ctx)
    {
        _states.verticalMove = 0;
        OnVerticalMoveCanceled?.Invoke();
    }

    private void HandleDismountPressed(InputAction.CallbackContext ctx)
    {
        _states.isFlying = false;
        OnDismountPressed?.Invoke();
    }

    private void BindInputActions()
    {
        _pia.Drone.VerticalMove.performed += HandleVerticalMove;
        _pia.Drone.VerticalMove.canceled += HandleVerticalMoveCancelled;
        _pia.Drone.Dismount.started += HandleDismountPressed;
    }

    private void UnbindInputActions()
    {
        _pia.Drone.VerticalMove.performed -= HandleVerticalMove;
        _pia.Drone.VerticalMove.canceled -= HandleVerticalMoveCancelled;
        _pia.Drone.Dismount.started -= HandleDismountPressed;
    }
}
