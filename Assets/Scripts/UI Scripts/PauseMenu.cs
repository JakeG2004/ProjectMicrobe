using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    
    private KeyCode _pauseGame = KeyCode.Escape;
    private bool _isPaused = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(_pauseGame))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        if(_isPaused)
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0.0f;

            // Free cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        _pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;

        // Lock cursor to center of screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReloadLevel()
    {
        TogglePause();
        LevelLoader.Instance.ReloadLevel();
    }

    public void LoadLevel(string levelName)
    {
        TogglePause();
        LevelLoader.Instance.LoadLevel(levelName);
    }

    public void QuitGame()
    {
        TogglePause();
        LevelLoader.Instance.QuitGame();
    }
}
