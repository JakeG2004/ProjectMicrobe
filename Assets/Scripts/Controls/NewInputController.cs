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
    private ControlMap _curMap = ControlMap.PLAYER;

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
        _pia.Drone.Disable();
        _pia.BaseControls.Enable();
        _pia.UI.Disable();

        _curMap = ControlMap.PLAYER;
    }

    public void SetMenuMode()
    {
        _pia.Player.Disable();
        _pia.Minigames.Disable();
        _pia.Drone.Disable();
        _pia.BaseControls.Disable();
        _pia.UI.Enable();

        _curMap = ControlMap.UI;
    }

    public void SetMinigameMode()
    {
        _pia.Player.Disable();
        _pia.BaseControls.Disable();
        _pia.Drone.Disable();
        _pia.Minigames.Enable();
        _pia.UI.Disable();

        _curMap = ControlMap.MINIGAME;
    }

    public void SetDroneMode()
    {
        _pia.Player.Disable();
        _pia.BaseControls.Enable();
        _pia.Drone.Enable();
        _pia.Minigames.Disable();
        _pia.UI.Disable();

        _curMap = ControlMap.DRONE;
    }



    public InputType GetCurrentInputDevice()
    {
        return _curDevice;
    }

    public ControlMap GetControlMap()
    {
        return _curMap;
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

    public void EmitCurDeviceNo3D()
    {
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

public enum ControlMap
{
    PLAYER,
    DRONE,
    UI,
    MINIGAME,
};
