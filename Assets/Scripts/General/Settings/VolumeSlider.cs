using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private VolumeSettings _vs;
    [SerializeField] private string _paramName = "Slider name";

    public void SetVolume(float val)
    {
        _vs.ChangeGroupVol(_paramName, val);
    }
}
