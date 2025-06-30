using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDefaultNav : MonoBehaviour
{
    [SerializeField] private Selectable _primarySelectable;

    void OnEnable()
    {
        _primarySelectable.Select();
    }

    public void TriggerDefaultSelectable()
    {
        _primarySelectable.Select();
    }
}
