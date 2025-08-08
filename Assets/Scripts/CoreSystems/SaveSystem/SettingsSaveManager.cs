// SettingsSaveManager.cs
// A script for saving and loading settings
// Author:  Jake Gendreau
// Date:    7/25/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsSaveManager
{
    private SaveObject _currentState;
    private AudioMixer _audioMixer;

    // Sets the references that are needed
    public SettingsSaveManager(SaveObject saveObject, AudioMixer am)
    {
        _currentState = saveObject;
        _audioMixer = am;
    }

    public void UpdateState(SaveObject state)
    {
        _currentState = state;
    }

    // Saves the look sensitivity by pulling from the player controller
    public void SaveSettings()
    {
        PlayerController pc = PlayerController.Instance;

        if (pc == null)
        {
            return;
        }

        _currentState.controlsData.lookSensitivity = pc.GetLookSensitivity();
        _currentState.controlsData.sprintIsToggle = pc.GetSprintToggle();
    }

    // Saves the look sensitivity directly
    public void SaveLookSensitivity(float val)
    {
        _currentState.controlsData.lookSensitivity = val;
    }

    public void SaveSprintToggle(bool state)
    {
        _currentState.controlsData.sprintIsToggle = state;
    }

    // Gets the numerical value of the look sensitivity
    public float GetLookSensitivity()
    {
        return _currentState.controlsData.lookSensitivity;
    }

    public bool GetSprintToggle()
    {
        return _currentState.controlsData.sprintIsToggle;
    }

    // Loads the look sensitivity to the player movement controller
    public void LoadSettings()
    {
        PlayerController pc = PlayerController.Instance;

        if (pc == null)
        {
            return;
        }

        pc.SetLookSensitivity(_currentState.controlsData.lookSensitivity);
        pc.SetSprintToggle(_currentState.controlsData.sprintIsToggle);

        LoadVolume();
    }

    // Saves the volume given the vector3
    public void SaveVolume(Vector3 vol)
    {
        _currentState.volumeData.masterVolume = vol.x;
        _currentState.volumeData.musicVolume = vol.y;
        _currentState.volumeData.sfxVolume = vol.z;
    }

    // Loads the volume into the mixer from the save
    public void LoadVolume()
    {
        _audioMixer.SetFloat("MasterVolume", _currentState.volumeData.masterVolume);
        _audioMixer.SetFloat("MusicVolume", _currentState.volumeData.musicVolume);
        _audioMixer.SetFloat("SFXVolume", _currentState.volumeData.sfxVolume);
    }

    // Sets the quality level
    public void SaveQualityLevel(int lvl)
    {
        _currentState.qualityLevel = lvl;
    }
}
