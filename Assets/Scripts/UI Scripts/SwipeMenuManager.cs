using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] _scrollObjects;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    
    private int _curObject = 0;
    private float spacing = 0;
    private Coroutine slideCoroutine;

    void Start()
    {
        if (_scrollObjects.Length <= 0)
        {
            Debug.Log("0 scroll objects");
            return;
        }

        spacing = GetComponent<RectTransform>().rect.width;

        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            _scrollObjects[i].anchoredPosition = new Vector2(spacing * i, 0);
        }
    }

    public void NextMenu()
    {
        if (_curObject >= _scrollObjects.Length - 1)
            return;

        _curObject++;
        UpdateButtons();
        StartSlide(1);
    }

    public void PrevMenu()
    {
        if (_curObject <= 0)
            return;

        _curObject--;
        UpdateButtons();
        StartSlide(-1);
    }

    private void UpdateButtons()
    {
        _nextButton.interactable = (_curObject < _scrollObjects.Length - 1);
        _prevButton.interactable = (_curObject > 0);
    }

    private void StartSlide(int direction)
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideBetweenMenus(0.3f, direction));
    }

    IEnumerator SlideBetweenMenus(float time, int direction)
    {
        float elapsed = 0f;
        Vector2[] startPositions = new Vector2[_scrollObjects.Length];
        Vector2[] endPositions = new Vector2[_scrollObjects.Length];

        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            startPositions[i] = _scrollObjects[i].anchoredPosition;
            endPositions[i] = new Vector2(spacing * (i - _curObject), 0);
        }

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            // Apply direction-based easing
            float easedT = EaseInOutQuad(t);

            for (int i = 0; i < _scrollObjects.Length; i++)
            {
                _scrollObjects[i].anchoredPosition = Vector2.Lerp(startPositions[i], endPositions[i], easedT);
            }

            yield return null;
        }

        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            _scrollObjects[i].anchoredPosition = endPositions[i];
        }
    }

    private float EaseInOutQuad(float t)
    {
        return t * t;
    }
}
