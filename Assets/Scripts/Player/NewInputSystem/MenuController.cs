using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private NewInputController _controller;
    private StringGameEventTrigger _menuTrigger;

    void Awake()
    {
        _menuTrigger = GetComponent<StringGameEventTrigger>();
    }

    void Start()
    {
        _controller = NewInputController.Instance;
        BindControls();
    }

    void OnDisable()
    {
        UnbindControls();
    }

    public void UnlockTablet()
    {
        _controller.generalInput.UnlockTablet();
    }

    private void BindControls()
    {
        _controller.generalInput.OnMenuDown += HandleMenuDown;
        _controller.generalInput.OnTabletDown += HandleTabletDown;
    }

    private void UnbindControls()
    {
        if (_controller == null)
        {
            return;
        }
        
        _controller.generalInput.OnMenuDown -= HandleMenuDown;
        _controller.generalInput.OnTabletDown -= HandleTabletDown;
    }

    private void HandleMenuDown()
    {
        _menuTrigger.TriggerEvent("PauseMenu");
    }

    private void HandleTabletDown()
    {
        _menuTrigger.TriggerEvent("Tablet");
    }
}
