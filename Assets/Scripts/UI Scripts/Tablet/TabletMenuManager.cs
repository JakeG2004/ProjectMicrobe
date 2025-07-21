using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletMenuManager : GeneralMenu
{
    [Header("Drone Entries")]
    [SerializeField] private GameObject _beachButton;
    [SerializeField] private GameObject _caveButton;
    [SerializeField] private GameObject _mountainButton;

    // Enables the buttons based on a string. Valid options are beach, cave, and mountain
    public void EnableDroneButton(string button)
    {
        switch (button)
        {
            case "beach":
                _beachButton.SetActive(true);
                break;

            case "cave":
                _caveButton.SetActive(true);
                break;

            case "mountain":
                _mountainButton.SetActive(true);
                break;
        }
    }
}
