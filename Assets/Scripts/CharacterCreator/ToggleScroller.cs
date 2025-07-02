using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleScroller : MonoBehaviour, ISelectHandler
{
    [SerializeField] private ScrollToObject _sto;
    [SerializeField] private int _toggleGroupSetCount = 1;
    private int _index;
    private float _scrollVal = 0.0f;

    void Awake()
    {
        _index = transform.GetSiblingIndex();
        _scrollVal = GetScrollValue();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _sto.ScrollTo(_scrollVal);
    }

    private float GetScrollValue()
    {
        int group = _index / _toggleGroupSetCount;

        int total = transform.parent.childCount - _toggleGroupSetCount;
        return total == 0 ? 0 : (float)group / (total / ((float)_toggleGroupSetCount));
    }
}
