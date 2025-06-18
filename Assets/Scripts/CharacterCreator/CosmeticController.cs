using UnityEngine;
using UnityEngine.UI;

public class CosmeticController : MonoBehaviour
{
    public enum CosmeticType { Hair, Top, Bottom, Glasses, Eyebrows, Hats, Other }

    [SerializeField] private CosmeticType _selectedCosmeticType;
    [SerializeField] private GameObject[] _cosmeticOptions;
    [SerializeField] private bool _includeNone = false;
    private GameObject[] _hats;
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
        UpdateCosmetic(0);

        _hats = CosmeticContainer.Instance.GetHats();
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
        _curVal = value;

        int idx = (int)value;
        for (int i = 0; i < _cosmeticOptions.Length; i++)
        {
            _cosmeticOptions[i].SetActive(i == idx);
        }
    }

    public float GetVal()
    {
        return _curVal;
    }

    public void DisableAllHats()
    {
        foreach (GameObject hat in _hats)
        {
            hat.SetActive(false);
        }
    }
} 
