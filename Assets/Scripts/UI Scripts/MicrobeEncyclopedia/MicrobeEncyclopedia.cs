using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MicrobeEncyclopedia : GeneralMenu
{
   
    [Space(10)]
    [SerializeField] private TMP_Text _exitButtonText;
    private Button _exitButton;

    protected override void Start()
    {
        base.Start();
        _exitButton = _exitButtonText.transform.parent.GetComponent<Button>();
    }

    public void ToggleMenuNormal()
    {
        _exitButtonText.text = "Exit";
        _exitButton.onClick.RemoveAllListeners();
        _exitButton.onClick.AddListener(ToggleMenu);

        ToggleMenu();
    }

    public void ToggleMenuTablet()
    {
        _exitButtonText.text = "Back";
        _exitButton.onClick.RemoveAllListeners();
        _exitButton.onClick.AddListener(ToggleMenuVisibility);

        ToggleMenu();
    }
}
