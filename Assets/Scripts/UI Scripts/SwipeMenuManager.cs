using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwipeMenuManager : MonoBehaviour
{
    public enum SlideType
    {
        Horizontal,
        Vertical
    };

    [SerializeField] private SwipeMenuNavManager _navMgr;

    [Space(10)]
    [SerializeField] private List<SwipeableObject> _scrollObjects = new();
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private SlideType _slideType = SlideType.Horizontal;

    private int _curObject = 0;
    private float spacing = 0;
    private Coroutine slideCoroutine;

    void Start()
    {
        if (_scrollObjects.Count <= 0)
        {
            Debug.Log("0 scroll objects");
            return;
        }

        if (_slideType == SlideType.Vertical)
        {
            spacing = GetComponent<RectTransform>().rect.height;

            for (int i = 0; i < _scrollObjects.Count; i++)
            {
                _scrollObjects[i].scrollObj.anchoredPosition = new Vector2(_scrollObjects[i].scrollObj.anchoredPosition.x, -(spacing * i));
            }
        }

        if (_slideType == SlideType.Horizontal)
        {
            spacing = GetComponent<RectTransform>().rect.width;

            for (int i = 0; i < _scrollObjects.Count; i++)
            {
                _scrollObjects[i].scrollObj.anchoredPosition = new Vector2(spacing * i, _scrollObjects[i].scrollObj.anchoredPosition.y);
            }
        }

        UpdateButtons();
    }

    // Go to the next menu
    public void NextMenu()
    {
        if (_curObject >= _scrollObjects.Count - 1)
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
        // Toggle interactibles
        _nextButton.interactable = _curObject < _scrollObjects.Count - 1;
        _prevButton.interactable = _curObject > 0;


        // Set previous button title
        string prevButtonName = "";
        if (_curObject > 0)
        {
            prevButtonName = _scrollObjects[_curObject - 1].scrollObj.gameObject.name;
        }

        // Set next button title
        string nextButtonName = "";
        if (_curObject < _scrollObjects.Count - 1)
        {
            nextButtonName = _scrollObjects[_curObject + 1].scrollObj.gameObject.name;
        }

        _prevButton.GetComponentInChildren<TMP_Text>().text = prevButtonName;
        _nextButton.GetComponentInChildren<TMP_Text>().text = nextButtonName;

        // Trigger the triggerable event (in case of weird navigation that isn't auto handled)
        if (_scrollObjects[_curObject].scrollObj.gameObject.TryGetComponent<TriggerableEvent>(out var te))
        {
            te.ActivateEvent();
        }

        // Get the expected selectables from the menu
        Selectable curMinSelectable = _scrollObjects[_curObject].minSelectable;
        Selectable curMaxSelectable = _scrollObjects[_curObject].maxSelectable;

        if (curMinSelectable == null && curMaxSelectable == null)
        {
            return;
        }

        // Set the selectables to be the same if one is null
        curMinSelectable = curMinSelectable == null ? curMaxSelectable : curMinSelectable;
        curMaxSelectable = curMaxSelectable == null ? curMinSelectable : curMaxSelectable;

        // Handle assignment in horizontal
        if (_slideType == SlideType.Horizontal)
        {
            Navigation nextNav = _nextButton.navigation;
            nextNav.selectOnLeft = curMaxSelectable;
            _nextButton.navigation = nextNav;

            Navigation prevNav = _prevButton.navigation;
            prevNav.selectOnRight = curMinSelectable;
            _prevButton.navigation = prevNav;
        }

        // Handle assignment in vertical
        else if (_slideType == SlideType.Vertical)
        {
            Navigation nextNav = _nextButton.navigation;
            nextNav.selectOnUp = curMaxSelectable;
            _nextButton.navigation = nextNav;

            Navigation prevNav = _prevButton.navigation;
            prevNav.selectOnDown = curMinSelectable;
            _prevButton.navigation = prevNav;
        }

        if (_navMgr != null)
        {
            _navMgr.SetOtherMenuNav(this, curMinSelectable);
        }
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

    public void ResetToInitialPosition()
    {
        _curObject = 0;

        Vector2[] startPositions = new Vector2[_scrollObjects.Count];
        Vector2[] endPositions = new Vector2[_scrollObjects.Count];

        // Create the end positions for each entry in the slide menu
        for (int i = 0; i < _scrollObjects.Count; i++)
        {
            startPositions[i] = _scrollObjects[i].scrollObj.anchoredPosition;

            if (_slideType == SlideType.Horizontal)
            {
                endPositions[i] = new Vector2(spacing * (i - _curObject), startPositions[i].y);
            }

            if (_slideType == SlideType.Vertical)
            {
                endPositions[i] = new Vector2(startPositions[i].x, -(spacing * (i - _curObject)));
            }
        }

        // Snap them all to their final destinations
        for (int i = 0; i < _scrollObjects.Count; i++)
        {
            _scrollObjects[i].scrollObj.anchoredPosition = endPositions[i];
        }
    }

    public void SetNav(Navigation newNav)
    {
        // Set the prev / next buttons
        Navigation prevNav = _prevButton.navigation;
        prevNav.selectOnDown = newNav.selectOnDown == null ? prevNav.selectOnDown : newNav.selectOnDown;
        prevNav.selectOnLeft = newNav.selectOnLeft == null ? prevNav.selectOnLeft : newNav.selectOnLeft;
        prevNav.selectOnRight = newNav.selectOnRight == null ? prevNav.selectOnRight : newNav.selectOnRight;
        prevNav.selectOnUp = newNav.selectOnUp == null ? prevNav.selectOnUp : newNav.selectOnUp;
        _prevButton.navigation = prevNav;

        Navigation nextNav = _nextButton.navigation;
        nextNav.selectOnDown = newNav.selectOnDown == null ? nextNav.selectOnDown : newNav.selectOnDown;
        nextNav.selectOnLeft = newNav.selectOnLeft == null ? nextNav.selectOnLeft : newNav.selectOnLeft;
        nextNav.selectOnRight = newNav.selectOnRight == null ? nextNav.selectOnRight : newNav.selectOnRight;
        nextNav.selectOnUp = newNav.selectOnUp == null ? nextNav.selectOnUp : newNav.selectOnUp;
        _nextButton.navigation = nextNav;

        foreach (SwipeableObject swipeObj in _scrollObjects)
        {
            // Set the minimum navigation
            if (swipeObj.minSelectable != null)
            {
                Navigation minNav = swipeObj.minSelectable.navigation;
                minNav.selectOnDown = newNav.selectOnDown == null ? minNav.selectOnDown : newNav.selectOnDown;
                minNav.selectOnLeft = newNav.selectOnLeft == null ? minNav.selectOnLeft : newNav.selectOnLeft;
                minNav.selectOnRight = newNav.selectOnRight == null ? minNav.selectOnRight : newNav.selectOnRight;
                minNav.selectOnUp = newNav.selectOnUp == null ? minNav.selectOnUp : newNav.selectOnUp;
                swipeObj.minSelectable.navigation = minNav;
            }

            // Set the maximum navigation
            if (swipeObj.maxSelectable != null)
            {
                Navigation maxNav = swipeObj.maxSelectable.navigation;
                maxNav.selectOnDown = newNav.selectOnDown == null ? maxNav.selectOnDown : newNav.selectOnDown;
                maxNav.selectOnLeft = newNav.selectOnLeft == null ? maxNav.selectOnLeft : newNav.selectOnLeft;
                maxNav.selectOnRight = newNav.selectOnRight == null ? maxNav.selectOnRight : newNav.selectOnRight;
                maxNav.selectOnUp = newNav.selectOnUp == null ? maxNav.selectOnUp : newNav.selectOnUp;
                swipeObj.maxSelectable.navigation = maxNav;
            }
        }
    }

    // Smoothly slide between menu entries horizontally
    IEnumerator SlideBetweenMenusHorizontal(float time, int direction)
    {
        float elapsed = 0f;
        Vector2[] startPositions = new Vector2[_scrollObjects.Count];
        Vector2[] endPositions = new Vector2[_scrollObjects.Count];

        // Create the end positions for each entry in the slide menu
        for (int i = 0; i < _scrollObjects.Count; i++)
        {
            startPositions[i] = _scrollObjects[i].scrollObj.anchoredPosition;
            endPositions[i] = new Vector2(spacing * (i - _curObject), startPositions[i].y);
        }

        // Do the slide for each of the entries
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            // Apply direction-based easing
            float easedT = EaseInOut(t);

            for (int i = 0; i < _scrollObjects.Count; i++)
            {
                _scrollObjects[i].scrollObj.anchoredPosition = Vector2.Lerp(startPositions[i], endPositions[i], easedT);
            }

            yield return null;
        }

        // Snap them all to their final destinations
        for (int i = 0; i < _scrollObjects.Count; i++)
        {
            _scrollObjects[i].scrollObj.anchoredPosition = endPositions[i];
        }

        UpdateButtons();
    }

    // Smoothly slide between menu entries vertically
    IEnumerator SlideBetweenMenusVertical(float time, int direction)
    {
        float elapsed = 0f;
        Vector2[] startPositions = new Vector2[_scrollObjects.Count];
        Vector2[] endPositions = new Vector2[_scrollObjects.Count];

        // Create the end positions for each entry in the slide menu
        for (int i = 0; i < _scrollObjects.Count; i++)
        {
            startPositions[i] = _scrollObjects[i].scrollObj.anchoredPosition;
            endPositions[i] = new Vector2(startPositions[i].x, -(spacing * (i - _curObject)));
        }

        // Do the slide for each of the entries
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            // Apply direction-based easing
            float easedT = EaseInOut(t);

            for (int i = 0; i < _scrollObjects.Count; i++)
            {
                _scrollObjects[i].scrollObj.anchoredPosition = Vector2.Lerp(startPositions[i], endPositions[i], easedT);
            }

            yield return null;
        }

        // Snap them all to their final destinations
        for (int i = 0; i < _scrollObjects.Count; i++)
        {
            _scrollObjects[i].scrollObj.anchoredPosition = endPositions[i];
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

    // Go back to the first slide whenever disabled
    public void OnDisable()
    {
        ResetToInitialPosition();
    }
}

[System.Serializable]
public class SwipeableObject
{
    public RectTransform scrollObj;
    public Selectable minSelectable;
    public Selectable maxSelectable;
}
