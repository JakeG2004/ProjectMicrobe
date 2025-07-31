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

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        GeneralController.Instance.OnTimeDown += (() => HideWaitPrompt());

        PlayerInputHandler.Instance.OnDroneToggled += ProgressDronePrompt;
        DroneInputHandler.Instance.OnVerticalMovePressed += HideDronePrompt;
    }

    public void ShowControls()
    {
        ShowStep(0);

        PlayerInputHandler.Instance.OnJumpDown += (() => OnInputStep(3));
        GeneralController.Instance.OnLookPerformed += (() => OnInputStep(0));
        GeneralController.Instance.OnMovePerformed += (() => OnInputStep(1));
        GeneralController.Instance.OnZoomPerformed += (() => OnInputStep(2));
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
