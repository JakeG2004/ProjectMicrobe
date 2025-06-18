using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObject
{
    public string name;
    public List<string> unlockedCosmetics = new List<string>();
    public List<CosmeticEntry> playerCosmetics = new List<CosmeticEntry>();
    public CCUIVals ccVals = new CCUIVals();
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
    public float masterVolume = 0;
    public float musicVolume = 0;
    public float sfxVolume = 0;
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

[System.Serializable]
public class CCUIVals
{
    public float hairIndex = 8;
    public int hairPrimary = 3;
    public int hairSecondary = 12;
    public int eyes = 1;
    public int skin = 1;
    public int glasses = 10;
    public int upperBody = 2;
    public int lowerBody = 14;
    public int hat = 0;

    /*
    BASIC ARI LOOK:
        "hairIndex": 0,
        "hairPrimary": 3,
        "hairSecondary": 12,
        "eyes": 1,
        "skin": 1,
        "glasses": 10,
        "upperBody": 2,
        "lowerBody": 14

    ANOTHER SENSIBLE DEFAULT:
        "hairIndex": 0
        "hairPrimary": 16
        "hairSecondary": 2
        "eyes": 0
        "skin": 2
        "glasses": 0
        "upperBody": 7
        "lowerBody": 5
    */
}
