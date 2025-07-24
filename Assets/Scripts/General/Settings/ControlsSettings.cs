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
    [SerializeField] private BoolGameEventListener _controlsListener;

    public void SetLookSensitivity(float val)
    {
        SaveSystem.Instance.SaveLookSensitivity(val);
        PlayerController.Instance?.SetLookSensitivity(val);
    }

    void OnEnable()
    {
        _lookSlider.value = SaveSystem.Instance.GetLookSensitivity();
        _controlsListener.OnEventRaised(NewInputController.Instance.GetCurrentInputDevice() == InputType.Controller);
    }
}
