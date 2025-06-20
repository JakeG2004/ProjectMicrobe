// PuaseMenu.cs
// A script which manages the pause menu
// Author:  Jake Gendreau
// Date:    5/27/25 (SBR came out in English :)))) )

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : GeneralMenu
{
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
