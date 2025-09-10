using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleScroller : MonoBehaviour, ISelectHandler, IPointerDownHandler
{
    [SerializeField] private ScrollToObject _sto;
    [SerializeField] private int _toggleGroupSetCount = 1;
    private int _index;
    private float _scrollVal = 0.0f;
    private Toggle _tg;

    void Awake()
    {
        _index = transform.GetSiblingIndex();
        _scrollVal = GetScrollValue();
        _tg = GetComponent<Toggle>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _sto.ScrollTo(_scrollVal);
    }

    // Listen for pointer down to prevent error where button would scroll away
    public void OnPointerDown(PointerEventData eventData)
    {
        _sto.ScrollTo(_scrollVal);

        // Set the toggle's state and trigger its value changed event if the toggle is interactable (not locked)
        if (_tg != null && !_tg.isOn && _tg.interactable)
        {
            _tg.isOn = true;
        }
    }

    private float GetScrollValue()
    {
        int group = _index / _toggleGroupSetCount;

        int total = transform.parent.childCount - _toggleGroupSetCount;
        return total == 0 ? 0 : (float)group / (total / ((float)_toggleGroupSetCount));
    }
}
