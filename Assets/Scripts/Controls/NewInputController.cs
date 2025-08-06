using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewInputController : MonoBehaviour
{
    public static NewInputController Instance { get; private set; }
    [SerializeField] private PlayerStatesSO _states;
    [SerializeField] private UnityEvent _onMouseKeyboard;
    [SerializeField] private UnityEvent _onGamepad;

    // Control type references
    private PlayerInputActions _pia;
    private InputType _curDevice = InputType.Unknown;
    [SerializeField] private ControlMap _curMap = ControlMap.PLAYER;

    // Reference scripts
    public PlayerInputHandler playerInput;
    public DroneInputHandler droneInput;
    public GeneralInputController generalInput;
    public MinigameInputHandler minigameInput;
    public UIInputHandler uiInput;
    private MenuControlsManager _mcm;
    private Stack<ControlMap> _prevMaps = new();

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

        if (_pia == null)
        {
            _pia = new();
        }

        // Hook into relevant actions to detect input device
        foreach (var map in _pia.asset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                action.performed += UpdateInputDevice;
            }
        }

        GetComponentReferences();
    }

    public PlayerInputActions GetPlayerInputActions()
    {
        return _pia;
    }

    public void Set3DMode()
    {
        _mcm.SetMouseState(false);

        _pia.Player.Enable();
        _pia.Minigames.Disable();
        _pia.Drone.Disable();
        _pia.BaseControls.Enable();
        _pia.UI.Disable();

        _curMap = ControlMap.PLAYER;
    }

    public void SetMenuMode()
    {
        _mcm.SetMouseState(true);

        _pia.Player.Disable();
        _pia.Minigames.Disable();
        _pia.Drone.Disable();
        _pia.BaseControls.Disable();
        _pia.UI.Enable();

        // Prevent menu controls from stacking and prevent access from returning to the player
        if (_curMap != ControlMap.UI)
        {
            _prevMaps.Push(_curMap);
        }
        
        _curMap = ControlMap.UI;
    }

    public void ExitMenuMode()
    {
        switch (_prevMaps.Pop())
        {
            case ControlMap.PLAYER:
                Set3DMode();

                break;

            case ControlMap.DRONE:
                SetDroneMode();
                break;

            case ControlMap.UI:
                break;
        }
    }

    public void SetMinigameMode()
    {
        _mcm.SetMouseState(true);

        _pia.Player.Disable();
        _pia.BaseControls.Disable();
        _pia.Drone.Disable();
        _pia.Minigames.Enable();
        _pia.UI.Disable();

        _curMap = ControlMap.MINIGAME;
    }

    public void SetDroneMode()
    {
        _mcm.SetMouseState(false);

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

    private void GetComponentReferences()
    {
        generalInput = new GeneralInputController(_states, _pia);
        playerInput = new PlayerInputHandler(_states, _pia);
        droneInput = new DroneInputHandler(_states, _pia);
        minigameInput = new MinigameInputHandler(_states, _pia);
        uiInput = new UIInputHandler(_states, _pia);

        _mcm = GetComponent<MenuControlsManager>();
    }

    public void EmitCurDevice()
    {
        //Set3DMode();
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

    public void UnlockDrone()
    {
        playerInput.UnlockDrone();
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
