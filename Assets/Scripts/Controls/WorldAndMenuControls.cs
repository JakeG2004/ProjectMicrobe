using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WorldAndMenuControls : MonoBehaviour
{
    [SerializeField] private UnityEvent _onMenuPressed;
    [SerializeField] private UnityEvent _onTabletPressed;
    [SerializeField] private UnityEvent _onInteractPressed;
    [SerializeField] private UnityEvent _onTimePressed;
    [SerializeField] private UnityEvent _on2dMenuBackPressed;

    private PlayerInputActions _pia;

    // Delegate fields for unsubscribing
    private Action _menuDelegate;
    private Action _timeDelegate;
    private Action _tabletDelegate;

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        _pia.Player.Interact.started += OnInteract;
        _pia.Minigames.Back.started += On2DMenuBack;

        // Assign delegates to fields so we can unsubscribe later
        _menuDelegate = HandleMenuPressed;
        _timeDelegate = HandleTimePressed;

        GeneralController.Instance.OnMenuDown += _menuDelegate;
        GeneralController.Instance.OnTimeDown += _timeDelegate;
    }

    private void OnDisable()
    {
        _pia.Player.Interact.started -= OnInteract;
        _pia.Minigames.Back.started -= On2DMenuBack;

        if (GeneralController.Instance != null)
        {
            GeneralController.Instance.OnMenuDown -= _menuDelegate;
            GeneralController.Instance.OnTimeDown -= _timeDelegate;

            if (_tabletDelegate != null)
                GeneralController.Instance.OnTabletDown -= _tabletDelegate;
        }
    }

    public void UnlockTablet()
    {
        if (_tabletDelegate == null)
        {
            _tabletDelegate = HandleTabletPressed;
            GeneralController.Instance.OnTabletDown += _tabletDelegate;
        }
    }

    // === Named Methods for Delegates ===
    private void OnInteract(InputAction.CallbackContext ctx) => _onInteractPressed.Invoke();
    private void On2DMenuBack(InputAction.CallbackContext ctx) => _on2dMenuBackPressed.Invoke();
    private void HandleMenuPressed() => _onMenuPressed.Invoke();
    private void HandleTimePressed() => _onTimePressed.Invoke();
    private void HandleTabletPressed() => _onTabletPressed.Invoke();
}
