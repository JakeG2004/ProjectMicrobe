// BottomButtonCreator.cs
// A script for dynamically creating buttons for the bottoms
// Author:  Jake Gendreau
// Date:    5/29/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomButtonCreator : MonoBehaviour
{
    [SerializeField] private Image _pantsFade;
    [SerializeField] private Image _sock;
    [SerializeField] private Image _shoe;
    [SerializeField] private Image _shoeToe;
    [SerializeField] private Image _shoeSole;

    private Image _pants;
    private BottomController _bc;
    private ColorTuple _shortsColor;
    private ColorTuple _pantsColor;
    private ColorTuple _shoeColor;
    private BottomType _bottomType;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        _bc = GetComponent<BottomController>();
        _pants = GetComponent<Image>();

        // Get the colors
        _shortsColor = _bc.GetShortsColor();
        _pantsColor = _bc.GetPantsColor();
        _shoeColor = _bc.GetShoeColor();

        // Get bottom type
        _bottomType = _bc.GetBottomType();

        SetCosmeticColors();
        SetCosmeticActive();
    }

    void SetCosmeticColors()
    {
        // Set the alpha of the colors
        _shortsColor.SetAlpha(1.0f);
        _pantsColor.SetAlpha(1.0f);
        _shoeColor.SetAlpha(1.0f);

        // Set the shoes
        _shoe.color = _shoeColor.r;
        _shoeToe.color = _shoeColor.g;
        _shoeSole.color = _shoeColor.b;

        // Handle the pants
        if (_bottomType == BottomType.Pants)
        {
            _pants.color = _pantsColor.r;
            _pantsFade.color = _pantsColor.g;
            return;
        }

        // Handle the shorts
        if (_bottomType == BottomType.Shorts)
        {
            _pants.color = _shortsColor.r;
            _sock.color = _shortsColor.g;
            return;
        }
    }

    void SetCosmeticActive()
    {
        // Show the shoe
        _shoe.gameObject.SetActive(true);
        _shoeToe.gameObject.SetActive(true);
        _shoeSole.gameObject.SetActive(true);

        // Enable / disable other GOs
        switch (_bottomType)
        {
            case BottomType.Pants:
                _pantsFade.gameObject.SetActive(true);
                _sock.gameObject.SetActive(false);
                break;

            case BottomType.Shorts:
                _pantsFade.gameObject.SetActive(false);
                _sock.gameObject.SetActive(true);
                break;
        }
    }
}
