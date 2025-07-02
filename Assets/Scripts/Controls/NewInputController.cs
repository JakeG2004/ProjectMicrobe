using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewInputController : MonoBehaviour
{
    public static NewInputController Instance { get; private set; }
    private PlayerInputActions _pia;
    private InputType _curDevice = InputType.Unknown;

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

        _pia = new();
        Set3DMode();

        // Hook into relevant actions to detect input device
        foreach (var map in _pia.asset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                action.performed += UpdateInputDevice;
            }
        }
    }

    public PlayerInputActions GetPlayerInputActions()
    {
        return _pia;
    }

    public void Set3DMode()
    {
        _pia.Player.Enable();
        _pia.Minigames.Disable();
    }

    public void SetMenuMode()
    {
        _pia.Player.Disable();
        _pia.Minigames.Disable();
    }

    public void SetMinigameMode()
    {
        _pia.Player.Disable();
        _pia.Minigames.Enable();
    }

    public InputType GetCurrentInputDevice()
    {
        return _curDevice;
    }

    private void UpdateInputDevice(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        var device = ctx.control.device;

        if (device is UnityEngine.InputSystem.Gamepad)
        {
            _curDevice = InputType.Controller;
        }

        else if (device is UnityEngine.InputSystem.Keyboard || device is UnityEngine.InputSystem.Mouse)
        {
            _curDevice = InputType.KeyboardMouse;
        }

        else
        {
            _curDevice = InputType.Unknown;
        }
    }
}

public enum InputType
{
    KeyboardMouse,
    Controller,
    Unknown,
};
