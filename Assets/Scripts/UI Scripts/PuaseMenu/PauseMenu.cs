// PuaseMenu.cs
// A script which manages the pause menu
// Author:  Jake Gendreau
// Date:    5/27/25 (SBR came out in English :)))) )

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool _isActive = false;

    [SerializeField] private GameObject _menu;

    void Start()
    {
        _menu.SetActive(false);
    }

    public void TogglePause()
    {
        _isActive = !_isActive;


        _menu.SetActive(_isActive);

        // Freeze camera controls
        GetComponent<ShowHideMouse>().SetState(_isActive);
        GetComponent<ToggleCameraTracking>().SetCameraTracking(!_isActive);
    }

    public void ReloadLevel()
    {
        LevelLoader.Instance.ReloadLevel();
    }

    public void ExitLevel()
    {
        LevelLoader.Instance.LoadLevel("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        LevelLoader.Instance.LoadLevel(levelName);
    }
}
