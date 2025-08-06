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

    private void SwitchGenre(MusicGenre genre)
    {
        MusicManager.SwitchToGenre(genre);
    }

    public void SwitchToEightBit() => SwitchGenre(MusicGenre.EIGHT_BIT);
    public void SwitchToOrchestral() => SwitchGenre(MusicGenre.ORCHESTRAL);
    public void SwitchToChill() => SwitchGenre(MusicGenre.CHILL);

}
