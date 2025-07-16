// SoundManager.cs
// A script for managing audio in a centralized location
// Author: Jake Gendreau
// Date:    7/15/25
// Following tutorial: https://youtu.be/g5WT91Sn3hg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    MENU_OPEN,
    MENU_CLOSED,
    MENU_SELECTED,
    MENU_CLICK,
    MENU_SLIDER,
    NOTIFICATION,
    OBJECTIVE,
    DOOR,
    EIGHT_BIT_COLLECTED,
    EIGHT_BIT_BASS,
    EIGHT_BIT_JUMP,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _mixer;

    [Space(10)]
    [SerializeField] private SoundList[] _soundList;
    private static SoundManager Instance;
    private AudioSource _as;
    private bool _canPlaySound = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        else
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);   
            }
            Instance = this;
        }

        _as = GetComponent<AudioSource>();
    }

    // Gets a random audio clip from the specified pool
    private AudioClip GetRandomClip(SoundType sound)
    {
        AudioClip[] clips = Instance._soundList[(int)sound].Sounds;
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    // Plays a random sound from the specified pool in 2d space
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip randomClip = SoundManager.Instance.GetRandomClip(sound);
        Instance._as.PlayOneShot(randomClip, volume);
    }

    // Plays a specific audio clip in 2d space
    public static void PlaySound(AudioClip clip, float volume = 1)
    {
        Instance._as.PlayOneShot(clip, volume);
    }

    // Plays random clip from specified pool with spacing between each other
    public static void PlayRapidSound(SoundType sound, float volume = 1)
    {
        if (!SoundManager.Instance._canPlaySound)
        {
            return;
        }

        PlaySound(Instance.GetRandomClip(sound), volume);
        Instance.StartCoroutine(Instance.DelaySound());
    }

    // Plays specific audio clip with spacing between each other
    public static void PlayRapidSound(AudioClip clip, float volume = 1)
    {
        if (!SoundManager.Instance._canPlaySound)
        {
            return;
        }

        PlaySound(clip, volume);
        Instance.StartCoroutine(Instance.DelaySound());
    }

    // Play a sound from a specified pool in 3d space
    public static void PlaySound(Transform pos, SoundType sound, float volume = 1)
    {
        // Spawn the object
        GameObject tmpObj = new GameObject("AudioSource");

        // Set the position to the parent origin
        tmpObj.transform.parent = pos;
        tmpObj.transform.localPosition = Vector3.zero;

        // Add audio source and play it
        AudioSource tmpAudioSrc = tmpObj.AddComponent<AudioSource>();
        tmpAudioSrc.outputAudioMixerGroup = SoundManager.Instance._mixer;
        tmpAudioSrc.PlayOneShot(SoundManager.Instance.GetRandomClip(sound));

        // Cull the audiosource after it's finished
        SoundManager.Instance.StartCoroutine(SoundManager.Instance.CullAfterFinished(tmpAudioSrc));
    }

    // Culls an audio source after it finishes its job
    private IEnumerator CullAfterFinished(AudioSource tmpAudioSrc)
    {
        while (tmpAudioSrc.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(tmpAudioSrc.gameObject);
    }

    // Forces sounds to wait between playing
    private IEnumerator DelaySound()
    {
        _canPlaySound = false;
        yield return new WaitForSeconds(0.05f);
        _canPlaySound = true;
    }

    // Populates the list of sounds when script added to an object
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref _soundList, names.Length);

        for(int i = 0; i < names.Length; i++)
        {
            _soundList[i].name = names[i];
        }
    }
#endif
}

[System.Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;

    public AudioClip[] Sounds { get => sounds; }
}
