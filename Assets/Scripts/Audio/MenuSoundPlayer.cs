// MenuSoundPlayer.cs
// A script for managing sounds in a menu
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _menuUp;
    [SerializeField] private AudioClip _menuDown;
    [SerializeField] private AudioClip _menuSound;

    private AudioSource _as;

    void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioType at)
    {
        switch(at)
        {
            case AudioType.MENU_OPEN:
                _as.PlayOneShot(_menuUp);
                break;
            case AudioType.MENU_CLOSED:
                _as.PlayOneShot(_menuDown);
                break;
            case AudioType.MENU_SOUND:
                _as.PlayOneShot(_menuSound);
                break;
            default:
                Debug.Log("Invalid case in MenuSoundPlayer");
                break;
        }
    }

    public void PlaySound(int at)
    {
        PlaySound((AudioType)at);
    }
}

[System.Serializable]
public enum AudioType
{
    MENU_OPEN,
    MENU_CLOSED,
    MENU_SOUND
};
