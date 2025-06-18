// CustomSlider.cs
// A script for creating a custom slider that completely empties and completely fills
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private Image _bgImg;
    [SerializeField] private Image _fillImg;
    private float _bgWidth;
    private bool _isReady = false;

    void Start()
    {
        StartCoroutine(IInit());
    }

    private IEnumerator IInit()
    {
        yield return null;
        _bgWidth = _bgImg.GetComponent<RectTransform>().rect.width;
        _isReady = true;
        SetSliderFill(0);
    }

    public void SetSliderFill(float fillAmt)
    {
        if (!_isReady)
        {
            StartCoroutine(WaitAndSetFill(fillAmt));
            return;
        }

        fillAmt = Mathf.Clamp01(fillAmt);
        RectTransform rt = _fillImg.GetComponent<RectTransform>();
        rt.offsetMax = new Vector2(-(_bgWidth * (1 - fillAmt)), rt.offsetMax.y);
    }

    private IEnumerator WaitAndSetFill(float fillAmt)
    {
        // Wait until we're ready
        while (!_isReady) yield return null;
        SetSliderFill(fillAmt);
    }
}
