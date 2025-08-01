using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverheadUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _controlIndicators;
    [SerializeField] private GameObject _droneActivateIndicator;
    [SerializeField] private GameObject _droneControlIndicators;
    [SerializeField] private GameObject _waitIndicator;
    [SerializeField] private GameObject _tabletIndicator;
    private PlayerInputActions _pia;
    private int _stepIndex = 0;
    private readonly Dictionary<string, System.Action> _inputStepHandlers = new();


    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        GeneralController.Instance.OnTimeDown += HideWaitPrompt;

        PlayerInputHandler.Instance.OnDroneToggled += ProgressDronePrompt;
        DroneInputHandler.Instance.OnVerticalMovePressed += HideDronePrompt;
    }

    // Uses a dictionary to store references to our lambda functions
    // so that we can safely subscribe and unsubscribe
    public void ShowControls()
    {
        ShowStep(0);

        _inputStepHandlers["Look"] = () => OnInputStep(0);
        _inputStepHandlers["Move"] = () => OnInputStep(1);
        _inputStepHandlers["Zoom"] = () => OnInputStep(2);
        _inputStepHandlers["Jump"] = () => OnInputStep(3);

        PlayerInputHandler.Instance.OnJumpDown += _inputStepHandlers["Jump"];
        GeneralController.Instance.OnLookPerformed += _inputStepHandlers["Look"];
        GeneralController.Instance.OnZoomPerformed += _inputStepHandlers["Zoom"];
        GeneralController.Instance.OnMovePerformed += _inputStepHandlers["Move"];
    }

    // Unsubscribes from the events
    void OnDisable()
    {
        if (_inputStepHandlers.TryGetValue("Jump", out var jump))
            PlayerInputHandler.Instance.OnJumpDown -= jump;

        if (_inputStepHandlers.TryGetValue("Look", out var look))
            GeneralController.Instance.OnLookPerformed -= look;

        if (_inputStepHandlers.TryGetValue("Move", out var move))
            GeneralController.Instance.OnMovePerformed -= move;

        if (_inputStepHandlers.TryGetValue("Zoom", out var zoom))
            GeneralController.Instance.OnZoomPerformed -= zoom;

        PlayerInputHandler.Instance.OnDroneToggled -= ProgressDronePrompt;
        DroneInputHandler.Instance.OnVerticalMovePressed -= HideDronePrompt;
        GeneralController.Instance.OnTimeDown -= HideWaitPrompt;
    }

    void ShowStep(int index)
    {
        if (_controlIndicators == null)
        {
            return;
        }

        for (int i = 0; i < _controlIndicators.Length; i++)
        {
            if (_controlIndicators[i] == null)
            {
                continue;
            }

            _controlIndicators[i].SetActive(i == index);
        }
    }

    void OnInputStep(int inputIndex)
    {
        if (inputIndex != _stepIndex)
            return; // Ignore out-of-order input

        _stepIndex++;

        if (_stepIndex < _controlIndicators.Length)
            ShowStep(_stepIndex);
        else
            HideAll(); // All done!
    }

    public void HideAll()
    {
        if (_controlIndicators == null)
        {
            return;
        }

        foreach (var indicator in _controlIndicators)
            indicator.SetActive(false);
    }

    public void ShowWaitPrompt()
    {
        _waitIndicator.SetActive(true);
    }

    public void HideWaitPrompt()
    {
        if (_waitIndicator == null)
        {
            return;
        }

        _waitIndicator.SetActive(false);
    }

    public void ShowTabletPrompt()
    {
        _tabletIndicator.SetActive(true);
    }

    public void ShowDronePrompt()
    {
        _droneActivateIndicator.SetActive(true);
    }

    public void ProgressDronePrompt()
    {

        if (!_droneActivateIndicator.activeSelf)
        {
            return;
        }

        _droneActivateIndicator.SetActive(false);
        _droneControlIndicators.SetActive(true);
    }

    public void HideDronePrompt()
    {
        _droneControlIndicators.SetActive(false);
    }

    public void HideTabletPrompt()
    {
        if (_tabletIndicator == null)
        {
            return;
        }

        _tabletIndicator.SetActive(false);
    }
}
