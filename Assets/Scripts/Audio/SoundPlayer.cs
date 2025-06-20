// MenuSoundPlayer.cs
// A script for managing sounds in a menu
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _sounds = new();
    [SerializeField] private float _rapidPlayPeriod = 0.05f;
    private bool _playedRapidSound = false;
    private bool _playingWithPriority = false;

    private AudioSource _as;

    void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    public void PlaySound(int idx)
    {
        if (_playingWithPriority)
        {
            return;
        }

        _as?.PlayOneShot(_sounds[idx]);
    }

    public void PlayWithPriority(int idx)
    {
        PlaySound(idx);

        _playingWithPriority = true;
        StartCoroutine(IWaitforPriorityToEnd());
    }

    public void PlayRapidSound(int idx)
    {
        if (_playedRapidSound)
        {
            return;
        }

        _playedRapidSound = true;

        PlaySound(idx);

        StartCoroutine(IWaitToPlaySound());
    }

    private IEnumerator IWaitforPriorityToEnd()
    {
        while (_as.isPlaying)
        {
            yield return null;
        }

        _playingWithPriority = false;
    }

    private IEnumerator IWaitToPlaySound()
    {
        yield return new WaitForSeconds(_rapidPlayPeriod);
        _playedRapidSound = false;
    }
}
