using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VerticalScrollSlave : MonoBehaviour, ISelectHandler
{
    [SerializeField] private GameObject _parentPanel;
    [SerializeField] private VerticalScrollerMaster _vsm;

    void Start()
    {
        FindVerticalScrollMaster();

        if (_parentPanel == null)
        {
            _parentPanel = this.gameObject;
        }

        _vsm.AddItem(_parentPanel);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _vsm.ScrollTo(_parentPanel);
    }

    public void FindVerticalScrollMaster(int maxLevels = 5)
    {
        Transform current = transform.parent;
        int levels = 0;

        while (current != null && levels < maxLevels)
        {
            _vsm = current.GetComponent<VerticalScrollerMaster>();
            if (_vsm != null)
                return;

            current = current.parent;
            levels++;
        }

        // Optionally fall back to singleton or scene-wide search
        if (_vsm == null)
            _vsm = VerticalScrollerMaster.Instance ?? FindObjectOfType<VerticalScrollerMaster>();
    }
}
