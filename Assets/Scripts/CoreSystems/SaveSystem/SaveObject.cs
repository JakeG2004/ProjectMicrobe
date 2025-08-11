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
    public ControlsData controlsData = new ControlsData();
    public List<ObjectiveGroupItem> objectives = new List<ObjectiveGroupItem>();
    public PlayerBackpack backpack = new PlayerBackpack();
    public bool hasCompletedGame = false;
    public int qualityLevel = (int)QualityTiers.HIGH;
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
    public float masterVolume = -3.1f;
    public float musicVolume = -8f;
    public float sfxVolume = 0f;
}

[System.Serializable]
public class ControlsData
{
    public float lookSensitivity = 3f;
    public bool sprintIsToggle = true;
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
    public int hairSecondary = 2;
    public int eyes = 1;
    public int skin = 1;
    public int glasses = 10;
    public int upperBody = 2;
    public int lowerBody = 14;
    public int hat = 0;
}
