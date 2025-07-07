using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VerticalScrollSlave : MonoBehaviour, ISelectHandler
{
    [SerializeField] private GameObject _parentPanel;
    VerticalScrollerMaster _vsm;

    void Start()
    {
        _vsm = VerticalScrollerMaster.Instance;
        _vsm.AddItem(_parentPanel);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _vsm.ScrollTo(_parentPanel);
    }
}
