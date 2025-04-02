using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance {get; private set;}

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Level {scene.name} has been loaded!");
        
        // Short delay to prevent race conditions
        StartCoroutine(DelayedLoadState());
    }

    private IEnumerator DelayedLoadState()
    {
        yield return null;
        SaveSystem.Instance.LoadState();
        Debug.Log("Save state loaded after delay.");
    }
}
