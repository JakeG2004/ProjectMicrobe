using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMicrobeToPlayerMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _menuPanel.SetActive(false);
    }

    public void ToggleState()
    {
        _isActive = !_isActive;
        _menuPanel.gameObject.SetActive(_isActive);
    }
}
