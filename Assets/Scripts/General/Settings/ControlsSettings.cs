// ControlSettings.cs
// A script for managing the controls
// Author:  Jake Gendreau
// Date:    7/14/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsSettings : MonoBehaviour
{
    [SerializeField] private Slider _lookSlider;
    [SerializeField] private Toggle _sprintToggleButton;
    [SerializeField] private BoolGameEventListener _controlsListener;

    public void SetLookSensitivity(float val)
    {
        SaveSystem.Instance.SaveLookSensitivity(val);
        PlayerController.Instance?.SetLookSensitivity(val);
    }

    public void SetSprintToggle(bool state)
    {
        PlayerController.Instance.SetSprintToggle(state);
        SaveSystem.Instance.SaveSprintToggle(state);
    }

    void OnEnable()
    {
        SaveSystem ss = SaveSystem.Instance;
        if (ss != null)
        {
            _lookSlider.value = ss.GetLookSensitivity();
            _sprintToggleButton.isOn = ss.GetSprintToggle();
        }

        _controlsListener.OnEventRaised(NewInputController.Instance.GetCurrentInputDevice() == InputType.Controller);
    }
}
