// GetPylonMenuManager.cs
// A script for the menu where players will grab the pylon
// Author:  Jake Gendreau
// Date:    5/21/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetPylonMenuManager : GeneralMenu
{
    public static GetPylonMenuManager Instance { get; private set; }

    [Space(20)]
    [SerializeField] private Button _takePylonButton;
    [SerializeField] private BoolGameEventTrigger _menuStateTracker;

    private CarriedPylon _cp;

    protected override void Start()
    {
        base.Start();

        // Handle singleton
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }

        // Get the carried pylon
        _cp = Object.FindObjectOfType<CarriedPylon>();

        if(!_cp)
        {
            Debug.Log("Failed to get carried microbes");
        }
    }

    public override void ToggleMenu()
    {
        base.ToggleMenu();

        _menuStateTracker.TriggerEvent(_isActive);
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        _takePylonButton.interactable = !_cp.HasPylon();
        if (!_cp.HasPylon())
        {
            _takePylonButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Take Pylon";
            GetComponent<MenuNavChanger>().UpdateNavObjects();
        }

        else
        {
            _takePylonButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Pylon Already in Inventory";
        }
    }

    public void GivePylon()
    {
        _cp.SetHasPylon(true);
    }
}
