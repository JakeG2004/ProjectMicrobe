using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopController : MonoBehaviour
{
    private enum TopType
    {
        Shirt,
        ShirtAndJacket,
        ShirtAndLabCoat
    }

    private Toggle _tg;

    [SerializeField] TopType _topType = TopType.Shirt;
    [SerializeField] private GameObject _shirt;
    [SerializeField] private GameObject _jacket;
    [SerializeField] private GameObject _coat;
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
            AssignColors();
        }
    }

    public void AssignColors()
    {
        AssignShirtColors();
        switch(_topType)
        {
            case TopType.ShirtAndJacket:
                AssignJacketColors();
                break;
            case TopType.ShirtAndLabCoat:
                AssignLabCoatColors();
                break;
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
        Renderer renderer = _jacket.GetComponent<Renderer>();
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
                mat.SetColor("_TintR", _coatColors.r);
                mat.SetColor("_TintG", _coatColors.g);
                mat.SetColor("_TintB", _coatColors.b);
            }
        }
    }
}

[System.Serializable]
public class ColorTuple
{
    [SerializeField] public Color r;
    [SerializeField] public Color g;
    [SerializeField] public Color b;
}
