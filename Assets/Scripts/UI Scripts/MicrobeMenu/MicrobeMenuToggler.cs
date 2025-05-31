using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeMenuToggler : MonoBehaviour
{
    private MicrobeMenu _microbeMenu;
    
    void Start()
    {
        _microbeMenu = GameObject.FindGameObjectWithTag("MicrobeMenu").GetComponent<MicrobeMenu>();
    }

    public void ToggleMicrobeMenu()
    {
        _microbeMenu.SetCurrentPylon(transform.parent.gameObject);
        _microbeMenu.ToggleState();
    }
}