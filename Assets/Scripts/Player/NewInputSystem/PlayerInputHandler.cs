// PlayerInputHandler.cs
// A script for handling player input
// Author:  Jake Gendreau
// Date:    7/18/25

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerStatesSO _states;
    private PlayerInputActions _playerInputActions;

    // Events that other components can subscribe to
    public event System.Action OnJumpStarted;
    public event System.Action OnSprintToggled;

    void Awake()
    {
        _states = GetComponent<PlayerController>().GetStates();
    }

    void Start()
    {
        _playerInputActions = NewInputController.Instance.GetPlayerInputActions();

        _playerInputActions.Player.Enable();
        BindInputActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        _playerInputActions.Player.Disable();
        UnbindInputActions();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Subscribes to the events regarding our inputs
    private void BindInputActions()
    {
        _playerInputActions.Player.Jump.started += ctx => OnJumpStarted?.Invoke();

        // Movement
        _playerInputActions.Player.Movement.performed += ctx => _states.move = ctx.ReadValue<Vector2>();
        _playerInputActions.Player.Movement.canceled += ctx => _states.move = Vector2.zero;

        _playerInputActions.Player.Sprint.started += ctx => OnSprintToggled?.Invoke();

        // Looking
        _playerInputActions.Player.Look.performed += ctx => _states.look = ctx.ReadValue<Vector2>();
        _playerInputActions.Player.Look.canceled += ctx => _states.look = Vector2.zero;

        // Zoom
        _playerInputActions.Player.Zoom.performed += ctx => _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(ctx.ReadValue<float>()) * _states.movementVals.scrollAmt);
    }

    // Unsubscribe to avoid errors / memleaks
    private void UnbindInputActions()
    {
        _playerInputActions.Player.Jump.started -= ctx => OnJumpStarted?.Invoke();
        _playerInputActions.Player.Movement.performed -= ctx => _states.move = ctx.ReadValue<Vector2>();
        _playerInputActions.Player.Movement.canceled -= ctx => _states.move = Vector2.zero;
        _playerInputActions.Player.Sprint.started -= ctx => OnSprintToggled?.Invoke();
        _playerInputActions.Player.Look.performed -= ctx => _states.look = ctx.ReadValue<Vector2>();
        _playerInputActions.Player.Look.canceled -= ctx => _states.look = Vector2.zero;
        _playerInputActions.Player.Zoom.performed -= ctx => _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(ctx.ReadValue<float>()) * _states.movementVals.scrollAmt);
    }

    public void SetLookSensitivity(float val)
    {
        _states.movementVals.lookSensitivity = val;
    }

    public float GetLookSensitivity()
    {
        return _states.movementVals.lookSensitivity;
    }
}
