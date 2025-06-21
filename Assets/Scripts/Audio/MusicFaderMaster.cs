using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFaderMaster : MonoBehaviour
{
    // Instance so that music may persist across reloads and different scenes
    public static MusicFaderMaster Instance { get; private set; }
    [SerializeField] private float _fadeTime = 1.0f;
    private MusicFaderSlave _curTrack;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        // Set the current track to be the first child, then start playing it
        _curTrack = transform.GetChild(0).GetComponent<MusicFaderSlave>();

        _curTrack.Play();
    }

    public void SmoothSwitchAudio(MusicFaderSlave mfs)
    {
        mfs.Play();
        mfs.SetPlaybackPos(_curTrack.GetPlaybackPos());

        StartCoroutine(IFadeAudio(_curTrack, mfs));
    }

    private IEnumerator IFadeAudio(MusicFaderSlave fadeOut, MusicFaderSlave fadeIn)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float fadeRatio = elapsedTime / _fadeTime;

            fadeOut.SetVolume(1 - fadeRatio);
            fadeIn.SetVolume(fadeRatio);

            yield return null;
        }

        _curTrack = fadeIn;
        fadeOut.Pause();
        fadeIn.SetVolume(1);
    }
}
