using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HairColorController : MonoBehaviour
{
    private Toggle _tg;
    private Color _color;

    [SerializeField] private GameObject[] _hairStyles;

    [SerializeField] private bool _isPrimary;

    void Start()
    {
        _hairStyles = CosmeticContainer.Instance?.GetHairStyles();

        _tg = GetComponent<Toggle>();

        if (!_tg)
        {
            Debug.Log("Failed to get Toggle");
        }

        _color = _tg.colors.normalColor;
        _tg.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            if(_isPrimary)
            {
                AssignPrimaryColorToMaterial();
            }

            else
            {
                AssignSecondaryColorToMaterial();
            }
        }
    }

    public void AssignPrimaryColorToMaterial()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        foreach (GameObject hair in _hairStyles)
        {
            Renderer renderer = hair.GetComponent<Renderer>();
            Material mat = renderer.material;
            mat.SetColor("_TintR", _color);
        }
    }

    public void AssignSecondaryColorToMaterial()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        foreach (GameObject hair in _hairStyles)
        {
            Renderer renderer = hair.GetComponent<Renderer>();
            Material mat = renderer.material;
            mat.SetColor("_TintG", _color);
        }
    }
}
