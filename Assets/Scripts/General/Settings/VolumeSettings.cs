using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _am;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    public void ChangeGroupVol(string paramName, float val)
    {
        // Prevent 0 miscalculations
        if (val == 0)
        {
            val = 0.0001f;
        }

        // Set the volume
        _am.SetFloat(paramName, Mathf.Log10(val) * 20);

        SaveVolume();
    }

    void OnEnable()
    {
        SetSlidersFromVals();
    }

    void SetSlidersFromVals()
    {
        // Get the values from the volume mixer
        _am.GetFloat("MasterVolume", out float masterVol);
        _am.GetFloat("MusicVolume", out float musicVol);
        _am.GetFloat("SFXVolume", out float sfxVol);

        // Inverse of logbase10(x) is 10^x
        float masterSliderVal = Mathf.Pow(10, (masterVol / 20));
        float musicSliderVal = Mathf.Pow(10, (musicVol / 20));
        float sfxSliderVal = Mathf.Pow(10, (sfxVol / 20));

        // Set the new values
        _masterSlider.value = masterSliderVal;
        _musicSlider.value = musicSliderVal;
        _sfxSlider.value = sfxSliderVal;
    }

    void SaveVolume()
    {
        // Get the values from the volume mixer
        _am.GetFloat("MasterVolume", out float masterVol);
        _am.GetFloat("MusicVolume", out float musicVol);
        _am.GetFloat("SFXVolume", out float sfxVol);

        SaveSystem.Instance.SaveVolume(new Vector3(masterVol, musicVol, sfxVol));
    }
}
