// PlayerInputHandler.cs
// A script for handling player input
// Author:  Jake Gendreau
// Date:    7/18/25

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    private PlayerStatesSO _states;
    private PlayerInputActions _playerInputActions;

    // Subscribable events
    public Action OnJumpDown;
    public Action OnSprintToggled;
    public Action OnDroneToggled;
    public Action OnInteractDown;

    public PlayerInputHandler(PlayerStatesSO states, PlayerInputActions pia)
    {
        _states = states;
        _playerInputActions = pia;

        BindInputActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Dispose()
    {
        _playerInputActions.Player.Disable();
        UnbindInputActions();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reset()
    {
        _playerInputActions.Player.Drone.started -= HandleDrone;
    }

    public void UnlockDrone()
    {
        _playerInputActions.Player.Drone.started += HandleDrone;
    }

    public void SetLookSensitivity(float val)
    {
        _states.movementVals.lookSensitivity = val;
    }

    public float GetLookSensitivity()
    {
        return _states.movementVals.lookSensitivity;
    }

    private void HandleJump(InputAction.CallbackContext ctx) => OnJumpDown?.Invoke();
    private void HandleSprint(InputAction.CallbackContext ctx) => OnSprintToggled?.Invoke();
    private void HandleDrone(InputAction.CallbackContext ctx) => OnDroneToggled?.Invoke();
    private void HandleInteract(InputAction.CallbackContext ctx) => OnInteractDown?.Invoke();

    // Subscribes to the events regarding our inputs
    private void BindInputActions()
    {
        _playerInputActions.Player.Jump.started += HandleJump;
        _playerInputActions.Player.Sprint.started += HandleSprint;
        _playerInputActions.Player.Interact.started += HandleInteract;
    }

    // Unsubscribe to avoid errors / memleaks
    private void UnbindInputActions()
    {
        _playerInputActions.Player.Jump.started -= HandleJump;
        _playerInputActions.Player.Drone.started -= HandleDrone;
        _playerInputActions.Player.Sprint.started -= HandleJump;
    }
}
