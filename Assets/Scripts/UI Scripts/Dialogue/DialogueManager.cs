// Dialogue.cs
// A script for managing dialogue
// Following brackeys tutorial: https://youtu.be/_nRzoTzeyxU
// Author:  Jake Gendreau
// Date:    5/19/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private Queue<string> _sentences;
    private Animator _anim;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _bodyText;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton logic
        if (Instance != this && Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _sentences = new();
        _anim = GetComponent<Animator>();
    }

    // Starts the dialogue using the dialogue box
    public void StartDialogue(Dialogue dialogue)
    {
        // Show the dialogue
        _anim.SetBool("IsOpen", true);

        // Set the dialogue name
        _nameText.text = dialogue.name;

        // Clear and populate sentence queue
        _sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }

        // Start the dialogue
        DisplayNextSentence();
    }

    // Displays the next sentence in the queue
    public void DisplayNextSentence()
    {
        // Check for complete dialogue
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Get and show the next sentence
        string sentence = _sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // Types the sentence one character at a time
    private IEnumerator TypeSentence(string sentence)
    {
        _bodyText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            _bodyText.text += letter;
            yield return null;
        }
    }

    // Ends the dialogue
    public void EndDialogue()
    {
        _anim.SetBool("IsOpen", false);
    }
}
