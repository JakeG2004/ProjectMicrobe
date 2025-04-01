using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinColorController : MonoBehaviour
{
    private Toggle _tg;
    private Color _color;

    [SerializeField] private GameObject[] _skinObjs;

    void Start()
    {
        _skinObjs = CosmeticContainer.Instance?.GetSkinObjects();

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

        foreach (GameObject so in _skinObjs)
        {
            Renderer renderer = so.GetComponent<Renderer>();
            foreach(Material mat in renderer.materials)
            {
                if(mat.name.Contains("m_Ari_Skin"))
                {
                    mat.SetColor("_TintR", _color);

                    // Create a lighter version of the color by interpolating with white
                    Color darkerColor = Color.Lerp(_color, Color.black, 0.25f);
                    mat.SetColor("_TintG", darkerColor);
                }

                if(mat.name.Contains("m_Ari_Mustache"))
                {
                    mat.SetColor("_TintR", _color);
                }
            }
        }
    }
}
