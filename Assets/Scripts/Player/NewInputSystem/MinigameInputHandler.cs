// DroneInputHandler.cs
// A script which manages input while the player is on the drone
// Author:  Jake Gendreau
// Date:    7/28/25

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameInputHandler
{
    private PlayerStatesSO _states;
    private PlayerInputActions _pia;

    // Subscribable events
    public event Action OnMovePressed;
    public event Action OnSelectPressed;
    public event Action OnSelectCanceled;
    public event Action OnBackPressed;

    public MinigameInputHandler(PlayerStatesSO states, PlayerInputActions pia)
    {
        _states = states;
        _pia = pia;

        _pia.Minigames.Disable();
        BindInputActions();
    }

    public void Dispose()
    {
        _pia.Drone.Disable();
        UnbindInputActions();
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        _states.minigameMove = ctx.ReadValue<Vector2>();
        OnMovePressed?.Invoke();
    }

    private void HandleMoveCancelled(InputAction.CallbackContext ctx)
    {
        _states.minigameMove = Vector2.zero;
    }

    private void HandleSelect(InputAction.CallbackContext ctx) => OnSelectPressed?.Invoke();
    private void HandleSelectCanceled(InputAction.CallbackContext ctx) => OnSelectCanceled?.Invoke();
    private void HandleBack(InputAction.CallbackContext ctx) => OnBackPressed?.Invoke();


    private void BindInputActions()
    {
        _pia.Minigames.Move.performed += HandleMove;
        _pia.Minigames.Move.canceled += HandleMoveCancelled;
        _pia.Minigames.Select.performed += HandleSelect;
        _pia.Minigames.Select.canceled += HandleSelectCanceled;
        _pia.Minigames.Back.performed += HandleBack;
    }

    private void UnbindInputActions()
    {
        _pia.Minigames.Move.performed -= HandleMove;
        _pia.Minigames.Move.canceled -= HandleMoveCancelled;
        _pia.Minigames.Select.performed -= HandleSelect;
        _pia.Minigames.Select.canceled -= HandleSelectCanceled;
        _pia.Minigames.Back.performed -= HandleBack;
    }
}
