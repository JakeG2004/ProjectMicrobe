// CosmeticController.cs
// A generalized script for controlling any cosmetic with a slider
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticController : MonoBehaviour
{
    public enum CosmeticType { Hair, Top, Bottom, Glasses, Eyebrows, Hats, Other }

    [SerializeField] private CosmeticType _selectedCosmeticType;
    [SerializeField] private GameObject[] _cosmeticOptions;
    [SerializeField] private bool _includeNone = false;
    private List<HatPositionManager> _hats = new();
    private Slider _slider;
    private float _curVal;

    void Start()
    {
        LoadCosmeticOptions();

        _slider = GetComponent<Slider>();
        if (!_slider)
        {
            Debug.Log("Failed to get slider!");
        }

        _slider.maxValue = _includeNone ? _cosmeticOptions.Length : _cosmeticOptions.Length - 1;

        foreach (GameObject hat in CosmeticContainer.Instance.GetHats())
        {
            _hats.Add(hat.GetComponent<HatPositionManager>());
        }

        UpdateCosmetic(0);
    }

    private void LoadCosmeticOptions()
    {
        switch (_selectedCosmeticType)
        {
            case CosmeticType.Hair:
                _cosmeticOptions = CosmeticContainer.Instance.GetHairStyles();
                break;
            case CosmeticType.Top:
                _cosmeticOptions = CosmeticContainer.Instance.GetTopStyles();
                break;
            case CosmeticType.Bottom:
                _cosmeticOptions = CosmeticContainer.Instance.GetBottomStyles();
                break;
            case CosmeticType.Glasses:
                _cosmeticOptions = CosmeticContainer.Instance.GetGlassesStyles();
                break;
            case CosmeticType.Eyebrows:
                _cosmeticOptions = CosmeticContainer.Instance.GetEyebrowStyles();
                break;
            case CosmeticType.Hats:
                _cosmeticOptions = CosmeticContainer.Instance.GetHats();
                break;
            default:
                Debug.LogWarning("Invalid cosmetic type selected in Inspector");
                break;
        }
    }

    public void UpdateCosmetic(float value)
    {
        SetVal(value);

        int idx = (int)value;
        for (int i = 0; i < _cosmeticOptions.Length; i++)
        {
            _cosmeticOptions[i].SetActive(i == idx);
        }

        if (_selectedCosmeticType == CosmeticType.Hair)
        {
            SaveSystem.Instance.SetHairIndex(idx);
            foreach (HatPositionManager hat in _hats)
            {
                hat.UpdateHatPos();
            }
        }
    }

    public float GetVal()
    {
        return _curVal;
    }

    public void SetVal(float value)
    {
        _curVal = value;
    }
} 
