// GeneralController.cs
// A script for managing the universal / general controls in the game
// Author:  Jake Gendreau
// Date:    7/28/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralController : MonoBehaviour
{
    public static GeneralController Instance;
    [SerializeField] private PlayerStatesSO _states;
    private PlayerInputActions _playerInputActions;

    // Subscribable events
    public event System.Action OnMenuDown;
    public event System.Action OnTimeDown;
    public event System.Action OnTabletDown;
    public event System.Action OnLookPerformed;
    public event System.Action OnZoomPerformed;
    public event System.Action OnMovePerformed;

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
        _playerInputActions = NewInputController.Instance.GetPlayerInputActions();

        _playerInputActions.BaseControls.Enable();
        BindInputActions();
    }

    void OnDisable()
    {
        _playerInputActions.BaseControls.Disable();
        UnbindInputActions();
    }

    private void BindInputActions()
    {
        // Movement
        _playerInputActions.BaseControls.Movement.performed += ctx => { _states.move = ctx.ReadValue<Vector2>(); OnMovePerformed?.Invoke(); };
        _playerInputActions.BaseControls.Movement.canceled += ctx => _states.move = Vector2.zero;

        // Looking
        _playerInputActions.BaseControls.Look.performed += ctx => { _states.look = ctx.ReadValue<Vector2>(); OnLookPerformed?.Invoke(); };
        _playerInputActions.BaseControls.Look.canceled += ctx => _states.look = Vector2.zero;

        // Zoom
        _playerInputActions.BaseControls.Zoom.performed += ctx => { _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(ctx.ReadValue<float>()) * _states.movementVals.scrollAmt); OnZoomPerformed?.Invoke(); };

        // Menus and other general interactions
        _playerInputActions.BaseControls.Menu.started += ctx => OnMenuDown?.Invoke();
        _playerInputActions.BaseControls.Time.started += ctx => OnTimeDown?.Invoke();
        _playerInputActions.BaseControls.Tablet.started += ctx => OnTabletDown?.Invoke();
    }

    private void UnbindInputActions()
    {
        // Movement
        _playerInputActions.BaseControls.Movement.performed -= ctx => _states.move = ctx.ReadValue<Vector2>();
        _playerInputActions.BaseControls.Movement.canceled -= ctx => _states.move = Vector2.zero;

        // Looking
        _playerInputActions.BaseControls.Look.performed -= ctx => _states.look = ctx.ReadValue<Vector2>();
        _playerInputActions.BaseControls.Look.canceled -= ctx => _states.look = Vector2.zero;

        // Zoom
        _playerInputActions.BaseControls.Zoom.performed -= ctx => _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(ctx.ReadValue<float>()) * _states.movementVals.scrollAmt);
    
        // Menus and other general interactions
        _playerInputActions.BaseControls.Menu.started -= ctx => OnMenuDown?.Invoke();
        _playerInputActions.BaseControls.Time.started -= ctx => OnTimeDown?.Invoke();
        _playerInputActions.BaseControls.Tablet.started -= ctx => OnTabletDown?.Invoke();
 
    }
}
