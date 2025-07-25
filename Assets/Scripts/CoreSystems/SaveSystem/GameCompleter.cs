// GameCompleter.cs
// A script for setting the game to be complete. Callable from other scripts
// Author:  Jake Gendreau
// Date:    7/25/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCompleter : MonoBehaviour
{
    public void SetGameComplete()
    {
        SaveSystem.Instance.SetGameComplete();
    }
}
