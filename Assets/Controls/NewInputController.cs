using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewInputController : MonoBehaviour
{
    public static NewInputController Instance { get; private set; }
    private PlayerInputActions _pia;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }

        _pia = new();
        Set3DMode();
    }

    public PlayerInputActions GetPlayerInputActions()
    {
        return _pia;
    }

    public void Set3DMode()
    {
        _pia.Player.Enable();
    }

    public void SetMenuMode()
    {
        _pia.Player.Disable();
    }
}
