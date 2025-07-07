using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewInputController : MonoBehaviour
{
    public static NewInputController Instance { get; private set; }
    [SerializeField] private UnityEvent _onMouseKeyboard;
    [SerializeField] private UnityEvent _onGamepad;
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
            DontDestroyOnLoad(this.gameObject);
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

    //void Update()
    //{
    //    Debug.Log($"UI: {_pia.UI.enabled}\nPlayer: {_pia.Player.enabled}\nMinigames: {_pia.Minigames.enabled}");
    //}

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
            if (_curDevice != InputType.Controller)
            {
                _onGamepad.Invoke();
            }

            _curDevice = InputType.Controller;
        }

        else if (device is UnityEngine.InputSystem.Keyboard || device is UnityEngine.InputSystem.Mouse)
        {
            if (_curDevice != InputType.KeyboardMouse)
            {
                _onMouseKeyboard.Invoke();
            }

            _curDevice = InputType.KeyboardMouse;
        }

        else
        {
            _curDevice = InputType.Unknown;
        }
    }

    public void EmitCurDevice()
    {
        Set3DMode();
        if (_curDevice == InputType.Controller)
        {
            _onGamepad.Invoke();
        }

        else
        {
            _onMouseKeyboard.Invoke();
        }
    }
}

public enum InputType
{
    KeyboardMouse,
    Controller,
    Unknown,
};
