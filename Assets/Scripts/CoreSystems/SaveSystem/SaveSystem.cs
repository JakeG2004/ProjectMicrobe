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

        _savePath = System.IO.Path.Combine(Application.persistentDataPath, "save.ari");
    }

    void Start()
    {
        // Debug.Log("Save Path: " + _savePath);
        _currentState = new SaveObject();
    }

    // Handle input keys for quick save / load
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadState();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveState();
        }
    }


    // ==========================
    // ===== MAIN FUNCTIONS =====
    // ==========================


    // Saves the current state of the game, optionally getting the player
    public void SaveState()
    {
        SaveTo(_savePath);
    }

    public void SaveTo(string path)
    {
        // Get the cosmetics off the player if the player exists
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            _currentState.playerCosmetics = SaveCosmetics();
        }

        // Perform the save functions
        SaveRegions();
        SaveObjectives();
        SavePlayerBackpack();
        SaveCCValues();

        // Write it to a file and announce it
        string saveJson = JsonUtility.ToJson(_currentState);
        File.WriteAllText(path, saveJson);
        Debug.Log("Saved State");
    }

    // Loads the state from the save
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

            // Load the cosmetics if the player is there
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                LoadCosmetics();
            }

            // Load the other things
            LoadVolume();
            LoadRegions();
            LoadObjectives();
            LoadPlayerBackpack();
            LoadCCValues();
            SetCCUnlockableStates();

            // Alert the player
            Debug.Log("SAVE SYSTEM: Loaded state");
            return;
        }

        Debug.Log("SAVE SYSTEM: Failed to find file");
        CreateNewSave();
    }

    private void CreateNewSave()
    {
        _currentState = new SaveObject();

        // Write it to a file and announce it
        string saveJson = JsonUtility.ToJson(_currentState);
        File.WriteAllText(_savePath, saveJson);
        Debug.Log("SAVE SYSTEM: Created new save");
        LoadState();
    }



    // =============================
    // ===== ONE-OFF FUNCTIONS =====
    // =============================


    // Set the name of the save
    public void SaveName(string name)
    {
        _currentState.name = name;
    }

    // Save the volume
    public void SaveVolume(Vector3 vol)
    {
        _currentState.volumeData.masterVolume = vol.x;
        _currentState.volumeData.musicVolume = vol.y;
        _currentState.volumeData.sfxVolume = vol.z;
    }

    // Loads the volume
    public void LoadVolume()
    {
        _am.SetFloat("MasterVolume", _currentState.volumeData.masterVolume);
        _am.SetFloat("MusicVolume", _currentState.volumeData.musicVolume);
        _am.SetFloat("SFXVolume", _currentState.volumeData.sfxVolume);
    }


    // ================================
    // ===== UNLOCKABLE COSMETICS =====
    // ================================


    public void UnlockCosmetic(string cosmeticName)
    {
        // Prevent duplicates
        if (_currentState.unlockedCosmetics.Contains(cosmeticName))
        {
            return;
        }

        // Add the new entry to the list
        _currentState.unlockedCosmetics.Add(cosmeticName);
    }

    public void SetCCUnlockableStates()
    {
        ToggleCosmeticLocker[] tcls = Object.FindObjectsOfType<ToggleCosmeticLocker>();

        // Early return if no lockers found
        if (tcls.Length == 0)
        {
            return;
        }

        foreach (string cosmetic in _currentState.unlockedCosmetics)
        {
            foreach (ToggleCosmeticLocker tcl in tcls)
            {
                string cosmeticName = tcl.GetCosmeticName();

                if (cosmeticName == cosmetic)
                {
                    tcl.SetLockState(false);
                }
            }
        }
    }


    // =====================================
    // ===== PLAYER BACKPACK FUNCTIONS =====
    // =====================================


    public void SavePlayerBackpack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        CarriedPylon cp = player.GetComponent<CarriedPylon>();
        CarriedMicrobes cm = player.GetComponent<CarriedMicrobes>();

        // Early return for no player
        if (player == null || cp == null || cm == null)
        {
            return;
        }

        // Pick up whether the player has a pylon
        _currentState.backpack.hasPylon = cp.HasPylon();

        // Pick up the microbes from the player
        _currentState.backpack.carriedMicrobes = cm.GetMicrobes();
    }

    public void LoadPlayerBackpack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        CarriedMicrobes cm = player.GetComponent<CarriedMicrobes>();
        CarriedPylon cp = player.GetComponent<CarriedPylon>();

        // Early return if references not found
        if (cm == null || cp == null)
        {
            return;
        }

        cp.SetHasPylon(_currentState.backpack.hasPylon);
        foreach (StringFloatPair microbe in _currentState.backpack.carriedMicrobes)
        {
            cm.AddMicrobe(microbe);
        }
    }


    // ===============================
    // ===== OBJECTIVE FUNCTIONS =====
    // ===============================


    // Saves the objectives sorted into objective groups
    public void SaveObjectives()
    {
        // Get all of the objective groups
        ObjectiveGroup[] objGroups = Object.FindObjectsOfType<ObjectiveGroup>();

        // Iterate through every objective group
        foreach (ObjectiveGroup objGroup in objGroups)
        {
            bool foundGroup = false;

            // Try to find a matching existing entry, then update it
            foreach (ObjectiveGroupItem ogi in _currentState.objectives)
            {
                if (ogi.name == objGroup.GetName())
                {
                    foundGroup = true;

                    ogi.completeObjectives = objGroup.GetCompletedObjectives();
                    ogi.currentObjective = objGroup.GetCurrentObjective();
                    ogi.complete = objGroup.IsComplete();
                    break;
                }
            }

            if (foundGroup)
            {
                continue;
            }

            // Create a new entry and add it
            ObjectiveGroupItem newOGI = new();
            newOGI.name = objGroup.GetName();
            newOGI.completeObjectives = objGroup.GetCompletedObjectives();
            newOGI.currentObjective = objGroup.GetCurrentObjective();
            newOGI.complete = objGroup.IsComplete();

            _currentState.objectives.Add(newOGI);
        }
    }

    // Load the objectives based on their objective groups
    void LoadObjectives()
    {
        // Get all of the objective groups
        ObjectiveGroup[] objGroups = Object.FindObjectsOfType<ObjectiveGroup>();

        foreach (ObjectiveGroupItem ogi in _currentState.objectives)
        {
            foreach (ObjectiveGroup objGroup in objGroups)
            {
                if (objGroup.GetName() == ogi.name)
                {
                    objGroup.CompleteObjectives(ogi);
                }
            }
        }
    }


    // ============================
    // ===== REGION FUNCTIONS =====
    // ============================


    // Saves the regions
    public void SaveRegions()
    {
        // Get each pylon
        MicrobePopSim[] sims = Object.FindObjectsOfType<MicrobePopSim>();

        // Check for empty, early return
        if (sims.Length == 0)
        {
            return;
        }

        // Get each of them, adding to the list
        foreach (MicrobePopSim sim in sims)
        {
            string envName = sim.GetEnvSO().envName;

            bool foundRegion = false;
            // Entry already exists
            foreach (RegionData region in _currentState.regionData)
            {
                if (region.regionName == envName)
                {
                    GetDataFromRegion(region, sim);
                    foundRegion = true;
                    break;
                }
            }

            // Prevent repeat entries
            if (foundRegion)
            {
                continue;
            }

            // Add new entry
            RegionData newRegion = new();
            newRegion.regionName = envName;

            GetDataFromRegion(newRegion, sim);

            // Add it to the list
            _currentState.regionData.Add(newRegion);
        }
    }

    // Helper function to get data from the regions
    private void GetDataFromRegion(RegionData region, MicrobePopSim sim)
    {
        // Copy the transform
        region.pylonPosition = sim.gameObject.transform.position;
        region.pylonRotation = sim.gameObject.transform.rotation;
        region.isActive = sim.gameObject.activeInHierarchy;

        // Copy the current microbes
        foreach (Microbe microbe in sim.GetMicrobes())
        {
            string name = microbe.microbeName;
            float population = microbe.population;

            bool foundMicrobe = false;
            foreach (StringFloatPair data in region.microbes)
            {
                if (data.name == name)
                {
                    data.amount = population;
                    foundMicrobe = true;
                    break;
                }
            }

            if (foundMicrobe)
            {
                continue;
            }

            // Create a new microbeSOPopPair to store the current populations
            StringFloatPair mnpp = new();
            mnpp.name = name;
            mnpp.amount = population;

            region.microbes.Add(mnpp);
        }

        // Copy the current environment
        foreach (var res in sim.GetEnv().resources)
        {
            // Get the values from the dictionary
            string name = res.Key;
            float amount = res.Value;

            bool foundResource = false;

            // Overwrite existing entry if exists
            foreach (StringFloatPair resourceEntry in region.resources)
            {
                if (resourceEntry.name == name)
                {
                    foundResource = true;
                    resourceEntry.amount = amount;
                    break;
                }
            }

            // Go to next resource if resource was found
            if (foundResource)
            {
                continue;
            }

            // Create a new StringFloatPair with the data
            StringFloatPair sfp = new();
            sfp.name = name;
            sfp.amount = amount;

            region.resources.Add(sfp);
        }

        // Get the health history
        region.healthHistory = sim.gameObject.GetComponent<PylonStatusEventsChecker>().GetHealthHist();
        region.mycorrhisArray = sim.GetMycorrhisArray();
    }

    // Loads the data from the regions
    public void LoadRegions()
    {
        PylonRegion[] regions = Object.FindObjectsOfType<PylonRegion>();

        // Early return if no regions
        if (regions.Length == 0)
        {
            return;
        }

        foreach (RegionData region in _currentState.regionData)
        {
            // Get the region name
            string regionName = region.regionName;

            // Find the region which corresponds to the pylon
            foreach (PylonRegion pylonRegion in regions)
            {
                if (pylonRegion.GetEnvSO().envName == regionName)
                {
                    // Instance a new pylon
                    GameObject newPylon = Object.Instantiate(pylonRegion.GetPylonPrefab());

                    // Set the region to correspond to the newPylon
                    pylonRegion.SetRegionPylon(newPylon);

                    // Set the new pylon transform
                    newPylon.transform.position = region.pylonPosition;
                    newPylon.transform.rotation = region.pylonRotation;

                    // Set the new pylon information
                    MicrobePopSim sim = newPylon.GetComponent<MicrobePopSim>();
                    sim.SetEnv(pylonRegion.GetEnvSO());

                    // Set the region for the PSED
                    newPylon.GetComponent<PylonStatusEventsChecker>().SetRegion(pylonRegion);

                    // Set microbes
                    foreach (StringFloatPair mnpp in region.microbes)
                    {
                        sim.QueueMicrobePop(mnpp);
                    }

                    // Set resources
                    foreach (StringFloatPair sfp in region.resources)
                    {
                        sim.QueueResourceAmount(sfp);
                    }

                    // Set health hist
                    sim.gameObject.GetComponent<PylonStatusEventsChecker>().SetHealthHist(region.healthHistory);
                    sim.SetMycorrhisArray(region.mycorrhisArray);

                    // Set the on/off state
                    pylonRegion.gameObject.SetActive(region.isActive);
                }
            }
        }
    }


    // ==============================
    // ===== COSMETIC FUNCTIONS =====
    // ==============================


    // Save the cosmetics
    private List<CosmeticEntry> SaveCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (!player)
        {
            return null;
        }

        List<CosmeticEntry> cosmetics = new List<CosmeticEntry>();

        // Iterate through each of the cosmetics
        int numChildren = player.transform.childCount;

        // Start at 1 to skip the skeleton
        for (int i = 1; i < numChildren; i++)
        {
            // get the child
            GameObject child = player.transform.GetChild(i).gameObject;

            // get the renderer of the child
            Renderer renderer = child.GetComponent<Renderer>();

            // skip if no renderer
            if (!renderer)
            {
                continue;
            }

            // create material data list
            List<MaterialData> mats = new List<MaterialData>();

            // save each material
            foreach (var mat in renderer.materials)
            {
                // skip if it doesnt have the right fields
                if (!mat.HasColor("_TintR") || !mat.HasColor("_TintG") || !mat.HasColor("_TintB"))
                {
                    continue;
                }

                // Create dat_currentState.playerCosmetics = cosmetics; structure
                MaterialData data = new MaterialData();

                // Populate
                data.tintR = mat.GetColor("_TintR");
                data.tintG = mat.GetColor("_TintG");
                data.tintB = mat.GetColor("_TintB");

                // Handle skin
                if (mat.name.Contains("m_Ari_Skin"))
                {
                    data.tintB = mat.GetColor("_SSS");
                }

                mats.Add(data);
            }

            // Create new cosmetic entry and add it to the list
            cosmetics.Add(new CosmeticEntry
            {
                name = child.name,
                enabled = child.activeSelf,
                materials = mats
            });
        }

        return cosmetics;
    }

    // Load cosmetics
    private void LoadCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (!player)
        {
            return;
        }

        int numChildren = player.transform.childCount;

        // Set the cosmetic status for each child
        for (int i = 1; i < numChildren; i++)
        {
            GameObject child = player.transform.GetChild(i).gameObject;
            Renderer renderer = child.GetComponent<Renderer>();

            // Look at the cosmetic entry list
            foreach (CosmeticEntry cosmetic in _currentState.playerCosmetics)
            {
                // Skip non-included entries
                if (cosmetic.name != child.name)
                {
                    continue;
                }

                child.SetActive(cosmetic.enabled);

                // Set the material colors how they should be
                int materialCount = Mathf.Min(renderer.materials.Length, cosmetic.materials.Count);

                for (int j = 0; j < materialCount; j++)
                {
                    MaterialData data = cosmetic.materials[j];

                    renderer.materials[j].SetColor("_TintR", data.tintR);
                    renderer.materials[j].SetColor("_TintG", data.tintG);
                    renderer.materials[j].SetColor("_TintB", data.tintB);

                    if (renderer.materials[j].name.Contains("m_Ari_Skin"))
                    {
                        renderer.materials[j].SetColor("_TintB", Color.white);
                        renderer.materials[j].SetColor("_SSS", data.tintB);
                    }
                }
            }
        }
    }


    // ===========================
    // ===== CC INDEX VALUES =====
    // ===========================

    public void SaveCCValues()
    {
        // Get all of the CCIndexManagers
        CCIndexManager[] ccim = Object.FindObjectsOfType<CCIndexManager>();

        // Early return if no entries
        if (ccim.Length == 0)
        {
            return;
        }

        // Save each element of the class
        foreach (CCIndexManager ccMgr in ccim)
        {
            switch (ccMgr.GetValType())
            {
                case "HairSlider":
                    _currentState.ccVals.hairIndex = ccMgr.GetSliderValue();
                    break;

                case "HairPrimary":
                    _currentState.ccVals.hairPrimary = ccMgr.GetToggleGroupValue();
                    break;

                case "HairSecondary":
                    _currentState.ccVals.hairSecondary = ccMgr.GetToggleGroupValue();
                    break;

                case "EyeColor":
                    _currentState.ccVals.eyes = ccMgr.GetToggleGroupValue();
                    break;

                case "SkinColor":
                    _currentState.ccVals.skin = ccMgr.GetToggleGroupValue();
                    break;

                case "GlassesColor":
                    _currentState.ccVals.glasses = ccMgr.GetToggleGroupValue();
                    break;

                case "UpperBody":
                    _currentState.ccVals.upperBody = ccMgr.GetToggleGroupValue();
                    break;

                case "LowerBody":
                    _currentState.ccVals.lowerBody = ccMgr.GetToggleGroupValue();
                    break;

                case "Hat":
                    _currentState.ccVals.hat = ccMgr.GetToggleGroupValue();
                    break;

                default:
                    Debug.LogWarning("Invalid case in SaveCCValues");
                    break;
            }
        }
    }

    public void LoadCCValues()
    {
        // Get all of the CCIndexManagers
        CCIndexManager[] ccim = Object.FindObjectsOfType<CCIndexManager>();

        // Early return if no entries
        if (ccim.Length == 0)
        {
            return;
        }

        // Load each element of the class
        foreach (CCIndexManager ccMgr in ccim)
        {
            switch (ccMgr.GetValType())
            {
                case "HairSlider":
                    // If a hat is selected
                    if (_currentState.ccVals.hat != 0)
                    {
                        // Sets the slider value without invoking the callback method
                        ccMgr.SetSliderValueNoNotify(_currentState.ccVals.hairIndex);

                        // Call its onload event
                        ccMgr.OnLoad(_currentState.ccVals.hairIndex);

                        // Get the cosmetic controller and set its value
                        CosmeticController cc = ccMgr.gameObject.GetComponent<CosmeticController>();
                        cc.SetVal(_currentState.ccVals.hairIndex);
                        break;
                    }

                    // No hat selected, continue as normal
                    else
                    {
                        ccMgr.SetSliderValueWithNotify(_currentState.ccVals.hairIndex);
                        break;
                    }

                case "HairPrimary":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.hairPrimary);
                    break;

                case "HairSecondary":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.hairSecondary);
                    break;

                case "EyeColor":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.eyes);
                    break;

                case "SkinColor":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.skin);
                    break;

                case "GlassesColor":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.glasses);
                    break;

                case "UpperBody":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.upperBody);
                    break;

                case "LowerBody":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.lowerBody);
                    break;

                case "Hat":
                    ccMgr.SetToggleGroupValue(_currentState.ccVals.hat);
                    break;

                default:
                    Debug.LogWarning("Invalid case in SaveCCValues");
                    break;
            }
        }
    }

    public bool HasSave()
    {
        return File.Exists(_savePath);
    }

    public bool IsDefaultSave()
    {
        return (_currentState.playerCosmetics.Count == 0);
    }

    public void DeleteSave()
    {
        if (File.Exists(_savePath))
        {
            // Delete from disk
            Debug.Log("SAVE SYSTEM: Deleting save");
            File.Delete(_savePath);

            // Delete from memory
            CreateNewSave();
        }
    }
}