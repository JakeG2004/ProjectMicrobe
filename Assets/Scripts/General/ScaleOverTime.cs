// ScaleOverTime.cs
// A script for scaling objects over time
// Author:  Jake Gendreau
// Date:    6/12/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    [SerializeField] private bool _activateOnStart = true;
    [SerializeField] private Vector3 _initScale;
    [SerializeField] private Vector3 _endScale;
    [SerializeField] private float _growTime;

    void Start()
    {
        if (_activateOnStart)
        {
            StartScale();
        }
    }

    public void StartScale()
    {
        transform.localScale = _initScale;
        StartCoroutine(IScaleObject());
    }

    private IEnumerator IScaleObject()
    {
        float _elapsedTime = 0.0f;

        while (_elapsedTime < _growTime)
        {
            _elapsedTime += Time.deltaTime;
            float scaleRatio = _elapsedTime / _growTime;
            transform.localScale = Vector3.Lerp(_initScale, _endScale, scaleRatio);
            yield return null;
        }

        transform.localScale = _endScale;
    }
}
