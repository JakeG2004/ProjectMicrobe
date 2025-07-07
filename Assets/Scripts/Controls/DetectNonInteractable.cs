using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Selectable))]
public class DetectNonInteractable : MonoBehaviour
{
    [SerializeField] private UnityEvent _onNonInteractable;
    [SerializeField] private UnityEvent _onInteractable;
    private Selectable _selectable;
    private bool _isInteractable;

    void Awake()
    {
        _selectable = GetComponent<Selectable>();
    }

    void Start()
    {
        _isInteractable = _selectable.interactable;
    }

    void Update()
    {
        if (_selectable.interactable == false && _isInteractable)
        {
            _isInteractable = false;
            _onNonInteractable.Invoke();
        }

        if (_selectable.interactable == true && !_isInteractable)
        {
            _isInteractable = true;
            _onInteractable.Invoke();
        }
    }
}
