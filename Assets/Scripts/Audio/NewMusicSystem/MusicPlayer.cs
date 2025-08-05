using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private MusicType _type;
    [SerializeField] private MusicGenre _genre;

    public void SwitchMusic()
    {
        MusicManager.SwitchToType(_type, _genre);
    }
}
