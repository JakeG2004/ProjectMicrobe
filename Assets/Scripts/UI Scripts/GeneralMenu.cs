// GeneralMenu.cs
// A generalized menu class for other to inherit from
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralMenu : MonoBehaviour
{
    [SerializeField] protected GameObject _panel;

    protected bool _isActive = false;

    // Component references
    protected MenuControlsManager _mcm;
    protected AudioSource _as;
    protected SoundPlayer _sp;
    protected BoolGameEventTrigger _bget;

    protected virtual void Start()
    {
        _mcm = GetComponent<MenuControlsManager>();
        _sp = GetComponent<SoundPlayer>();
        _bget = GetComponent<BoolGameEventTrigger>();

        if (_panel != null)
        {
            _panel.SetActive(false);
        }
    }

    // Toggles the menu, publicly accessible
    public virtual void ToggleMenu()
    {
        // Toggle the Active State
        _isActive = !_isActive;

        // Set the 3d controls
        SetMouseVisibility(_isActive);

        // Set the panel
        if (_panel != null)
        {
            _panel?.SetActive(_isActive);    
        }
        

        // Handle the audio
        if (_isActive)
        {
            _sp.PlaySound(0);
            NewInputController.Instance.SetMenuMode();
        }

        else
        {
            _sp.PlaySound(1);
            NewInputController.Instance.Set3DMode();
        }

        // State tracker
        _bget?.TriggerEvent(_isActive);
    }

    void SetMouseVisibility(bool state)
    {
        _mcm?.SetMouseState(state);
    }
}
