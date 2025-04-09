using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomController : MonoBehaviour
{
    private enum BottomType
    {
        Shorts,
        Pants
    }

    private Toggle _tg;

    [SerializeField] BottomType _bottomType = BottomType.Pants;
    [SerializeField] private GameObject _shorts;
    [SerializeField] private GameObject _pants;
    [SerializeField] private GameObject _shoes;

    [SerializeField] private ColorTuple _shortsColors;
    [SerializeField] private ColorTuple _pantsColors;
    [SerializeField] private ColorTuple _shoeColors;

    // Start is called before the first frame update
    void Start()
    {
        _tg = GetComponent<Toggle>();

        if (!_tg)
        {
            Debug.Log("Failed to get Toggle");
        }

        _tg.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            CosmeticContainer.Instance.DisableAllBottoms();
            AssignColors();
        }
    }

    public void AssignColors()
    {
        AssignShoeColor();

        switch(_bottomType)
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
        foreach(Material mat in renderer.materials)
        {
            if(mat.name.Contains("m_Ari_ClothShorts"))
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
}
