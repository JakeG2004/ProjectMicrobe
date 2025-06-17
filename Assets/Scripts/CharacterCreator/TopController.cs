using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopController : MonoBehaviour
{
    private Toggle _tg;

    [SerializeField] TopType _topType = TopType.Shirt;
    private GameObject _shirt;
    private GameObject _jacket;
    private GameObject _coat;
    [SerializeField] private Color _hairAccessoryColor;
    [SerializeField] private ColorTuple _shirtColors;
    [SerializeField] private ColorTuple _jacketColors;
    [SerializeField] private ColorTuple _coatColors;

    // Start is called before the first frame update
    void Start()
    {
        _tg = GetComponent<Toggle>();

        if (!_tg)
        {
            Debug.Log("Failed to get Toggle");
        }

        _tg.onValueChanged.AddListener(OnToggleValueChanged);

        _shirtColors.SetAlpha(1.0f);
        _jacketColors.SetAlpha(1.0f);
        _coatColors.SetAlpha(1.0f);

        foreach (GameObject cosmetic in CosmeticContainer.Instance.GetTopStyles())
        {
            if (cosmetic.name == "Shirt")
            {
                _shirt = cosmetic;
                continue;
            }

            if (cosmetic.name == "Hoodie")
            {
                _jacket = cosmetic;
                continue;
            }

            if (cosmetic.name == "LabCoat")
            {
                _coat = cosmetic;
                continue;
            }
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            CosmeticContainer.Instance.DistableAllTops();
            AssignColors();
        }
    }

    public void AssignColors()
    {
        AssignHairAccessoryColor();

        switch (_topType)
        {
            case TopType.Shirt:
                AssignShirtColors();
                _shirt.SetActive(true);
                break;
            case TopType.ShirtAndJacket:
                AssignJacketColors();
                _jacket.SetActive(true);
                break;
            case TopType.ShirtAndLabCoat:
                AssignLabCoatColors();
                _coat.SetActive(true);
                break;
        }
    }

    void AssignHairAccessoryColor()
    {
        foreach (var hair in CosmeticContainer.Instance.GetHairStyles())
        {
            Renderer renderer = hair.GetComponent<Renderer>();
            renderer.material.SetColor("_TintB", _hairAccessoryColor);
        }
    }

    void AssignShirtColors()
    {
        Renderer renderer = _shirt.GetComponent<Renderer>();
        renderer.material.SetColor("_TintR", _shirtColors.r);
        renderer.material.SetColor("_TintG", _shirtColors.g);
        renderer.material.SetColor("_TintB", _shirtColors.b);
    }

    void AssignJacketColors()
    {
        Renderer renderer = _jacket.GetComponent<Renderer>();
        foreach (Material mat in renderer.materials)
        {
            if (mat.name.Contains("m_Ari_ClothShirt"))
            {
                mat.SetColor("_TintR", _shirtColors.r);
                mat.SetColor("_TintG", _shirtColors.g);
                mat.SetColor("_TintB", _shirtColors.b);
                continue;
            }

            if (mat.name.Contains("m_Ari_ClothHoodie"))
            {
                mat.SetColor("_TintR", _jacketColors.r);
                mat.SetColor("_TintG", _jacketColors.g);
                mat.SetColor("_TintB", _jacketColors.b);
                continue;
            }
        }
    }

    void AssignLabCoatColors()
    {
        Renderer renderer = _coat.GetComponent<Renderer>();
        foreach (Material mat in renderer.materials)
        {
            if (mat.name.Contains("m_Ari_ClothShirt"))
            {
                mat.SetColor("_TintR", _shirtColors.r);
                mat.SetColor("_TintG", _shirtColors.g);
                mat.SetColor("_TintB", _shirtColors.b);
            }

            if (mat.name.Contains("m_Ari_ClothLabCoat"))
            {
                mat.SetColor("_TintR", _coatColors.r);
                mat.SetColor("_TintG", _coatColors.g);
                mat.SetColor("_TintB", _coatColors.b);
            }
        }
    }

    public ColorTuple GetShirtColors()
    {
        return _shirtColors;
    }

    public ColorTuple GetJacketColors()
    {
        return _jacketColors;
    }

    public ColorTuple GetCoatColors()
    {
        return _coatColors;
    }

    public TopType GetTopType()
    {
        return _topType;
    }

    public void SetHairAccessoryColor(Color color)
    {
        _hairAccessoryColor = color;
    }
}

public enum TopType
{
    Shirt,
    ShirtAndJacket,
    ShirtAndLabCoat
}

[System.Serializable]
public class ColorTuple
{
    [SerializeField] public Color r;
    [SerializeField] public Color g;
    [SerializeField] public Color b;

    public void SetAlpha(float alpha)
    {
        r.a = Mathf.Clamp01(alpha);
        g.a = Mathf.Clamp01(alpha);
        b.a = Mathf.Clamp01(alpha);
    }

    public float GetProteanAlpha()
    {
        // Weighted brightness to reflect human perception
        return Mathf.Clamp01(0.299f * b.r + 0.587f * b.g + 0.114f * b.b);
    }

}
