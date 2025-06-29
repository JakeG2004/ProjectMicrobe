using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeColorController : MonoBehaviour
{
    private Toggle _tg;
    [SerializeField] private ColorTuple _colors;

    private GameObject _eyes;

    void Start()
    {
        _tg = GetComponent<Toggle>();

        if (!_tg)
        {
            Debug.Log("Failed to get Toggle");
        }

        _tg.onValueChanged.AddListener(OnToggleValueChanged);

        _eyes = CosmeticContainer.Instance.GetEyes();

        // Create a color block given the new colors
        ColorBlock buttonColors = new();
        buttonColors.colorMultiplier = 1.0f;
        buttonColors.disabledColor = (_colors.r / 4f) + new Color (0f, 0f, 0f, 1f);
        buttonColors.fadeDuration = 0.1f;
        buttonColors.highlightedColor = _colors.r + new Color(0.1f, 0.1f, 0.1f, 1f);
        buttonColors.normalColor = _colors.r * .85f;
        buttonColors.pressedColor = _colors.g;
        buttonColors.selectedColor = _colors.r;

        // Handle base color being black, fall back to secondary color
        if (_colors.r == Color.black)
        {
            buttonColors.highlightedColor = _colors.g + new Color(0.1f, 0.1f, 0.1f, 1f);
            buttonColors.normalColor = _colors.g;
            buttonColors.pressedColor = _colors.r;
            buttonColors.selectedColor = _colors.g;
        }

        // Assign the color block
            _tg.colors = buttonColors;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            AssignColorToMaterial();
        }
    }

    public void AssignColorToMaterial()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        Renderer renderer = _eyes.GetComponent<Renderer>();

        foreach (Material mat in renderer.materials)
        {
            if (mat.name.Contains("m_Ari_Eye"))
            {
                mat.SetColor("_TintR", _colors.r);
                mat.SetColor("_TintG", _colors.g);
                mat.SetColor("_TintB", _colors.b);
            }
        }
    }
}
