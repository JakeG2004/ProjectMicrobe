// SaveSystem.cs
// A script for manging saving and loading with the game
// Author:  Jake Gendreau
// Date:    6/10/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Audio;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private AudioMixer _am;
    private SaveObject _currentState;
    private string _savePath;
    public static SaveSystem Instance { get; private set; }

    private SettingsSaveManager _settingsManager;
    private CosmeticsSaveManager _cosmeticsManager;
    private BackpackSaveManager _backpackManager;
    private ObjectivesSaveManager _objectivesManager;
    private RegionSaveManager _regionManager;

    // Singleton pattern
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        _currentState = new SaveObject();
        _savePath = System.IO.Path.Combine(Application.persistentDataPath, "save.ari");

        // Initialize managers
        _settingsManager = new SettingsSaveManager(_currentState, _am);
        _cosmeticsManager = new CosmeticsSaveManager(_currentState);
        _backpackManager = new BackpackSaveManager(_currentState);
        _objectivesManager = new ObjectivesSaveManager(_currentState);
        _regionManager = new RegionSaveManager(_currentState);
    }

    // ==========================
    // ===== SAVE FUNCTIONS =====
    // ==========================

    // Default save
    public void SaveState()
    {
        SaveTo(_savePath);
    }

    // Gets save data from the managers and writes it out to the specified path
    public void SaveTo(string path)
    {
        UpdateManagerReferences();

        // Get data from the managers
        _settingsManager.SaveSettings();
        _cosmeticsManager.SaveCosmetics();
        _cosmeticsManager.SaveCCValues();
        _backpackManager.SavePlayerBackpack();
        _objectivesManager.SaveObjectives();
        _regionManager.SaveRegions();

        WriteCurrentState(path);
    }

    public void WriteCurrentState()
    {
        WriteCurrentState(_savePath);
    }

    public void WriteCurrentState(string path)
    {
        string saveJson = JsonUtility.ToJson(_currentState);
        File.WriteAllText(path, saveJson);
    }

    // ===========================
    // ===== LOAD FUNCTIONS ======
    // ===========================

    public void LoadState()
    {
        LoadFrom(_savePath);
    }

    public void LoadFrom(string path)
    {
        // If the file exists, load it
        if (File.Exists(path))
        {
            string loadJson = File.ReadAllText(path);
            _currentState = JsonUtility.FromJson<SaveObject>(loadJson);

            UpdateManagerReferences();

            // Load data with the managers
            _settingsManager.LoadSettings();
            _cosmeticsManager.LoadCosmetics();
            _cosmeticsManager.LoadCCValues();
            _cosmeticsManager.SetCCUnlockableStates();
            _backpackManager.LoadPlayerBackpack();
            _objectivesManager.LoadObjectives();
            _regionManager.LoadRegions();

            // Set the graphics preset only if it doesnt match
            if (QualitySettings.GetQualityLevel() != _currentState.qualityLevel)
            {
                QualitySettings.SetQualityLevel(_currentState.qualityLevel, true);
            }

            return;
        }

        CreateNewSave();
    }

    // ======================================
    // ===== SAVE DELETION AND CREATION =====
    // ======================================

    public void DeleteSave()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            CreateNewSave();
        }
    }

    private void UpdateManagerReferences()
    {
        _backpackManager.UpdateState(_currentState);
        _cosmeticsManager.UpdateState(_currentState);
        _objectivesManager.UpdateState(_currentState);
        _regionManager.UpdateState(_currentState);
        _settingsManager.UpdateState(_currentState);
    }

    public void CreateNewSave()
    {
        _currentState = new SaveObject();
        string saveJson = JsonUtility.ToJson(_currentState);
        File.WriteAllText(_savePath, saveJson);
        LoadState();
    }

    // =====================
    // ===== DELEGATES =====
    // =====================

    public void UnlockCosmetic(string cosmeticName) => _cosmeticsManager.UnlockCosmetic(cosmeticName);
    public float GetLookSensitivity() => _settingsManager.GetLookSensitivity();
    public void SaveLookSensitivity(float val) => _settingsManager.SaveLookSensitivity(val);
    public void SaveVolume(Vector3 vol) => _settingsManager.SaveVolume(vol);
    public bool GetSprintToggle() => _settingsManager.GetSprintToggle();
    public bool GameIsComplete() => _currentState.hasCompletedGame;
    public void SetGameComplete() => _currentState.hasCompletedGame = true;
    public void SaveQualityLevel(int lvl) => _currentState.qualityLevel = lvl;
    public bool HasSave() => File.Exists(_savePath);
    public bool IsDefaultSave() => (_currentState.playerCosmetics.Count == 0);
    public void SaveName(string name) => _currentState.name = name;
}