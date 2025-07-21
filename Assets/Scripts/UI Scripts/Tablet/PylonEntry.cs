// PylonEntry.cs
// A script which is responsible for populating the pylon entries in the tablet
// Author:  Jake Gendreau
// Date:    7/21/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PylonEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _pylonName;
    [SerializeField] private CustomSlider _slider;

    public void SetVals(string pylonName, float envHealth)
    {
        _pylonName.text = pylonName;
        _slider.SetSliderFill(envHealth);
    }
}
