using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance {get; private set;}
    public event System.Action OnSceneUnload;
    public event System.Action OnSceneLoad;
    [SerializeField] private GameObject _loadingScreen;
    private CanvasGroup _cg;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;


        _cg = _loadingScreen.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _loadingScreen.SetActive(false);
        _cg.alpha = 0;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedLoadState());
    }

    private IEnumerator DelayedLoadState()
    {
        yield return null;
        SaveSystem.Instance.LoadState();
        NewInputController.Instance?.EmitCurDevice();
    }

    public void LoadLevel(string levelName)
    {
        SaveSystem.Instance.SaveState();
        StartCoroutine(StartLoad(levelName));

    }

    // Writes the current state of the save file as well to save things like volume, controls, etc.
    public void LoadLevelNoSave(string levelName)
    {
        SaveSystem.Instance.WriteCurrentState();
        StartCoroutine(StartLoad(levelName));
    }

    private IEnumerator StartLoad(string levelName)
    {
        _loadingScreen.SetActive(true);
        yield return null; // Ensure UI has a frame to render

        OnSceneUnload?.Invoke();

        yield return StartCoroutine(FadeLoadingScreen(1f, 0.5f));

        AsyncOperation op = SceneManager.LoadSceneAsync(levelName);
        op.allowSceneActivation = false; // Wait until fade is complete

        // Optionally, wait for scene to be 90% loaded
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // Now that loading screen is opaque, activate the scene
        op.allowSceneActivation = true;

        // Wait until scene is fully activated
        while (!op.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(0f, 0.5f));
        _loadingScreen.SetActive(false);
        OnSceneLoad?.Invoke();
    }


    private IEnumerator FadeLoadingScreen(float targetValue, float duration)
    {
        float startVal = _cg.alpha;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _cg.alpha = Mathf.Lerp(startVal, targetValue, elapsedTime / duration);
            yield return null;
        }

        _cg.alpha = targetValue;
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
