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
    [SerializeField] protected string _menuName;
    [SerializeField] protected GameObject _panel;

    protected bool _isActive = false;

    // Component references
    protected ToggleCameraTracking _tct;
    protected ShowHideMouse _shm;
    protected MovementController _mc;
    protected AudioSource _as;
    protected MenuSoundPlayer _msp;

    protected virtual void Start()
    {
        _tct = GetComponent<ToggleCameraTracking>();
        _shm = GetComponent<ShowHideMouse>();
        _mc = MovementController.instance;

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
        if(_isActive)
        {
            _msp.PlaySound(AudioType.MENU_OPEN);
        }

        else
        {
            _msp.PlaySound(AudioType.MENU_CLOSED);
        }
    }

    void Set3DControls(bool state)
    {
        _tct.SetCameraTracking(state);
        _mc.SetMovementState(state);
        _shm.SetState(!state);
    }
}
