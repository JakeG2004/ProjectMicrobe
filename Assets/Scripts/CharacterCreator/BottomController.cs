/*using System.Collections;
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

        if(!_shirt)
        {
            Debug.Log("Failed to get shirt!");
        }

        if(_topType == TopType.ShirtAndJacket && !_jacket)
        {
            Debug.Log("Failed to get jacket");
        }

        if(_topType == TopType.ShirtAndLabCoat && !_coat)
        {
            Debug.Log("Failed to get coat!");
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            CosmeticContainer.Instance.DistableAllBottoms();
            AssignColors();
        }
    }

    public void AssignColors()
    {
        //AssignShoeColor();

        switch(_topType)
        {
            case BottomType.Shorts:
                //AssignShirtColors();
                _shirt.SetActive(true);
                break;
            case TopType.Pants:
                //AssignPantsColors();
                _jacket.SetActive(true);
                break;
        }
    }

    void AssignHairAccessoryColor()
    {
        foreach(var hair in CosmeticContainer.Instance.GetHairStyles())
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
        foreach(Material mat in renderer.materials)
        {
            if(mat.name.Contains("m_Ari_ClothShirt"))
            {
                mat.SetColor("_TintR", _shirtColors.r);
                mat.SetColor("_TintG", _shirtColors.g);
                mat.SetColor("_TintB", _shirtColors.b);
            }

            if(mat.name.Contains("m_Ari_ClothHoodie"))
            {
                mat.SetColor("_TintR", _jacketColors.r);
                mat.SetColor("_TintG", _jacketColors.g);
                mat.SetColor("_TintB", _jacketColors.b);
            }
        }
    }

    void AssignLabCoatColors()
    {
        Renderer renderer = _coat.GetComponent<Renderer>();
        foreach(Material mat in renderer.materials)
        {
            if(mat.name.Contains("m_Ari_ClothShirt"))
            {
                mat.SetColor("_TintR", _shirtColors.r);
                mat.SetColor("_TintG", _shirtColors.g);
                mat.SetColor("_TintB", _shirtColors.b);
            }

            if(mat.name.Contains("m_Ari_ClothLabCoat"))
            {
                Debug.Log("Test");
                mat.SetColor("_TintR", _coatColors.r);
                mat.SetColor("_TintG", _coatColors.g);
                mat.SetColor("_TintB", _coatColors.b);
            }
        }
    }
}*/
