using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFaderSlave : MonoBehaviour
{
    private string _slaveName;
    private AudioSource _as;

    void Start()
    {
        _as = GetComponent<AudioSource>();
        _slaveName = gameObject.name;
    }

    public void SetPlaybackPos(int pos)
    {
        _as.timeSamples = pos;
    }

    public int GetPlaybackPos()
    {
        return _as.timeSamples;
    }

    public void Pause()
    {
        _as.Pause();
    }

    public void Play()
    {
        _as.Play();
    }

    public void SetVolume(float vol)
    {
        _as.volume = vol;
    }
}
