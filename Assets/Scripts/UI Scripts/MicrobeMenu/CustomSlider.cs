using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private Image _bgImg;
    [SerializeField] private Image _fillImg;
    private float _bgWidth;

    void Start()
    {
        SetSliderFill(0);
        _bgWidth = _bgImg.GetComponent<RectTransform>().rect.width;
    }

    // Sets the fill on the normal 0 - 1
    public void SetSliderFill(float fillAmt)
    {
        RectTransform rt = _fillImg.GetComponent<RectTransform>();
        rt.offsetMax = new Vector2(-(_bgWidth * (1 - fillAmt)), rt.offsetMax.y);
    }
}
