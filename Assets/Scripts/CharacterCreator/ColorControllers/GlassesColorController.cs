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
    [SerializeField] private Color _color;
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

        _color.a = 1.0f;
        GetComponent<Image>().color = _color;
        _tg.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            AssignColorToMaterial();
            EnableGOs();
        }
    }

    public void AssignColorToMaterial()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        foreach (GameObject glasses in _glassesStyles)
        {
            Renderer renderer = glasses.GetComponent<Renderer>();
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_TintR", _color);
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
