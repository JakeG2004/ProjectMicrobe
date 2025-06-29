using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollToObject : MonoBehaviour
{
    private ScrollRect _sr;

    void Awake()
    {
        _sr = GetComponent<ScrollRect>();
    }

    public void ScrollTo(float scrollVal)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothScroll(scrollVal));
    }

    private IEnumerator SmoothScroll(float scrollVal)
    {
        float elapsedTime = 0.0f;
        float startVal = _sr.horizontalScrollbar.value;

        while(elapsedTime <= 0.05f)
        {
            elapsedTime += Time.deltaTime;
            float scrollRatio = elapsedTime / 0.05f;

            _sr.horizontalScrollbar.value = Mathf.Lerp(startVal, scrollVal, scrollRatio);
            yield return null;
        }

        _sr.horizontalScrollbar.value = scrollVal;
    }
}
