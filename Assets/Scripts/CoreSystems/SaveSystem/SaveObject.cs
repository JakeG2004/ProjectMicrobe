using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObject
{
    public string name;
    public List<CosmeticEntry> playerCosmetics = new List<CosmeticEntry>();
}

[System.Serializable]
public class CosmeticEntry
{
    public string name;
    public bool enabled;
    public List<MaterialData> materials;
}

[System.Serializable]
public class MaterialData
{
    public Color tintR;
    public Color tintG;
    public Color tintB;
}
