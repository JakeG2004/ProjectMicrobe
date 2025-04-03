using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelScript : MonoBehaviour
{
    private enum LoadType
    {
        Save,
        NoSave
    }

    [SerializeField] private LoadType _loadType = LoadType.Save;

    public void LoadLevel(string levelName)
    {
        if(_loadType == LoadType.Save)
        {
            LevelLoader.Instance.LoadLevel(levelName);
            return;
        }

        LevelLoader.Instance.LoadLevelNoSave(levelName);
    }
}
