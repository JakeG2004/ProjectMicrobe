// DroneInputHandler.cs
// A script which manages input while the player is on the drone
// Author:  Jake Gendreau
// Date:    7/28/25

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler
{
    private PlayerStatesSO _states;

    // Subscribable events
    public event Action OnRightStickMove;

    public DroneInputHandler(PlayerStatesSO states, PlayerInputActions pia)
    {
        _states = states;
        _pia = pia;

        _pia.Drone.Disable();
        BindInputActions();
    }

    public void Dispose()
    {
        _pia.UI.Disable();
        UnbindInputActions();
    }

    private void HandleRightStickPerformed(InputAction.CallbackContext ctx)
    {
        _states.minigameMove = ctx.ReadValue<float>();
        OnRightStickMove?.Invoke();
    }


    private void BindInputActions()
    {
        _pia.UI.RightStick.performed += HandleRightStickPerformed;
    }

    private void UnbindInputActions()
    {
        _pia.UI.RightStick.performed -= HandleRightStickPerformed;
    }
}
