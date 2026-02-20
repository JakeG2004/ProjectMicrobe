using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticsSaveManager
{
    private SaveObject _currentState;

    public CosmeticsSaveManager(SaveObject saveObject)
    {
        _currentState = saveObject;
    }

    public void UpdateState(SaveObject state)
    {
        _currentState = state;
    }

    // Unlocks cosmetics and prevents duplicate entries
    public void UnlockCosmetic(string cosmeticName)
    {
        if (_currentState.unlockedCosmetics.Contains(cosmeticName))
        {
            return;
        }

        // Add new entry
        _currentState.unlockedCosmetics.Add(cosmeticName);
    }

    // Sets whether a cosmetic is locked based on the save data
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

    // Save the cosmetics
    public void SaveCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Extra search for saving the cosmetics of the player out of the Character Creator since that Ari has a different tag.
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("DummyPlayer");
        }

        if (!player)
        {
            return;
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
            HatPositionManager hatPosMgr = child.GetComponent<HatPositionManager>();

            // skip if no renderer
            if (!renderer && !hatPosMgr)
            {
                continue;
            }

            // Early exit for hats with no renderer
            if (!renderer)
            {
                // Create new cosmetic entry and add it to the list
                cosmetics.Add(new CosmeticEntry
                {
                    name = child.name,
                    enabled = child.activeSelf,
                    materials = null
                });

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

        _currentState.playerCosmetics = cosmetics;
    }

    // Load cosmetics
    public void LoadCosmetics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Extra search for saving the cosmetics of the player out of the Character Creator since that Ari has a different tag.
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("DummyPlayer");
        }

        if (!player)
        {
            Debug.Log("Found no player");
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

                // Skip the current loop if we are loading something with no materials or no renderer
                if (cosmetic.materials == null || renderer == null)
                {
                    continue;
                }

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

    // Saves the CC values
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

    // Loads the character creator values based on saves
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
}
