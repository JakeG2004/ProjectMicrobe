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
    private List<FacePlayer> _fps = new();
    private Stack<FacePlayer> _currentInteractPrompts = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    public void AddInteract(FacePlayer fp)
    {
        _fps.Add(fp);
    }

    public void DisableOtherInteracts(FacePlayer curFp)
    {
        foreach (FacePlayer fp in _fps)
        {
            if (fp == curFp)
            {
                foreach (Transform child in fp.gameObject.transform.parent)
                {
                    InteractInSphere iip = child.gameObject.GetComponent<InteractInSphere>();
                    if (iip != null)
                    {
                        iip.SetInteractable(true);
                    }
                }
                continue;
            }

            fp.SetAnimState(false);
            foreach (Transform child in fp.gameObject.transform.parent)
            {
                InteractInSphere iip = child.gameObject.GetComponent<InteractInSphere>();
                if (iip != null)
                {
                    iip.SetInteractable(false);
                }
            }
        }
    }

    public void PushInteractable(FacePlayer curFP)
    {
        InteractInSphere iip;
        foreach (FacePlayer fp in _currentInteractPrompts)
        {
            iip = fp.gameObject.GetComponent<InteractInSphere>();
            if (iip != null)
            {
                iip.SetInteractable(false);
            }
        }

        _currentInteractPrompts.Push(curFP);
        iip = curFP.gameObject.GetComponent<InteractInSphere>();
        if (iip != null)
        {
            iip.SetInteractable(true);
        }
    }

    public void PopInteractable()
    {
        FacePlayer fp = _currentInteractPrompts.Pop();
        InteractInSphere iip = fp.gameObject.GetComponent<InteractInSphere>();
        if (iip != null)
        {
            iip.SetInteractable(false);
        }
    }
}
