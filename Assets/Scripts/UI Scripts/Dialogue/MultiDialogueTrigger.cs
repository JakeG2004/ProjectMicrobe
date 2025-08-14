// DialogueTrigger.cs
// A script for starting dialogue
// Following brackeys tutorial: https://youtu.be/_nRzoTzeyxU
// Author:  Jake Gendreau
// Date:    8/14/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue[] _dialogues;

    private void Start()
    {
        for (int i = 0; i < _dialogues.Length - 1; i++)
        {
            int nextIndex = i + 1; // capture to avoid closure issues
            _dialogues[i].onDialogueComplete.AddListener(() =>
            {
                DialogueManager.Instance?.StartDialogue(_dialogues[nextIndex]);
            });
        }
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance?.StartDialogue(_dialogues[0]);
    }
}
