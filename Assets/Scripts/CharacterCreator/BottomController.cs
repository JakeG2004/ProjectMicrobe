using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomController : BaseToggleGroupController
{
    private GameObject _shorts;
    private GameObject _pants;
    private GameObject _shoes;
    private ColorTuple _shortsColors = new();
    [SerializeField] private ColorTupleSO _pantsColors;
    [SerializeField] private ColorTupleSO _shoeColors;
    [SerializeField] BottomType _bottomType = BottomType.Pants;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _shortsColors.r = _colorTuple.r;
        _shortsColors.g = _colorTuple.g;
        _shortsColors.b = _colorTuple.b;

        foreach (GameObject cosmetic in CosmeticContainer.Instance.GetBottomStyles())
        {
            if (cosmetic.name == "Shorts")
            {
                _shorts = cosmetic;
                continue;
            }

            if (cosmetic.name == "Pants")
            {
                _pants = cosmetic;
                continue;
            }

            if (cosmetic.name == "Shoes")
            {
                _shoes = cosmetic;
                continue;
            }
        }
    }

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
        CosmeticContainer.Instance.DisableAllBottoms();
        AssignColors();
    }

    public void AssignColors()
    {
        AssignShoeColor();

        switch (_bottomType)
        {
            case BottomType.Shorts:
                AssignShortsColors();
                _shorts.SetActive(true);
                break;
            case BottomType.Pants:
                AssignPantsColors();
                _pants.SetActive(true);
                break;
        }
    }

    void AssignShoeColor()
    {
        Renderer renderer = _shoes.GetComponent<Renderer>();
        renderer.material.SetColor("_TintR", _shoeColors.r);
        renderer.material.SetColor("_TintG", _shoeColors.g);
        renderer.material.SetColor("_TintB", _shoeColors.b);
    }

    void AssignShortsColors()
    {
        Renderer renderer = _shorts.GetComponent<Renderer>();
        foreach (Material mat in renderer.materials)
        {
            if (mat.name.Contains("m_Ari_ClothShorts"))
            {
                mat.SetColor("_TintR", _shortsColors.r);
                mat.SetColor("_TintG", _shortsColors.g);
                mat.SetColor("_TintB", _shortsColors.b);
            }
        }
    }

    void AssignPantsColors()
    {
        Renderer renderer = _pants.GetComponent<Renderer>();
        Material mat = renderer.material;
        mat.SetColor("_TintR", _pantsColors.r);
        mat.SetColor("_TintG", _pantsColors.g);
        mat.SetColor("_TintB", _pantsColors.b);
    }

    public ColorTuple GetShoeColor()
    {
        ColorTuple ct = new();
        ct.r = _shoeColors.r;
        ct.g = _shoeColors.g;
        ct.b = _shoeColors.b;

        return ct;
    }

    public ColorTuple GetPantsColor()
    {
        ColorTuple ct = new();
        ct.r = _pantsColors.r;
        ct.g = _pantsColors.g;
        ct.b = _pantsColors.b;

        return ct;
    }

    public ColorTuple GetShortsColor()
    {
        ColorTuple ct = new();
        ct.r = _colorTuple.r;
        ct.g = _colorTuple.g;
        ct.b = _colorTuple.b;

        return ct;
    }

    public BottomType GetBottomType()
    {
        return _bottomType;
    }
}

public enum BottomType
{
    Shorts,
    Pants
}
