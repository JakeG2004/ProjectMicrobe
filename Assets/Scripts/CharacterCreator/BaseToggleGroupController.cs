// BaseToggleGroupController.cs
// A script to be inherited by other scripts to manage toggle groups and simplify
// Author:  Jake Gendreau
// Date:    7/2/25 (my birthday!)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseToggleGroupController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] protected ColorTupleSO _colorTuple;
    protected Toggle _tg;
    protected bool _isOn = false;
    protected GameObject _outline;

    protected virtual void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Outline")
            {
                _outline = child.gameObject;
                break;
            }
        }
        _tg = GetComponent<Toggle>();
    }

    protected virtual void Start()
    {
        SetColorBlock();

        _tg.onValueChanged.AddListener(OnToggleValueChanged);

        _outline.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _outline.SetActive(true);
        _outline.GetComponent<Image>().color = Color.white;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_isOn)
        {
            _outline.GetComponent<Image>().color = Color.yellow;
        }

        else
        {
            _outline.SetActive(false);
        }
    }

    protected virtual void SetColorBlock()
    {
        ColorBlock tgColors = new ColorBlock();
        tgColors.normalColor = _colorTuple.r;
        tgColors.selectedColor = _colorTuple.r;
        tgColors.disabledColor = _colorTuple.r;
        tgColors.highlightedColor = _colorTuple.g;
        tgColors.pressedColor = _colorTuple.g;
        tgColors.colorMultiplier = 1.0f;
        tgColors.fadeDuration = 0.1f;

        _tg.colors = tgColors;
    }

    protected virtual void OnToggleValueChanged(bool isOn)
    {
        _isOn = isOn;

        if (isOn)
        {
            OnTurnOn();
        }

        else
        {
            OnTurnOff();
            _outline.SetActive(false);
        }
    }

    protected virtual void OnTurnOn()
    {

    }

    protected virtual void OnTurnOff()
    {

    }
}
