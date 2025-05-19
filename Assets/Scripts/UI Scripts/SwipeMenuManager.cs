using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenuManager : MonoBehaviour
{
    public enum SlideType
    {
        Horizontal,
        Vertical
    };

    [SerializeField] private RectTransform[] _scrollObjects;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private SlideType _slideType = SlideType.Horizontal;

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

        if (_slideType == SlideType.Vertical)
        {
            spacing = GetComponent<RectTransform>().rect.height;

            for (int i = 0; i < _scrollObjects.Length; i++)
            {
                _scrollObjects[i].anchoredPosition = new Vector2(_scrollObjects[i].anchoredPosition.x, -(spacing * i));
            }
        }

        if (_slideType == SlideType.Horizontal)
        {
            spacing = GetComponent<RectTransform>().rect.width;

            for (int i = 0; i < _scrollObjects.Length; i++)
            {
                _scrollObjects[i].anchoredPosition = new Vector2(spacing * i, _scrollObjects[i].anchoredPosition.y);
            }
        }
    }

    // Go to the next menu
    public void NextMenu()
    {
        if (_curObject >= _scrollObjects.Length - 1)
            return;

        _curObject++;
        UpdateButtons();
        StartSlide(1);
    }

    // Go to the previous menu
    public void PrevMenu()
    {
        if (_curObject <= 0)
            return;

        _curObject--;
        UpdateButtons();
        StartSlide(-1);
    }

    // Show and hide buttons in accordance to current page
    private void UpdateButtons()
    {
        _nextButton.interactable = (_curObject < _scrollObjects.Length - 1);
        _prevButton.interactable = (_curObject > 0);
    }

    // Start a slide either to the left or to the right
    private void StartSlide(int direction)
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        if (_slideType == SlideType.Horizontal)
        {
            slideCoroutine = StartCoroutine(SlideBetweenMenusHorizontal(0.3f, direction));
        }

        if (_slideType == SlideType.Vertical)
        {
            slideCoroutine = StartCoroutine(SlideBetweenMenusVertical(0.3f, direction));
        }
    }

    // Smoothly slide between menu entries horizontally
    IEnumerator SlideBetweenMenusHorizontal(float time, int direction)
    {
        float elapsed = 0f;
        Vector2[] startPositions = new Vector2[_scrollObjects.Length];
        Vector2[] endPositions = new Vector2[_scrollObjects.Length];

        // Create the end positions for each entry in the slide menu
        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            startPositions[i] = _scrollObjects[i].anchoredPosition;
            endPositions[i] = new Vector2(spacing * (i - _curObject), startPositions[i].y);
        }

        // Do the slide for each of the entries
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            // Apply direction-based easing
            float easedT = EaseInOut(t);

            for (int i = 0; i < _scrollObjects.Length; i++)
            {
                _scrollObjects[i].anchoredPosition = Vector2.Lerp(startPositions[i], endPositions[i], easedT);
            }

            yield return null;
        }

        // Snap them all to their final destinations
        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            _scrollObjects[i].anchoredPosition = endPositions[i];
        }
    }

    // Smoothly slide between menu entries vertically
    IEnumerator SlideBetweenMenusVertical(float time, int direction)
    {
        float elapsed = 0f;
        Vector2[] startPositions = new Vector2[_scrollObjects.Length];
        Vector2[] endPositions = new Vector2[_scrollObjects.Length];

        // Create the end positions for each entry in the slide menu
        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            startPositions[i] = _scrollObjects[i].anchoredPosition;
            endPositions[i] = new Vector2(startPositions[i].x, -(spacing * (i - _curObject)));
        }

        // Do the slide for each of the entries
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            // Apply direction-based easing
            float easedT = EaseInOut(t);

            for (int i = 0; i < _scrollObjects.Length; i++)
            {
                _scrollObjects[i].anchoredPosition = Vector2.Lerp(startPositions[i], endPositions[i], easedT);
            }

            yield return null;
        }

        // Snap them all to their final destinations
        for (int i = 0; i < _scrollObjects.Length; i++)
        {
            _scrollObjects[i].anchoredPosition = endPositions[i];
        }
    }

    // Provide smoothing for the transitions
    private float EaseInOut(float t)
    {
        return t * t;
    }

    // Set the buttons to be active in accordance with the current page when enabled
    public void OnEnable()
    {
        UpdateButtons();
    }
}
