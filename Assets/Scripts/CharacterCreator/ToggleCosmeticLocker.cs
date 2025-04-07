using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCosmeticLocker : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] private bool _isLocked = false;
    private GameObject _lockIcon;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if(child.gameObject.name == "Lock")
            {
                _lockIcon = child.gameObject;
                break;
            }
        }

        _toggle = GetComponent<Toggle>();

        _toggle.interactable = !_isLocked;
        _lockIcon.SetActive(_isLocked);
    }
}
