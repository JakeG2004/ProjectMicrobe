using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeColorController : MonoBehaviour
{
    private Toggle _tg;
    private Color _color;

    [SerializeField] private GameObject _eyes;

    void Start()
    {
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

        Renderer renderer = _eyes.GetComponent<Renderer>();

        foreach (Material mat in renderer.materials)
        {
            if (mat.name.Contains("m_Ari_Eye")) // Adjust with your actual material name
            {
                mat.SetColor("_TintR", _color);

                // Find the complementary color
                Color complementaryColor = GetComplementaryColor(_color);
                mat.SetColor("_TintG", complementaryColor);
            }
        }
    }

    // Function to calculate complementary color
    private Color GetComplementaryColor(Color color)
    {
        // Convert to HSV to manipulate the hue
        Color.RGBToHSV(color, out float h, out float s, out float v);

        // Add 180 degrees to the hue to find the complementary color
        h += 0.5f; // 180 degrees is 0.5 in the 0-1 range for HSV

        // Ensure hue is wrapped within the 0-1 range
        if (h > 1f)
        {
            h -= 1f;
        }

        // Convert back to RGB
        return Color.HSVToRGB(h, s, v);
    }
}
