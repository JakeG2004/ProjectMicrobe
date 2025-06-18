// InteractMaster.cs
// A script for managing and referencing all interactible text prompts
// Author:  Jake Gendreau
// Date:    6/18/25

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMaster : MonoBehaviour
{
    public static InteractMaster Instance { get; private set; }
    private List<InteractInSphere> _iiss = new();

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

    public void AddInteract(InteractInSphere iis)
    {
        _iiss.Add(iis);
    }

    public void DisableOtherInteracts(InteractInSphere curiis)
    {
        foreach (InteractInSphere iis in _iiss)
        {
            if (iis == curiis)
            {
                continue;
            }

            iis.SetInteractable(false);
        }
    }
}*/
