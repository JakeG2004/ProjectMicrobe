using UnityEngine;

public class CosmeticController : MonoBehaviour
{
    public enum CosmeticType { Hair, Other }

    [SerializeField] private CosmeticType _selectedCosmeticType;
    [SerializeField] private GameObject[] _cosmeticOptions;

    void Start()
    {
        LoadCosmeticOptions();
        UpdateCosmetic(0);
    }

    private void LoadCosmeticOptions()
    {
        switch (_selectedCosmeticType)
        {
            case CosmeticType.Hair:
                _cosmeticOptions = CosmeticContainer.Instance.GetHairStyles();
                break;
            // Add other cases for different cosmetic types here
            default:
                Debug.LogWarning("Invalid cosmetic type selected in Inspector");
                break;
        }
    }

    public void UpdateCosmetic(float value)
    {
        int idx = (int)value;
        for (int i = 0; i < _cosmeticOptions.Length; i++)
        {
            _cosmeticOptions[i].SetActive(i == idx);
        }
    }
} 
