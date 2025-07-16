// SoundListSO.cs
// A ScriptableObject which contains sounds of a certain type
// Author:  Jake Gendreau
// Date:    7/15/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundListSO", menuName = "ScriptableObjects/Audio/SoundListSO")]
public class SoundListSO : ScriptableObject
{
    public string soundsName;
    public AudioClip[] sounds;
}
