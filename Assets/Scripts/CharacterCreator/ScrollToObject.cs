using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollToObject : MonoBehaviour
{
    public enum ScrollType
    {
        HORIZONTAL,
        VERTICAL,
    };

    [SerializeField] private ScrollType _scrollType = ScrollType.HORIZONTAL;
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
        yield return new WaitForSeconds(0.05f);
        Scrollbar scrollbar = _sr.horizontalScrollbar;

        if (_scrollType == ScrollType.VERTICAL)
        {
            scrollbar = _sr.verticalScrollbar;
            scrollVal = 1 - scrollVal;
        }

        float elapsedTime = 0.0f;
        float startVal = scrollbar.value;

        while (elapsedTime <= 0.05f)
        {
            elapsedTime += Time.deltaTime;
            float scrollRatio = elapsedTime / 0.05f;

            scrollbar.value = Mathf.Lerp(startVal, scrollVal, scrollRatio);
            yield return null;
        }

        scrollbar.value = scrollVal;
    }
}
