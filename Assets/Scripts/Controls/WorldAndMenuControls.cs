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

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        _pia.Player.Interact.started += ctx => _onInteractPressed.Invoke();

        GeneralController.Instance.OnMenuDown += (() => _onMenuPressed.Invoke());
        GeneralController.Instance.OnTimeDown += (() => _onTimePressed.Invoke());
        GeneralController.Instance.OnTabletDown += (() => _onTabletPressed.Invoke());

        _pia.Minigames.Back.started += ctx => _on2dMenuBackPressed.Invoke();
    }
}
