using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomColorController : MonoBehaviour
{
    private Toggle _tg;
    private Color _color;

    [SerializeField] private GameObject[] _bottomStyles;

    void Start()
    {
        _bottomStyles = CosmeticContainer.Instance?.GetBottomStyles();

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

        foreach (GameObject bottom in _bottomStyles)
        {
            Renderer renderer = bottom.GetComponent<Renderer>();
            foreach(Material mat in renderer.materials)
            {
                if (mat.name.Contains("m_Ari_Cloth")) // Adjust with your actual material name
                {
                    mat.SetColor("_TintR", _color);

                    // Create a lighter version of the color by interpolating with white
                    Color lighterColor = Color.Lerp(_color, Color.white, 0.5f);
                    mat.SetColor("_TintG", lighterColor);
                }
            }
        }
    }
}
