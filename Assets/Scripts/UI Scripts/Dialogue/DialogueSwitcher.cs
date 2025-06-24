// DialogueSwitcher.cs
// A script for switching between dialogues for a single dialogue trigger
// Author:  Jake Gendreau
// Date:    5/31/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSwitcher : MonoBehaviour
{
    [SerializeField] private Dialogue[] _dialogues;
    [SerializeField] private DialogueTrigger _dialogueTrigger;

    public void SetDialogue(int _dialogueIndex)
    {
        // Handle no dialogue trigger
        if (!_dialogueTrigger)
        {
            Debug.LogWarning("Failed to get dialogue trigger!");
            return;
        }

        // Handle out of bounds
        if (_dialogueIndex >= _dialogues.Length || _dialogueIndex < 0)
        {
            Debug.LogWarning("Dialogue index is outside of bounds!");
            return;
        }

        // Switch the dialogue
        _dialogueTrigger.SwitchDialogue(_dialogues[_dialogueIndex]);
    }
}
