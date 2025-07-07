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
    [SerializeField] private InteractableStack<InteractableObject> _interactables = new();
    [SerializeField] private bool _isInteractable = false;
    bool _hasDialogue = false;

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
            // Get the current interactable
            _interactables.Pop();

            // Move to the next one if there are more
            if (_interactables.Count > 0)
            {
                _interactText.ShowText(_interactables.Peek());
                return;
            }

            // Otherwise, hide and turn off interaction
            _interactText.HideText();
            _isInteractable = false;
            return;
        }

        _interactables.Remove(io);
    }

    // Handle the interaction and tracking of objects
    void Update()
    {
        // Track moving interactable objects (placeable pylon, NPC's, etc...)
        if (_isInteractable)
        {
            InteractableObject io = _interactables.Peek();

            if (!io)
            {
                return;
            }

            _interactText.SetPos(_interactables.Peek().transform.position);

            // Disable if the gameobjcet is inactinve
            if (io.gameObject.activeInHierarchy == false)
            {
                RemoveInteract(io);
            }

        }
    }

    public void TryInteract()
    {
        if (!_isInteractable || _hasDialogue)
        {
            return;
        }

        InteractableObject io = _interactables.Peek();

        if (io == null)
        {
            return;
        }

        io.Interact();
    }

    public void SetInteractableState(bool state)
    {
        _isInteractable = state;
    }

    public void SetHasDialogue(bool state)
    {
        _hasDialogue = state;
    }
}
