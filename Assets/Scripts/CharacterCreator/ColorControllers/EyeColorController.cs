using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeColorController : BaseToggleGroupController
{
    private GameObject _eyes;

    protected override void Start()
    {
        base.Start();

        _eyes = CosmeticContainer.Instance.GetEyes();
    }

    protected override void SetColorBlock()
    {
        // Create a color block given the new colors
        ColorBlock buttonColors = new();
        buttonColors.colorMultiplier = 1.0f;
        buttonColors.disabledColor = (_colorTuple.r / 4f) + new Color(0f, 0f, 0f, 1f);
        buttonColors.fadeDuration = 0.1f;
        buttonColors.highlightedColor = _colorTuple.r + new Color(0.1f, 0.1f, 0.1f, 1f);
        buttonColors.normalColor = _colorTuple.r;
        buttonColors.pressedColor = _colorTuple.g;
        buttonColors.selectedColor = _colorTuple.r;

        // Handle base color being black, fall back to secondary color
        if (_colorTuple.r == Color.black)
        {
            buttonColors.highlightedColor = _colorTuple.g + new Color(0.1f, 0.1f, 0.1f, 1f);
            buttonColors.normalColor = _colorTuple.g;
            buttonColors.pressedColor = _colorTuple.r;
            buttonColors.selectedColor = _colorTuple.g;
        }

        _tg.colors = buttonColors;
    }

    protected override void OnTurnOn()
    {
        AssignColorToMaterial();
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
                mat.SetColor("_TintR", _colorTuple.r);
                mat.SetColor("_TintG", _colorTuple.g);
                mat.SetColor("_TintB", _colorTuple.b);
            }
        }
    }
}
