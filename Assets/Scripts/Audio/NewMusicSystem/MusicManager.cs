using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// List of music types (themes)
public enum MusicType
{
    TITLE,
    EXPLORATION,
    DRONE
}

// List of music genres
public enum MusicGenre
{
    ORCHESTRAL,
    EIGHT_BIT,
    CHILL
}

[ExecuteInEditMode]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _mixer;

    [Space(10)]
    [SerializeField] private MusicSet[] _musicList;

    [Space(10)]
    [SerializeField] private float _fadeTime = .5f;

    private static MusicManager Instance;
    private List<AudioSource> _sources = new();
    private AudioSource _fadeSource;
    private MusicGenre _currentGenre = MusicGenre.ORCHESTRAL;
    private MusicType _currentType = MusicType.TITLE;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        else
        {
            Instance = this;

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        if (Application.isPlaying)
        {
            AddSources();
        }
    }

    void Start()
    {
        LevelLoader.Instance.OnSceneUnload += StartLoadingScreenMusic;
    }

    // Function to switch the genre to chill for loading screens
    private void StartLoadingScreenMusic()
    {
        SwitchToGenre(MusicGenre.CHILL);
    }

    // Fades the current audio out, whatever that may be
    private void FadeAudioOut()
    {
        AudioSource currentSource = Instance._sources[(int)Instance._currentGenre];
        Instance.StartCoroutine(Instance.FadeAudio(0, _fadeTime / 2, currentSource));
    }

    // Adds the Audio Sources to the gameobject during runtime
    private void AddSources()
    {
        int numGenres = System.Enum.GetNames(typeof(MusicGenre)).Length;

        for (int i = 0; i < numGenres; i++)
        {
            _sources.Add(gameObject.AddComponent<AudioSource>());
            _sources[i].outputAudioMixerGroup = MusicManager.Instance._mixer;
            _sources[i].loop = true;
        }

        _fadeSource = gameObject.AddComponent<AudioSource>();
        _fadeSource.outputAudioMixerGroup = MusicManager.Instance._mixer;
        _fadeSource.loop = true;
    }

    // Switches from the current music type to the specified type and genre
    public static void SwitchToType(MusicType type, MusicGenre genre)
    {
        Instance.StopAllCoroutines();

        if (type == Instance._currentType)
        {
            SwitchToGenre(genre);
            return;
        }

        AudioSource currentSource = Instance._sources[(int)Instance._currentGenre];
        AudioSource targetSource = Instance._sources[(int)genre];

        AudioClip targetClip = Instance._musicList[(int)type].clips[(int)genre].clip;
        Instance._currentGenre = genre;
        Instance._currentType = type;

        Instance.StartCoroutine(Instance.FadeOutInAudio(currentSource, targetSource, targetClip));
    }

    // Switches the genre of music specified. Type remains the same
    public static void SwitchToGenre(MusicGenre genre)
    {
        Instance.StopAllCoroutines();

        if (Instance._currentGenre == genre)
        {
            return;
        }

        AudioSource currentSource = Instance._sources[(int)Instance._currentGenre];
        AudioSource targetSource = Instance._sources[(int)genre];

        AudioClip targetClip = Instance._musicList[(int)Instance._currentType].clips[(int)genre].clip;

        Instance._currentGenre = genre;

        Instance.StartCoroutine(Instance.CrossfadeGenre(currentSource, targetSource, targetClip));
    }

    // Fades the audio of the current source to a target volume 
    private IEnumerator FadeAudio(float targetVol, float fadeTime, AudioSource fadeSrc)
    {
        float startVol = fadeSrc.volume;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / fadeTime;

            fadeSrc.volume = Mathf.Lerp(startVol, targetVol, ratio);
            yield return null;
        }

        fadeSrc.volume = targetVol;
    }

    // Fades out the current audio, switches the audio to the target, then fades in the new audio
    private IEnumerator FadeOutInAudio(AudioSource fadeOut, AudioSource fadeIn, AudioClip targetClip)
    {
        if (fadeOut.clip == targetClip)
        {
            yield break;
        }

        if (fadeOut.isPlaying)
        {
            _fadeSource.clip = targetClip;
            _fadeSource.volume = 0;
            _fadeSource.timeSamples = 0;

            float elapsedTime = 0f;
            while (elapsedTime < _fadeTime)
            {
                float ratio = elapsedTime / _fadeTime;

                _fadeSource.volume = ratio;
                fadeOut.volume = 1 - ratio;

                yield return null;
            }

            fadeOut.Pause();
        }

        // turn off fadeSource
        _fadeSource.volume = 0;
        _fadeSource.Pause();

        // Establish the new current audio
        fadeIn.clip = targetClip;
        fadeIn.volume = 1f;
        fadeIn.timeSamples = _fadeSource.timeSamples;
        fadeIn.Play();
    }

    // Transitions between two genres by fading one out and fading another in
    private IEnumerator CrossfadeGenre(AudioSource fadeOut, AudioSource fadeIn, AudioClip clip)
    {
        fadeIn.volume = 0f;
        fadeIn.clip = clip;
        fadeIn.Play();
        fadeIn.timeSamples = fadeOut.timeSamples;

        float elapsedTime = 0.0f;

        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float fadeRatio = elapsedTime / _fadeTime;

            fadeOut.volume = (1 - fadeRatio);
            fadeIn.volume = fadeRatio;

            yield return null;
        }

        fadeOut.Pause();
        fadeIn.volume = 1f;
    }

    // Creates the pre-filled editor arrays
    #if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = System.Enum.GetNames(typeof(MusicType));
        string[] genres = System.Enum.GetNames(typeof(MusicGenre));
        System.Array.Resize(ref _musicList, names.Length);

        // Add new entries as needed
        for (int i = 0; i < names.Length; i++)
        {
            _musicList[i].name = names[i];

            if (_musicList[i].clips == null)
                _musicList[i].clips = new List<Song>();

            for (int j = 0; j < genres.Length; j++)
            {
                string genreName = genres[j];

                // Only add if it doesn't already exist
                if (!_musicList[i].clips.Exists(song => song.name == genreName))
                {
                    Song newSong = new Song
                    {
                        name = genreName,
                        clip = null
                    };

                    _musicList[i].clips.Add(newSong);
                }
            }
        }

        // Remove no longer needed entries
        for (int i = 0; i < _musicList.Length; i++)
        {
            MusicSet set = _musicList[i];

            if (set.clips == null)
                continue;

            // Remove any clips whose names don't match a valid genre
            for (int j = set.clips.Count - 1; j >= 0; j--)
            {
                if (!System.Array.Exists(genres, g => g == set.clips[j].name))
                {
                    set.clips.RemoveAt(j);
                }
            }

            _musicList[i] = set; // Needed because MusicSet is a struct
        }
    }
    #endif
}

[System.Serializable]
public struct MusicSet
{
    [HideInInspector] public string name;
    [SerializeField] public List<Song> clips;
}

[System.Serializable]
public struct Song
{
    [HideInInspector] public string name;
    [SerializeField] public AudioClip clip;
}