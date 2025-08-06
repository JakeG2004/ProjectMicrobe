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

// List of sound pools that can be played
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
    DRONE_TAKEOFF,
    DRONE_FLIGHT,
    DRONE_LANDING,
    FOOTSTEP_SOFT,
    FOOTSTEP_HARD,
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
    private List<AudioSource> _curSounds = new();

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

    void Start()
    {
        LevelLoader.Instance.OnSceneUnload += FadeOutAllSounds;
        LevelLoader.Instance.OnSceneLoad += TurnOnSounds;
        _curSounds.Add(_as);
    }

    // Called on level unload, fades out all sound effects
    private void FadeOutAllSounds()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutSounds());
    }

    // Called on level load, resets volume
    private void TurnOnSounds()
    {
        _canPlaySound = true;
        _as.volume = 1f;
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
        tmpAudioSrc.spatialBlend = 1;
        tmpAudioSrc.outputAudioMixerGroup = SoundManager.Instance._mixer;
        tmpAudioSrc.PlayOneShot(SoundManager.Instance.GetRandomClip(sound));

        Instance._curSounds.Add(tmpAudioSrc);

        // Cull the audiosource after it's finished
        SoundManager.Instance.StartCoroutine(SoundManager.Instance.CullAfterFinished(tmpAudioSrc));
    }

    // Creates, starts, and returns a looping audio which can be stopped externally
    public static LoopingSoundHandle PlayLoopingSoundWithIntroAndOutro(AudioClip intro, AudioClip loop, AudioClip outro, Transform parentObj = null, float minDist = 1, float volume = 1)
    {
        LoopingSoundHandle handle = new LoopingSoundHandle();
        handle.owner = Instance;
        handle.outro = outro;
        handle.coroutine = Instance.StartCoroutine(Instance.PlayLoopingSoundCoroutine(handle, intro, loop, parentObj, minDist, volume));

        Instance._curSounds.Add(handle.source);

        return handle;
    }

    // Create a looping sound based on args
    public static LoopingSoundHandle PlayLoopingSoundWithIntroAndOutro(SoundType intro, SoundType loop, SoundType outro, Transform parentObj = null, float minDist = 1, float volume = 1)
    {
        return PlayLoopingSoundWithIntroAndOutro(Instance.GetRandomClip(intro), Instance.GetRandomClip(loop), Instance.GetRandomClip(outro), parentObj, minDist, volume);
    }

    // Starts a looping sound coroutine
    private IEnumerator PlayLoopingSoundCoroutine(LoopingSoundHandle handle, AudioClip intro, AudioClip loop, Transform parentObj, float minDist, float volume)
    {
        GameObject newSrcObj = new GameObject("LoopingAudioSource");
        if (parentObj != null)
        {
            newSrcObj.transform.parent = parentObj;
            newSrcObj.transform.localPosition = Vector3.zero;
        }
        handle.gameObject = newSrcObj;

        AudioSource audioSource = newSrcObj.AddComponent<AudioSource>();
        handle.source = audioSource;

        audioSource.outputAudioMixerGroup = _mixer;
        audioSource.volume = volume;

        if (parentObj != null)
        {
            audioSource.spatialBlend = 1;
            audioSource.minDistance = minDist;
        }

        // Plays the intro to the loop
        if (intro != null)
        {
            audioSource.clip = intro;
            audioSource.Play();

            // Wait for the clip to get almost finished before switching.
            // We do this so that it'll work the same regardless of pitch, and will not have any downtime between clips
            int numSamples = audioSource.clip.samples;
            while (audioSource != null && audioSource.timeSamples < (numSamples * .95))
            {
                yield return null;
            }
        }

        // Prevent null issues and prevent audio from playing when started and rapidly stopped
        if (handle == null || audioSource == null || handle.isStopping)
        {
            yield break;
        }

        // Play the loop
        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.Play();

        // Wait for stop
        while (true)
        {
            if (audioSource == null)
            {
                yield break;
            }

            if (!audioSource.loop)
            {
                break;
            }

            yield return null;
        }

        _curSounds.Remove(audioSource);
    }

    // Stops a sound from looping, puts it into its exit state
    public IEnumerator StopLoopingSoundCoroutine(LoopingSoundHandle handle)
    {
        if (handle == null || handle.source == null)
        {
            yield break;
        }

        handle.source.loop = false;

        // Stop pitch lerping if active
        if (handle.pitchCoroutine != null)
        {
            StopCoroutine(handle.pitchCoroutine);
            handle.pitchCoroutine = null;
        }

        if (handle.outro != null)
        {
            handle.source.clip = handle.outro;
            handle.source.Play();
            yield return new WaitForSeconds(handle.outro.length);
        }

        if (handle.gameObject != null)
        {
            handle.source = null;
            Destroy(handle.gameObject);
        }
    }

    // Culls an audio source after it finishes playing
    private IEnumerator CullAfterFinished(AudioSource tmpAudioSrc)
    {
        while (tmpAudioSrc.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        _curSounds.Remove(tmpAudioSrc);
        Destroy(tmpAudioSrc.gameObject);
    }

    // Forces sounds to wait before playing
    private IEnumerator DelaySound()
    {
        _canPlaySound = false;
        yield return new WaitForSeconds(0.05f);
        _canPlaySound = true;
    }

    // Lerps the pitch of a looping sound (i.e. drone pitch)
    public IEnumerator LerpPitch(LoopingSoundHandle handle)
    {
        if (handle.source == null)
            yield break;

        while (handle.source != null && (handle.source.loop || handle.source.isPlaying))
        {
            handle.source.pitch = Mathf.Lerp(handle.source.pitch, handle.targetPitch, 0.9f);
            yield return null;
        }

        handle.IsPitchLerping = false;
        handle.pitchCoroutine = null;
    }
    
    // Handles fading out every sound that is being played for level unload
    private IEnumerator FadeOutSounds()
    {
        float elapsedTime = 0f;
        float totalTime = 0.5f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / totalTime;

            foreach (AudioSource curAudio in _curSounds)
            {
                curAudio.volume = (1 - ratio);
            }

            yield return null;
        }

        foreach (AudioSource curAudio in _curSounds)
        {
            curAudio.volume = 0;
        }

        _curSounds.Clear();
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

public class LoopingSoundHandle
{
    public GameObject gameObject;
    public AudioSource source;
    public Coroutine coroutine;
    public Coroutine pitchCoroutine;
    public SoundManager owner;
    public AudioClip outro;
    public float targetPitch = 10.0f;
    public bool IsPitchLerping { get; set; } = false;

    public bool isStopping { get; set; } = false;

    // Signals to the sound manager to stop the looping sound
    public void Stop()
    {
        // Exit if already stopping or no owner
        if (isStopping || owner == null)
        {
            return;
        }

        isStopping = true;
        owner.StartCoroutine(owner.StopLoopingSoundCoroutine(this));
    }

    // Sets whether pitch should be lerped to
    public void IsLerpingPitch(bool state)
    {
        IsPitchLerping = state;

        if (state)
        {
            pitchCoroutine = owner.StartCoroutine(owner.LerpPitch(this));
        }
    }

    // Sets the target pitch if lerping, or immediately sets it
    public void SetPitch(float newPitch)
    {
        if (source == null || owner == null)
            return;

        if (!IsPitchLerping)
        {
            source.pitch = newPitch;
            return;
        }

        targetPitch = newPitch;
    }
}
