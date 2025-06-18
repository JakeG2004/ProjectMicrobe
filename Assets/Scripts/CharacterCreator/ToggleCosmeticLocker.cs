// ToggleCosmeticLocker.cs
// A script for switching locked cosmetics on and off according to whether they're unlocked
// Author:  Jake Gendreau
// Date:    6/17/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCosmeticLocker : MonoBehaviour
{
    [SerializeField] private string cosmeticName;
    [SerializeField] private bool _isLocked = true;
    private GameObject _lockIcon;
    private Toggle _toggle;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Lock")
            {
                _lockIcon = child.gameObject;
                break;
            }
        }

        _toggle = GetComponent<Toggle>();

        SetLockState(_isLocked);
    }

    public string GetCosmeticName()
    {
        return cosmeticName;
    }

    public void SetLockState(bool state)
    {
        _isLocked = state;
        _toggle.interactable = !_isLocked;
        _lockIcon.SetActive(_isLocked);
    }
}
