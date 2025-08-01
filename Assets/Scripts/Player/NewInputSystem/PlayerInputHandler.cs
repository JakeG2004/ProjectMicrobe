// PlayerInputHandler.cs
// A script for handling player input
// Author:  Jake Gendreau
// Date:    7/18/25

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance;
    private PlayerStatesSO _states;
    private PlayerInputActions _playerInputActions;

    // Events that other components can subscribe to
    public event System.Action OnJumpDown;
    public event System.Action OnSprintToggled;
    public event System.Action OnDroneToggled;

    public void Init(PlayerStatesSO states)
    {
        _states = states;
    }

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
        _playerInputActions.Player.Jump.started += ctx => OnJumpDown?.Invoke();
        _playerInputActions.Player.Sprint.started += ctx => OnSprintToggled?.Invoke();
    }

    // Unsubscribe to avoid errors / memleaks
    private void UnbindInputActions()
    {
        _playerInputActions.Player.Jump.started -= ctx => OnJumpDown?.Invoke();
        _playerInputActions.Player.Drone.started -= ctx => OnDroneToggled?.Invoke();
        _playerInputActions.Player.Sprint.started -= ctx => OnSprintToggled?.Invoke();
    }

    public void UnlockDrone()
    {
        _playerInputActions.Player.Drone.started += ctx => OnDroneToggled?.Invoke();
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
