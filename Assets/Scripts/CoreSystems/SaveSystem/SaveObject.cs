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
    public string regionName;
    public Vector3 pylonPosition;
    public Quaternion pylonRotation;
    public List<MicrobeNamePopPair> microbes = new();
}

[System.Serializable]
public class MicrobeNamePopPair
{
    public string name;
    public float pop;
}

[System.Serializable]
public class VolumeData
{
    // Initialize to all zero dB
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
}
