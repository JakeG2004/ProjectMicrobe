using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HairColorController : BaseToggleGroupController
{
    [SerializeField] private bool _isPrimary;
    private GameObject[] _hairStyles;
    private GameObject[] _eyebrowStyles;

    protected override void Start()
    {
        base.Start();

        _hairStyles = CosmeticContainer.Instance?.GetHairStyles();
        _eyebrowStyles = CosmeticContainer.Instance?.GetEyebrowStyles();
    }

    protected override void OnTurnOn()
    {
        AssignHairColors();

        if (_isPrimary)
        {
            AssignEyebrowColors();
        }
    }

    private void AssignHairColors()
    {
        foreach (GameObject hair in _hairStyles)
        {
            Renderer renderer = hair.GetComponent<Renderer>();
            Material mat = renderer.material;
            if (_isPrimary)
            {
                mat.SetColor("_TintR", _colorTuple.r);
                // Set highlight color to hair color b
                mat.SetColor("_HighlightColor", _colorTuple.b);
            }

            else
            {
                mat.SetColor("_TintG", _colorTuple.g);
            }
        }
    }

    private void AssignEyebrowColors()
    {
        foreach (GameObject eyebrow in _eyebrowStyles)
        {
            Renderer renderer = eyebrow.GetComponent<Renderer>();
            foreach(Material mat in renderer.materials)
            {
                if (mat.name.Contains("m_Ari_EyeBrow"))
                {
                    mat.SetColor("_TintR", _colorTuple.r);
                    mat.SetColor("_TintG", _colorTuple.g);
                    mat.SetColor("_HighlightColor", _colorTuple.b);
                }

                if (mat.name.Contains("m_Ari_Mustache"))
                {
                    mat.SetColor("_TintG", _colorTuple.r);
                    mat.SetColor("_TintB", _colorTuple.g);
                }
            }
        }
    }
}