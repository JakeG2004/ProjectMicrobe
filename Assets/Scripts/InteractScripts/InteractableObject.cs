// InteractableObject.cs
// A script which communicates to InteractableMaster.cs to control single instances of interactables
// Author:  Jake Gendreau
// Date:    6/26/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private string _interactText = "Press E";

    [Space(10)]
    [SerializeField] private UnityEvent _onInteract;

    public void Interact()
    {
        _onInteract.Invoke();
    }

    public string GetInteractText()
    {
        return _interactText;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            InteractMaster.Instance?.AddInteract(this);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            InteractMaster.Instance?.AddInteract(this);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            InteractMaster.Instance?.RemoveInteract(this);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            InteractMaster.Instance?.RemoveInteract(this);
        }
    }
}