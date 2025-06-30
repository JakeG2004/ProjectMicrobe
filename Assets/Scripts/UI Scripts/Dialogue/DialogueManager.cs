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
    private SoundPlayer _sp;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _bodyText;
    [SerializeField] private Image _img;

    [Space(10)]
    [SerializeField] private UnityEvent _onDisplayDialogue;
    [SerializeField] private UnityEvent _onFinishDialogue;

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
        _sp = GetComponent<SoundPlayer>();
    }

    // Starts the dialogue using the dialogue box
    public void StartDialogue(Dialogue dialogue)
    {
        StartCoroutine(DelayStartDialogue(dialogue));
    }

    public IEnumerator DelayStartDialogue(Dialogue dialogue)
    {
        yield return new WaitForSeconds(0.1f);

        // Show the dialogue
        _anim.SetBool("IsOpen", true);

        _curDialogue = dialogue;
        DialogueSO _dso = _curDialogue.dialogueSO;

        // Clear and populate sentence queue
        _sentences.Clear();
        foreach (DialogueUnit sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }

        _sp.SetSound(0, _dso.dialogueSound);

        // Set the image to be the image in the Dialogue unit if its not null, disable if null
        _img.enabled = (_dso.img != null);
        _img.sprite = _dso.img;

        // Set the name text
        _nameText.text = _dso.dialogueName;

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

        _onDisplayDialogue.Invoke();

        // Get next sentence
        DialogueUnit _curSentence = _sentences.Dequeue();

        // Set the body text
        string sentence = _curSentence.sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // Types the sentence one character at a time
    private IEnumerator TypeSentence(string sentence)
    {
        // Play the continue sound
        GetComponent<SoundPlayer>().PlaySound(1);

        _bodyText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            _bodyText.text += letter;
            _sp.PlayRapidSound(0);

            if (_anim.GetBool("IsOpen") == false)
            {
                yield break;
            }

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

        _onFinishDialogue.Invoke();

        _curDialogue.onDialogueComplete?.Invoke();
        _anim.SetBool("IsOpen", false);
    }
}
