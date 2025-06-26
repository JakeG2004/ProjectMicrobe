// InteractMaster.cs
// A script for managing and referencing all interactible text prompts
// Uses a stack to manage which one is currently active
// Author:  Jake Gendreau
// Date:    6/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMaster : MonoBehaviour
{
    public static InteractMaster Instance { get; private set; }
    [SerializeField] private InteractText _interactText;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    private InteractableStack<InteractableObject> _interactables = new();
    private bool _isInteractable = false;


    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public void AddInteract(InteractableObject io)
    {
        _interactables.Push(io);
        _interactText.ShowText(io);
        _isInteractable = true;
    }

    public void RemoveInteract(InteractableObject io)
    {
        if (_interactables.Count == 0 || !_interactables.Contains(io))
        {
            return;
        }

        // If current objective, hide it and show the next one;
        if (_interactables.Peek() == io)
        {
            _interactables.Pop();

            if (_interactables.Count > 0)
            {
                _interactText.ShowText(_interactables.Peek());
                return;
            }

            _interactText.HideText();
            _isInteractable = false;
            return;
        }

        _interactables.Remove(io);
    }

    // Handle the interaction using interrupt instead of polling (hopefully better)
    void Update()
    {
        if (Input.GetKeyDown(_interactKey) && _isInteractable)
        {
            InteractableObject io = _interactables.Peek();
            io.Interact();
            
            if (io.gameObject.activeSelf == false)
            {
                RemoveInteract(io);
            }
        }
    }
}
