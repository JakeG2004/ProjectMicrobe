// Dialogue.cs
// A script for managing dialogue
// Following brackeys tutorial: https://youtu.be/_nRzoTzeyxU
// Author:  Jake Gendreau
// Date:    5/19/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private Queue<DialogueUnit> _sentences;
    private Animator _anim;
    private Dialogue _curDialogue;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _bodyText;
    [SerializeField] private Image _img;

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

        _curDialogue = dialogue;

        // Clear and populate sentence queue
        _sentences.Clear();
        foreach (DialogueUnit sentence in dialogue.sentences)
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

        // Get next sentence
        DialogueUnit _curSentence = _sentences.Dequeue();

        // Set the image to be the image in the Dialogue unit if its not null, disable if null
        _img.enabled = (_curSentence.img != null);
        _img.sprite = _curSentence.img;

        // Set the name text
        _nameText.text = _curSentence.name;

        // Set the body text
        string sentence = _curSentence.sentence;
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

            // Show characters at 60 / sec
            yield return new WaitForSeconds(.016f);
        }
    }

    // Ends the dialogue
    public void EndDialogue()
    {
        if (_curDialogue == null)
        {
            return;
        }

        _curDialogue.onDialogueComplete?.Invoke();
        _anim.SetBool("IsOpen", false);
    }
}
