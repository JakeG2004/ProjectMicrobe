// GetPylonMenuManager.cs
// A script for the menu where players will grab the pylon
// Author:  Jake Gendreau
// Date:    5/21/25

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetPylonMenuManager : MonoBehaviour
{
    public static GetPylonMenuManager Instance { get; private set; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _takePylonButton;
    [SerializeField] private BoolGameEventTrigger _menuStateTracker;

    private bool _isActive = false;
    private CarriedPylon _cp;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else if(Instance != this)
        {
            Destroy(this);
        }

        _cp = Object.FindObjectOfType<CarriedPylon>();

        if(!_cp)
        {
            Debug.Log("Failed to get carried microbes");
        }

        _panel.SetActive(false);
    }

    public void ToggleMenu()
    {
        // Toggle active state
        _isActive = !_isActive;

        // Set the panel active
        _panel.SetActive(_isActive);

        // Set UI Control state
        GetComponent<ToggleCameraTracking>()?.SetCameraTracking(!_isActive);
        MovementController.instance.SetMovementState(!_isActive);
        GetComponent<ShowHideMouse>()?.SetState(_isActive);

        // Set UI Menu Tracker State
        _menuStateTracker.TriggerEvent(_isActive);
        UpdateButtonText();

        
    }

    public void UpdateButtonText()
    {
        _takePylonButton.interactable = !_cp.HasPylon();
        if(!_cp.HasPylon())
        {
            _takePylonButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Take Pylon";
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
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetPylonMenuManager : GeneralMenu
{
    public static GetPylonMenuManager Instance { get; private set; }

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
        if(!_cp.HasPylon())
        {
            _takePylonButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Take Pylon";
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
