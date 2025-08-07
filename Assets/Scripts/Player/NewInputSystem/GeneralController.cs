using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeneralInputController
{
    private PlayerStatesSO _states;
    private PlayerInputActions _playerInputActions;

    // Subscribable events
    public Action OnMenuDown;
    public Action OnTimeDown;
    public Action OnTabletDown;
    public Action OnMovePerformed;
    public Action OnLookPerformed;
    public Action OnZoomPerformed;

    public GeneralInputController(PlayerStatesSO states, PlayerInputActions pia)
    {
        _states = states;

        _playerInputActions = pia;
        BindInputActions();
    }

    public void Reset()
    {
        _playerInputActions.BaseControls.Tablet.started -= HandleTablet;
    }

    public void Dispose()
    {
        UnbindInputActions();
        _playerInputActions.BaseControls.Disable();
    }

    public void UnlockTablet()
    {
        _playerInputActions.BaseControls.Tablet.started += HandleTablet;
    }

    // === Named Methods ===
    private void HandleMove(InputAction.CallbackContext ctx)
    {
        _states.move = ctx.ReadValue<Vector2>();
        OnMovePerformed?.Invoke();
    }

    private void CancelMove(InputAction.CallbackContext ctx)
    {
        _states.move = Vector2.zero;
    }

    private void HandleLook(InputAction.CallbackContext ctx)
    {
        _states.look = ctx.ReadValue<Vector2>();
        OnLookPerformed?.Invoke();
    }

    private void CancelLook(InputAction.CallbackContext ctx)
    {
        _states.look = Vector2.zero;
    }

    private void HandleZoom(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>();
        _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(scroll) * _states.movementVals.scrollAmt);
        OnZoomPerformed?.Invoke();
    }

    private void HandleMenu(InputAction.CallbackContext ctx) => OnMenuDown?.Invoke();
    private void HandleTime(InputAction.CallbackContext ctx) => OnTimeDown?.Invoke();
    private void HandleTablet(InputAction.CallbackContext ctx) => OnTabletDown?.Invoke();

    // === Binding ===
    private void BindInputActions()
    {
        _playerInputActions.BaseControls.Movement.performed += HandleMove;
        _playerInputActions.BaseControls.Movement.canceled += CancelMove;

        _playerInputActions.BaseControls.Look.performed += HandleLook;
        _playerInputActions.BaseControls.Look.canceled += CancelLook;

        _playerInputActions.BaseControls.Zoom.performed += HandleZoom;

        _playerInputActions.BaseControls.Menu.started += HandleMenu;
        _playerInputActions.BaseControls.Time.started += HandleTime;
    }

    private void UnbindInputActions()
    {
        _playerInputActions.BaseControls.Movement.performed -= HandleMove;
        _playerInputActions.BaseControls.Movement.canceled -= CancelMove;

        _playerInputActions.BaseControls.Look.performed -= HandleLook;
        _playerInputActions.BaseControls.Look.canceled -= CancelLook;

        _playerInputActions.BaseControls.Zoom.performed -= HandleZoom;

        _playerInputActions.BaseControls.Menu.started -= HandleMenu;
        _playerInputActions.BaseControls.Time.started -= HandleTime;
        _playerInputActions.BaseControls.Tablet.started -= HandleTablet;
    }
}
