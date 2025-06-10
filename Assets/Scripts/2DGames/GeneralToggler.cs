using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralToggler : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTrue;
    [SerializeField] private UnityEvent _onFalse;

    private bool _state = false;

    public void ToggleState()
    {
        SetState(!_state);
    }

    public void SetState(bool state)
    {
        _state = state;

        if (_state)
        {
            _onTrue.Invoke();
        }

        if (!_state)
        {
            _onFalse.Invoke();
        }
    }
}
