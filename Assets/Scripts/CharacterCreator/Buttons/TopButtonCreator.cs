// TopButtonCreator.cs
// A script for dynamically decorating the clothing buttons
// Author:  Jake Gendreau
// Date:    5/29/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopButtonCreator : MonoBehaviour
{
    [SerializeField] private Image _shirtStripe;
    [SerializeField] private Image _shirtProtean;
    [SerializeField] private Image _half;
    [SerializeField] private Image _jacket;
    [SerializeField] private Image _labPocket;
    [SerializeField] private Image _labButton;

    private Image _bgImage;
    private TopController _tc;
    private ColorTuple _shirtColors;
    private ColorTuple _jacketColors;
    private ColorTuple _coatColors;
    public TopType _topType;

    // Start is called before the first frame update
    void Start()
    {
        // Get the components
        _tc = GetComponent<TopController>();
        _bgImage = GetComponent<Image>();

        // Get the colors
        _shirtColors = _tc.GetShirtColors();
        _jacketColors = _tc.GetJacketColors();
        _coatColors = _tc.GetCoatColors();

        // Get the top type
        _topType = _tc.GetTopType();

        SetCosmeticColors();
        SetCosmeticActive();
    }

    // Sets the colors for all of the objects
    void SetCosmeticColors()
    {
        _shirtColors.SetAlpha(1.0f);
        _jacketColors.SetAlpha(1.0f);
        _coatColors.SetAlpha(1.0f);

        _bgImage.color = _shirtColors.r;
        _shirtStripe.color = _shirtColors.g;

        Color proteanColor = _shirtColors.b;
        proteanColor.a = _shirtColors.GetProteanAlpha();
        _shirtProtean.color = proteanColor;

        // Early return if just a shirt
        if (_topType == TopType.Shirt)
        {
            return;
        }

        // Handle jacket
        if (_topType == TopType.ShirtAndJacket)
        {
            _half.color = _jacketColors.r;
            _jacket.color = _jacketColors.g;

            // Early return
            return;
        }

        // Handle coat
        if (_topType == TopType.ShirtAndLabCoat)
        {
            _half.color = _coatColors.r;
            _labPocket.color = _coatColors.g;
            _labButton.color = _coatColors.b;
        }
    }

    // Turns GO's on and off in accordance with what should be on and off
    void SetCosmeticActive()
    {
        switch (_topType)
        {
            case TopType.Shirt:
                _half.gameObject.SetActive(false);
                _jacket.gameObject.SetActive(false);
                _labPocket.gameObject.SetActive(false);
                _labButton.gameObject.SetActive(false);
                break;

            case TopType.ShirtAndJacket:
                _half.gameObject.SetActive(true);
                _jacket.gameObject.SetActive(true);
                _labPocket.gameObject.SetActive(false);
                _labButton.gameObject.SetActive(false);
                break;

            case TopType.ShirtAndLabCoat:
                _half.gameObject.SetActive(true);
                _jacket.gameObject.SetActive(false);
                _labPocket.gameObject.SetActive(true);
                _labButton.gameObject.SetActive(true);
                break;
        }
    }
}
