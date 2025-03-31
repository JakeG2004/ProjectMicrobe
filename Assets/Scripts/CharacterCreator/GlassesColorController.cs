using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassesColorController : MonoBehaviour
{
    private Toggle _tg;
    private Color _color;

    [SerializeField] private GameObject[] _glassesStyles;

    void Start()
    {
        _glassesStyles = CosmeticContainer.Instance?.GetGlassesStyles();

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
            AssignColorToMaterial();
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
            foreach(Material mat in renderer.materials)
            {
                mat.SetColor("_TintR", _color);
            }
        }
    }
}
