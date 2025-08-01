using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeneralController : MonoBehaviour
{
    public static GeneralController Instance;

    [SerializeField] private PlayerStatesSO _states;
    private PlayerInputActions _playerInputActions;

    // Subscribable events
    public event Action OnMenuDown;
    public event Action OnTimeDown;
    public event Action OnTabletDown;
    public event Action OnLookPerformed;
    public event Action OnZoomPerformed;
    public event Action OnMovePerformed;

    void Awake()
    {
        if (Instance != null && Instance != this)
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
        UnbindInputActions();
        _playerInputActions.BaseControls.Disable();
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
        _playerInputActions.BaseControls.Tablet.started += HandleTablet;
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
