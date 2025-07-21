using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverheadUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _controlIndicators;
    [SerializeField] private GameObject _waitIndicator;
    [SerializeField] private GameObject _tabletIndicator;
    private PlayerInputActions _pia;
    private int _stepIndex = 0;

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        _pia.Player.Time.started += ctx => HideWaitPrompt();
    }

    public void ShowControls()
    {
        ShowStep(0);

        _pia.Player.Look.performed += ctx => OnInputStep(0);
        _pia.Player.Movement.performed += ctx => OnInputStep(1);
        _pia.Player.Zoom.performed += ctx => OnInputStep(2);
        _pia.Player.Jump.started += ctx => OnInputStep(3);
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

    public void HideTabletPrompt()
    {
        if (_tabletIndicator == null)
        {
            return;
        }

        _tabletIndicator.SetActive(false);
    }
}
