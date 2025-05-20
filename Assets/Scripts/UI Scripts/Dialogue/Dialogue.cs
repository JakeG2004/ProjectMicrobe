// Dialogue.cs
// A class for housing dialogue information
// Following brackeys tutorial: https://youtu.be/_nRzoTzeyxU
// Author:  Jake Gendreau
// Date:    5/19/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public DialogueUnit[] sentences;
}

[System.Serializable]
public class DialogueUnit
{
    public Sprite img;
    public string name;
    [TextArea(2, 3)]
    public string sentence;
}
