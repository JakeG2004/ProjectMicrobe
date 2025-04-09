using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HairColorController : MonoBehaviour
{
    private Toggle _tg;
    private Color _color;

    private GameObject[] _hairStyles;
    private GameObject[] _eyebrowStyles;

    [Tooltip("R - Primary\nG - Secondary\nB - Highlight")]
    [SerializeField] private ColorTuple _hairColor;

    [SerializeField] private bool _isPrimary;

    void Start()
    {
        _hairStyles = CosmeticContainer.Instance?.GetHairStyles();
        _eyebrowStyles = CosmeticContainer.Instance?.GetEyebrowStyles();

        _tg = GetComponent<Toggle>();

        if (!_tg)
        {
            Debug.Log("Failed to get Toggle");
        }

        ColorBlock tgColors = new ColorBlock();
        tgColors.normalColor = _hairColor.r;
        tgColors.selectedColor = _hairColor.r;
        tgColors.disabledColor = _hairColor.r;
        tgColors.highlightedColor = _hairColor.g;
        tgColors.pressedColor = _hairColor.g;
        tgColors.colorMultiplier = 1.0f;
        tgColors.fadeDuration = 0.1f;

        _tg.colors = tgColors;

        _tg.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            AssignHairColors();

            if(_isPrimary)
            {
                AssignEyebrowColors();
            }
        }
    }

    public void AssignHairColors()
    {
        foreach (GameObject hair in _hairStyles)
        {
            Renderer renderer = hair.GetComponent<Renderer>();
            Material mat = renderer.material;
            if(_isPrimary)
            {
                mat.SetColor("_TintR", _hairColor.r);
                // Set highlight color to hair color b
                mat.SetColor("_HighlightColor", _hairColor.b);
            }

            else
            {
                mat.SetColor("_TintG", _hairColor.g);
            }
        }
    }

    public void AssignEyebrowColors()
    {
        foreach (GameObject eyebrow in _eyebrowStyles)
        {
            Renderer renderer = eyebrow.GetComponent<Renderer>();
            foreach(Material mat in renderer.materials)
            {
                if (mat.name.Contains("m_Ari_EyeBrow"))
                {
                    mat.SetColor("_TintR", _hairColor.r);
                    mat.SetColor("_TintG", _hairColor.g);
                    mat.SetColor("_HighlightColor", _hairColor.b);
                }

                if (mat.name.Contains("m_Ari_Mustache"))
                {
                    mat.SetColor("_TintG", _hairColor.r);
                    mat.SetColor("_TintB", _hairColor.g);
                }
            }
        }
    }
}
