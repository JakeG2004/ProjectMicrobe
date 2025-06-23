using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuOpenChecker : MonoBehaviour
{
    [SerializeField] private UnityEvent _onMenuOpenEvent;
    private bool _menuCanOpen = true;

    public void SetMenuState(bool state)
    {
        _menuCanOpen = state;
    }

    public bool CanOpenMenu()
    {
        return _menuCanOpen;
    }

    public void AttemptToOpenMenu()
    {
        if (!_menuCanOpen)
        {
            return;
        }

        _onMenuOpenEvent.Invoke();
    }
}
