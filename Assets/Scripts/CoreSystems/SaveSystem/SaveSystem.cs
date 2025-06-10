using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Audio;

public class SaveSystem : MonoBehaviour
{
    private string _savePath;

    public static SaveSystem Instance { get; private set; }

    private SaveObject _currentState;

    [SerializeField] private AudioMixer _am;

    // Singleton pattern
    void Awake()
    {
        if(Instance == null)
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
        Debug.Log("Save Path: " + _savePath);
        _currentState = new SaveObject();
    }

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

    public void SaveState()
    {
        if(GameObject.FindGameObjectWithTag("Player"))
        {
            _currentState.playerCosmetics = GetCosmetics();
        }

        GetRegions();

        string saveJson = JsonUtility.ToJson(_currentState);
        File.WriteAllText(_savePath, saveJson);
        Debug.Log("Saved State");
    }

    public void LoadState()
    {
        // If the file exists, load it
        if(File.Exists(_savePath))
        {
            string loadJson = File.ReadAllText(_savePath);
            _currentState = JsonUtility.FromJson<SaveObject>(loadJson);
            
            if(GameObject.FindGameObjectWithTag("Player"))
            {
                SetCosmetics();
            }

            LoadVolume();
            LoadRegions();

            Debug.Log("Loaded state");
            return;
        }

        Debug.Log("Failed to find file");
    }

    private void SetCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(!player)
        {
            Debug.Log("Failed to find player.");
            return;
        }

        int numChildren = player.transform.childCount;

        // Set the cosmetic status for each child
        for(int i = 1; i < numChildren; i++)
        {
            GameObject child = player.transform.GetChild(i).gameObject;
            Renderer renderer = child.GetComponent<Renderer>();

            // Look at the cosmetic entry list
            foreach(CosmeticEntry cosmetic in _currentState.playerCosmetics)
            {
                // Skip non-included entries
                if(cosmetic.name != child.name)
                {
                    continue;
                }

                child.SetActive(cosmetic.enabled);

                // Set the material colors how they should be
                int materialCount = Mathf.Min(renderer.materials.Length, cosmetic.materials.Count);

                for(int j = 0; j < materialCount; j++)
                {
                    MaterialData data = cosmetic.materials[j];

                    renderer.materials[j].SetColor("_TintR", data.tintR);
                    renderer.materials[j].SetColor("_TintG", data.tintG);
                    renderer.materials[j].SetColor("_TintB", data.tintB);
                }
            }
        }
    }

    public void SetName(string name)
    {
        _currentState.name = name;
    }

    // Vector3 is master, music, sfx
    public void SetVolume(Vector3 vol)
    {
        _currentState.volumeData.masterVolume = vol.x;
        _currentState.volumeData.musicVolume = vol.y;
        _currentState.volumeData.sfxVolume = vol.z;
    }

    public void LoadVolume()
    {
        _am.SetFloat("MasterVolume", _currentState.volumeData.masterVolume);
        _am.SetFloat("MusicVolume", _currentState.volumeData.musicVolume);
        _am.SetFloat("SFXVolume", _currentState.volumeData.sfxVolume);
    }

    public void GetRegions()
    {
        // Get each pylon
        MicrobePopSim[] sims = Object.FindObjectsOfType<MicrobePopSim>();

        // Check for empty, early return
        if (sims.Length == 0)
        {
            Debug.Log("No sims!");
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

    private void GetDataFromRegion(RegionData region, MicrobePopSim sim)
    {
        // Copy the transform
        region.pylonPosition = sim.gameObject.transform.position;
        region.pylonRotation = sim.gameObject.transform.rotation;

        // Copy the current microbes
        foreach (Microbe microbe in sim.GetMicrobes())
        {
            string name = microbe.microbeName;
            float population = microbe.population;

            bool foundMicrobe = false;
            foreach (MicrobeNamePopPair data in region.microbes)
            {
                if (data.name == name)
                {
                    data.pop = population;
                    foundMicrobe = true;
                    break;
                }
            }

            if (foundMicrobe)
            {
                continue;
            }

            // Create a new microbeSOPopPair to store the current populations
            MicrobeNamePopPair mnpp = new();
            mnpp.name = name;
            mnpp.pop = population;

            region.microbes.Add(mnpp);
        }
    }

    private void GetMicrobesFromRegion(RegionData region)
    {
        
    }

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
                    newPylon.GetComponent<PylonStatusEventsChecker>().SetRegion(pylonRegion);

                    // Set microbes
                    foreach (MicrobeNamePopPair mnpp in region.microbes)
                    {
                        sim.QueueMicrobePop(mnpp);
                    }
                }
            }
        }
    }

    private List<CosmeticEntry> GetCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (!player)
        {
            Debug.Log("Failed to find player.");
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

        Debug.Log("Got Cosmetics");
        return cosmetics;
    }
}
