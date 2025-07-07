using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassesColorController : BaseToggleGroupController
{
    public enum AccessoryType
    {
        Glasses,
        Goggles,
        None,
    }
    [SerializeField] private ColorTupleSO _lensColor;
    [SerializeField] private AccessoryType _accesoryType;
    [SerializeField] private GameObject[] _glassesStyles;

    protected override void Start()
    {
        base.Start();

        _glassesStyles = CosmeticContainer.Instance?.GetGlassesStyles();
    }

    protected override void OnTurnOn()
    {
        EnableGOs();
        AssignColors();
    }

    public void AssignColors()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        if (_colorTuple == null || _lensColor == null)
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
                    mat.SetColor("_TintR", _colorTuple.r);
                    mat.SetColor("_TintG", _colorTuple.g);
                    mat.SetColor("_TintB", _colorTuple.b);
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
