// GraphicsSettings.cs
// A script for settings graphics settings
// Author:  Jake Gendreau
// Date:    7/8/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField] private Toggle _fullscreenTg;
    [SerializeField] private Toggle _vsyncTg;
    [SerializeField] private TMP_Text _resText;

    [Space(10)]
    [SerializeField] private List<ResItem> _resolutions = new();
    private int _curResIdx;
    void Start()
    {
        SetInfo();
    }

    void OnEnable()
    {
        SetInfo();
    }

    void SetInfo()
    {
        _fullscreenTg.isOn = Screen.fullScreen;
        _vsyncTg.isOn = (QualitySettings.vSyncCount != 0);

        int height = Screen.height;
        int width = Screen.width;

        // If resolution found, set it
        foreach (ResItem res in _resolutions)
        {
            if (res.horizontal == width && res.vertical == height)
            {
                _curResIdx = _resolutions.IndexOf(res);
                UpdateResText();
                return;
            }
        }

        // Add resolution if not found
        ResItem newRes = new();
        newRes.horizontal = Screen.width;
        newRes.vertical = Screen.height;

        _resolutions.Add(newRes);
        _curResIdx = _resolutions.Count - 1;

        UpdateResText();
    }

    public void ApplyGraphics()
    {
        QualitySettings.vSyncCount = (_vsyncTg.isOn ? 1 : 0);

        ResItem curRes = _resolutions[_curResIdx];
        Screen.SetResolution(curRes.horizontal, curRes.vertical, _fullscreenTg.isOn);
    }

    public void ResLeft()
    {
        _curResIdx--;
        if (_curResIdx < 0)
        {
            _curResIdx = 0;
        }

        UpdateResText();
    }

    public void ResRight()
    {
        _curResIdx++;
        if (_curResIdx > _resolutions.Count - 1)
        {
            _curResIdx = _resolutions.Count - 1;
        }

        UpdateResText();
    }

    public void UpdateResText()
    {
        ResItem curRes = _resolutions[_curResIdx];

        _resText.text = $"{curRes.horizontal} x {curRes.vertical}";
    }
}

[System.Serializable]
public class ResItem
{
    [SerializeField] public int horizontal;
    [SerializeField] public int vertical;
}
