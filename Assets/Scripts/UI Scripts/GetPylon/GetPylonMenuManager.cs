// GetPylonMenuManager.cs
// A script for the menu where players will grab the pylon
// Author:  Jake Gendreau
// Date:    5/21/25

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
    private CarriedMicrobes _cm;

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

        _cm = Object.FindObjectOfType<CarriedMicrobes>();

        if(!_cm)
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
        _takePylonButton.interactable = !_cm.HasPylon();
        if(!_cm.HasPylon())
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
        _cm.SetHasPylon(true);
    }
}
