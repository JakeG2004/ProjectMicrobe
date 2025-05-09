using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenManager : MonoBehaviour
{
    private Canvas _canvas;
    private RectTransformData _worldSpaceVals;
    private RectTransform _rt;

    // Start is called before the first frame update
    void Start()
    {
        _canvas = GetComponent<Canvas>();    
        _rt = GetComponent<RectTransform>();
    }

    public void ToggleUISpace()
    {
        if(_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            SetWorldSpace();
            return;
        }

        SetScreenSpace();
    }

    public void SetScreenSpace()
    {
        if(_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return;
        }

        // Store the worldspace data to be returned to later
        _worldSpaceVals = new RectTransformData(_rt.localPosition, _rt.localScale, _rt.localRotation, _rt.sizeDelta);
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    public void SetWorldSpace()
    {
        if(_canvas.renderMode == RenderMode.WorldSpace)
        {
            return;
        }

        // Restore the worldspace data
        _rt.localPosition = _worldSpaceVals.localPos;
        _rt.localScale = _worldSpaceVals.localScale;
        _rt.localRotation = _worldSpaceVals.localRotation;
        _rt.sizeDelta = _worldSpaceVals.sizeDelta;
        _canvas.renderMode = RenderMode.WorldSpace;
    }
}

public class RectTransformData
{
    public Vector3 localPos;
    public Vector3 localScale;
    public Quaternion localRotation;
    public Vector3 sizeDelta;

    public RectTransformData(Vector3 pos, Vector3 scale, Quaternion rot, Vector3 sizeDelt)
    {
        localPos = pos;
        localScale = scale;
        localRotation = rot;
        sizeDelta = sizeDelt;
    }
}
