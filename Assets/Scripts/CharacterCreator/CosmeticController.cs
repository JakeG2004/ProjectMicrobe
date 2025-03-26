using UnityEngine;

public class CosmeticController : MonoBehaviour
{
    [SerializeField] private GameObject[] _cosmeticOptions;

    void Start()
    {
        UpdateCosmetic(0);
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
