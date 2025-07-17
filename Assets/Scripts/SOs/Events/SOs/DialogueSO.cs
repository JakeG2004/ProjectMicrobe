// DialogueSO.cs
// A script for containing dialogue information to be reuseed
// Author:  Jake Gendreau
// Date:    6/24/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "ScriptableObjects/Dialogue/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public AudioClip dialogueSound;
    public Sprite img;
    public string dialogueName;
}
