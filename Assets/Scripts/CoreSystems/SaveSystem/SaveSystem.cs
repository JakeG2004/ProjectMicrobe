using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    private string _savePath;

    public static SaveSystem Instance { get; private set; }

    private SaveObject _currentState;

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
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadState();
        }
    }

    public void SaveState()
    {
        if(GameObject.FindGameObjectWithTag("Player"))
        {
            _currentState.playerCosmetics = GetCosmetics();
        }

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

    private List<CosmeticEntry> GetCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(!player)
        {
            Debug.Log("Failed to find player.");
            return null;
        }

        List<CosmeticEntry> cosmetics = new List<CosmeticEntry>();

        // Iterate through each of the cosmetics
        int numChildren = player.transform.childCount;

        // Start at 1 to skip the skeleton
        for(int i = 1; i< numChildren; i++)
        {
            // get the child
            GameObject child = player.transform.GetChild(i).gameObject;

            // get the renderer of the child
            Renderer renderer = child.GetComponent<Renderer>();

            // skip if no renderer
            if(!renderer)
            {
                continue;
            }

            // create material data list
            List<MaterialData> mats = new List<MaterialData>();

            // save each material
            foreach(var mat in renderer.materials)
            {
                // skip if it doesnt have the right fields
                if(!mat.HasColor("_TintR") || !mat.HasColor("_TintG") || !mat.HasColor("_TintB"))
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
