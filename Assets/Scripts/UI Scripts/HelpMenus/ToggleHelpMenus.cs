// ToggleHelpMenu.cs
// A script for toggling the help menu in UI
// Author:  Jake Gendreau
// Date:    5/24/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleHelpMenus : MonoBehaviour
{
    [SerializeField] private GameObject[] _helpMenuContainers;
    [SerializeField] private TMP_Text _buttonText;
    private bool _helpIsOn = false;

    void Start()
    {
        _buttonText.text = "Show Help";
    }

    public void ToggleHelp()
    {
        _helpIsOn = !_helpIsOn;

        if(_helpIsOn)
        {
            ShowHelp();
        }

        else
        {
            HideHelp();
        }
    }

    public void ShowHelp()
    {
        _buttonText.text = "Hide Help";
        
        foreach(var container in _helpMenuContainers)
        {
            container.SetActive(true);
        }
    }

    public void HideHelp()
    {
        _buttonText.text = "Show Help";
        
        foreach(var container in _helpMenuContainers)
        {
            container.SetActive(false);
        }
    }
}
