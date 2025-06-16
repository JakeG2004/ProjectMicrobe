using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassesColorController : MonoBehaviour
{
    public enum AccessoryType
    {
        Glasses,
        Goggles,
        None,
    }

    private Toggle _tg;
    [SerializeField] private ColorTuple _lensColor;
    [SerializeField] private ColorTuple _bodyColor;
    [SerializeField] private AccessoryType _accesoryType;
    [SerializeField] private GameObject[] _glassesStyles;

    void Start()
    {
        _glassesStyles = CosmeticContainer.Instance?.GetGlassesStyles();

        _tg = GetComponent<Toggle>();

        if (!_tg)
        {
            Debug.Log("Failed to get Toggle");
        }

        GetComponent<Image>().color = _bodyColor.r;
        _tg.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            AssignColors();
            EnableGOs();
        }
    }

    public void AssignColors()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        // Assign colors
        foreach (GameObject glasses in _glassesStyles)
        {
            Renderer renderer = glasses.GetComponent<Renderer>();

            // Iterate through every material
            foreach (Material mat in renderer.materials)
            {
                // Set body colors
                if (mat.name.Contains("m_Ari_ClothGlasses") || mat.name.Contains("m_Ari_ClothGoggles"))
                {
                    mat.SetColor("_TintR", _bodyColor.r);
                    mat.SetColor("_TintG", _bodyColor.g);
                    mat.SetColor("_TintB", _bodyColor.b);
                    continue;
                }
                
                // Set lens colors
                if (mat.name.Contains("m_Ari_Glass_Lens"))
                {
                    mat.SetColor("_TintR", _lensColor.r);
                    mat.SetColor("_TintG", _lensColor.g);
                    mat.SetColor("_TintB", _lensColor.b);
                    continue;
                }
            }
        }
    }

    private void EnableGOs()
    {
        foreach (GameObject go in _glassesStyles)
            {
                // Handle none
                if (_accesoryType == AccessoryType.None)
                {
                    go.SetActive(false);
                }
                
                // Handle glasses
                if (go.name == "Glasses")
                {
                    go.SetActive(_accesoryType == AccessoryType.Glasses);
                }

                // Handle goggles
                if (go.name == "Goggles")
                {
                    go.SetActive(_accesoryType == AccessoryType.Goggles);
                }
            }
    }
}
