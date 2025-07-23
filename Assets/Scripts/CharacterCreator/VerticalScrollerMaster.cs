using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollerMaster : MonoBehaviour
{
    public static VerticalScrollerMaster Instance {get; private set; }
    private float _totalHeight = 0.0f;
    private Dictionary<GameObject, float> _menuItems = new();
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public void ScrollTo(GameObject go)
    {
        RectTransform target = go.GetComponent<RectTransform>();
        RectTransform content = _scrollRect.content;
        float contentHeight = content.rect.height;
        float viewportHeight = _scrollRect.viewport.rect.height;

        // Distance from top of content to the target's top
        float itemTop = -(target.anchoredPosition.y) - (target.rect.height / 2);
        itemTop -= _content.GetComponent<VerticalLayoutGroup>().padding.top;


        // Calculate scrollable height (how far the scroll can move)
        float scrollableHeight = contentHeight - viewportHeight;

        if (scrollableHeight <= 0)
        {
            _scrollRect.verticalNormalizedPosition = 1f; // Nothing to scroll
            return;
        }

        // 1 - (top distance / scrollable height) puts item at the top
        float normalizedPos = 1f - (itemTop / scrollableHeight);
        normalizedPos = Mathf.Clamp01(normalizedPos);

        StopAllCoroutines();
        StartCoroutine(SlowScroll(_scrollRect.verticalNormalizedPosition, normalizedPos));
    }

    private IEnumerator SlowScroll(float curScroll, float goalScroll)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            float scrollRatio = Mathf.Clamp01(elapsedTime / 0.1f);

            _scrollRect.verticalNormalizedPosition = curScroll + ((goalScroll - curScroll) * scrollRatio);
            yield return null;
        }

        _scrollRect.verticalNormalizedPosition = goalScroll;
    }

    public void AddItem(GameObject go)
    {
        float height = go.GetComponent<RectTransform>().sizeDelta.y;

        if (_menuItems.ContainsKey(go))
        {
            return;
        }

        _totalHeight += height;
        _menuItems.Add(go, height);
    }
}
