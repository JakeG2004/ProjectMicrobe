using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopController : BaseToggleGroupController
{
    private GameObject _shirt;
    private GameObject _jacket;
    private GameObject _coat;
    [SerializeField] private ColorTupleSO _jacketColors;
    [SerializeField] private ColorTupleSO _coatColors;
    [SerializeField] private Color _hairAccessoryColor;
    [SerializeField] TopType _topType = TopType.Shirt;
    private ColorTupleSO _shirtColors;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _shirtColors = _colorTuple;

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

        _hairAccessoryColor = _shirtColors.r;
    }

    // Override with nothing so that the colors dont get too wonky
    protected override void SetColorBlock()
    {
        ColorBlock tgColors = new ColorBlock();
        tgColors.normalColor = Color.white;
        tgColors.selectedColor = Color.white;
        tgColors.disabledColor = Color.white;
        tgColors.highlightedColor = Color.white;
        tgColors.pressedColor = Color.white;
        tgColors.colorMultiplier = 1.0f;
        tgColors.fadeDuration = 0.1f;

        _tg.colors = tgColors;
    }

    protected override void OnTurnOn()
    {
        CosmeticContainer.Instance.DistableAllTops();
        AssignColors();
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
        ColorTuple ct = new ColorTuple();
        ct.r = _colorTuple.r;
        ct.g = _colorTuple.g;
        ct.b = _colorTuple.b;

        return ct;
    }

    public ColorTuple GetJacketColors()
    {
        ColorTuple ct = new ColorTuple();
        ct.r = _jacketColors.r;
        ct.g = _jacketColors.g;
        ct.b = _jacketColors.b;

        return ct;
    }

    public ColorTuple GetCoatColors()
    {
        ColorTuple ct = new ColorTuple();
        ct.r = _coatColors.r;
        ct.g = _coatColors.g;
        ct.b = _coatColors.b;

        return ct;
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
