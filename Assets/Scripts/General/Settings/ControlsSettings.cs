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

    public void SetLookSensitivity(float val)
    {
        PlayerMovementController.Instance.SetLookSensitivity(val);
        SaveSystem.Instance.SaveLookSensitivity();
    }

    void OnEnable()
    {
        _lookSlider.value = SaveSystem.Instance.GetLookSensitivity();
    }
}
