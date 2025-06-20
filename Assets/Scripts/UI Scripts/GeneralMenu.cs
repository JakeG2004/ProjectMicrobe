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

        _panel.SetActive(false);
    }

    // Toggles the menu, publicly accessible
    public virtual void ToggleMenu()
    {
        // Toggle the Active State
        _isActive = !_isActive;

        // Set the 3d controls
        Set3DControls(!_isActive);

        // Set the panel
        _panel.SetActive(_isActive);

        // Handle the audio
        if (_isActive)
        {
            _sp.PlaySound(0);
        }

        else
        {
            _sp.PlaySound(1);
        }

        // State tracker
        _bget?.TriggerEvent(_isActive);
    }

    void Set3DControls(bool state)
    {
        _mcm?.SetControlState(!state);
    }
}
