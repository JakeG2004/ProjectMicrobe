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

    [Space(10)]
    [SerializeField] private UnityEvent _onDisplayDialogue;
    [SerializeField] private UnityEvent _onFinishDialogue;
    private AudioClip _dialogueSound;
    private float _charactersPerSecond = 60f;
    private bool _isTyping = false;

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

        _dialogueSound = _dso.dialogueSound;

        // Set the image to be the image in the Dialogue unit if its not null, disable if null
        _img.enabled = (_dso.img != null);
        _img.sprite = _dso.img;

        // Set the name text
        _nameText.text = _dso.dialogueName;

        // Set the music to be chill
        MusicManager.SwitchToGenre(MusicGenre.CHILL);

        // Start the dialogue
        DisplayNextSentence();
    }

    // Displays the next sentence in the queue
    public void DisplayNextSentence()
    {
        // Skip to the end of the sentence if button is pressed during dialogue
        if (_isTyping)
        {
            _isTyping = false;
            return;
        }

        // Check for complete dialogue
        if (_sentences.Count == 0 || _curDialogue == null)
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
        _isTyping = true;

        SoundManager.PlaySound(SoundType.MENU_OPEN);

        _bodyText.text = "";
        float delay = 1f / _charactersPerSecond;
        float timer = 0f;
        int charIndex = 0;

        while (charIndex < sentence.Length)
        {
            timer += Time.unscaledDeltaTime;

            if (_isTyping == false)
            {
                _bodyText.text = sentence;
                yield break;
            }

            while (timer >= delay && charIndex < sentence.Length)
            {
                _bodyText.text += sentence[charIndex];
                SoundManager.PlayRapidSound(_dialogueSound);
                charIndex++;
                timer -= delay;
            }

            if (_anim.GetBool("IsOpen") == false)
            {
                yield break;
            }

            yield return null; // wait for next frame
        }

        _isTyping = false;
    }

    // Ends the dialogue
    public void EndDialogue()
    {
        if (_curDialogue == null)
        {
            return;
        }

        MusicManager.SwitchToGenre(MusicGenre.ORCHESTRAL);

        _onFinishDialogue.Invoke();

        _curDialogue.onDialogueComplete?.Invoke();
        _anim.SetBool("IsOpen", false);

        _curDialogue = null;
    }
}
