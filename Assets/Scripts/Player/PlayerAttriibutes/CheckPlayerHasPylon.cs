// CheckPlayerHasPylon.cs
// A script for checking whether or not the player has a pylon,
// then running an event if so
// Author:  Jake Gendreau
// Date:    6/2/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPlayerHasPylon : MonoBehaviour
{
    [SerializeField] private UnityEvent _hasPylonEvent;
    [SerializeField] private UnityEvent _notHasPylonEvent;

    public void StartCheckPylon()
    {
        StartCoroutine(WaitAndCheck());
    }

    public void CheckPylon()
    {
        bool hasPylon = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedPylon>().HasPylon();

        if (hasPylon)
        {
            _hasPylonEvent.Invoke();
            return;
        }

        _notHasPylonEvent.Invoke();
    }

    IEnumerator WaitAndCheck()
    {
        // Wait one frame
        yield return null;

        CheckPylon();
    }
}
