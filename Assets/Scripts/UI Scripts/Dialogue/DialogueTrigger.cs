// DialogueTrigger.cs
// A script for starting dialogue
// Following brackeys tutorial: https://youtu.be/_nRzoTzeyxU
// Author:  Jake Gendreau
// Date:    5/19/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue _dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(_dialogue);
    }

    public void SwitchDialogue(Dialogue newDialogue)
    {
        _dialogue = newDialogue;
    }
}
