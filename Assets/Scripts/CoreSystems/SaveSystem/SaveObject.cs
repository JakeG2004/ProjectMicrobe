using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObject
{
    public string name;
    public List<CosmeticEntry> playerCosmetics = new List<CosmeticEntry>();
    public List<RegionData> regionData = new List<RegionData>();
    public VolumeData volumeData = new VolumeData();
    public List<ObjectiveGroupItem> objectives = new List<ObjectiveGroupItem>();
    public PlayerBackpack backpack = new PlayerBackpack();
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

[System.Serializable]
public class RegionData
{
    public bool isActive = false;
    public string regionName;
    public Vector3 pylonPosition;
    public Quaternion pylonRotation;
    public List<StringFloatPair> microbes = new();
    public List<StringFloatPair> resources = new();
    public float[] healthHistory;
    public float[] mycorrhisArray;
}

[System.Serializable]
public class StringFloatPair
{
    public string name;
    public float amount;
}

[System.Serializable]
public class VolumeData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
}

[System.Serializable]
public class ObjectiveGroupItem
{
    public string name;
    public List<string> completeObjectives;
    public string currentObjective;
    public bool complete;
}

[System.Serializable]
public class PlayerBackpack
{
    public List<StringFloatPair> carriedMicrobes;
    public bool hasPylon;
}
