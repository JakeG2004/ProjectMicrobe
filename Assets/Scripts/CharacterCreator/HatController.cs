// HatController.cs
// A script for controlling the hats
// Author:  Jake Gendreau
// Date:    6/17/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatController : MonoBehaviour
{
    [SerializeField] private string _hatName;
    [SerializeField] private CosmeticController _hairSlider;
    private Toggle _tg;
    private GameObject[] _hairStyles;
    private GameObject[] _hats;

    void Start()
    {
        _hairStyles = CosmeticContainer.Instance.GetHairStyles();
        _hats = CosmeticContainer.Instance.GetHats();

        _tg = GetComponent<Toggle>();
        if (!_tg)
        {
            Debug.Log("Failed to get toggle!");
        }

        _tg.onValueChanged.AddListener(OnToggleValueChanged);
    }

    // Hats take priority over hair, so turn all hair off
    private void OnToggleValueChanged(bool isOn)
    {
        // Earlly return and check for no hat, turn on hair
        if (isOn && _hatName == "")
        {
            foreach (GameObject hat in _hats)
            {
                hat.SetActive(false);
                _hairSlider.UpdateCosmetic(_hairSlider.GetVal());
            }

            return;
        }

        // Disable all hair
        foreach (GameObject hair in _hairStyles)
        {
            hair.SetActive(false);
        }

        // Turn all other hats off, turn on the target hat
        foreach (GameObject hat in _hats)
        {
            if (hat.name == _hatName)
            {
                if (isOn)
                {
                    hat.SetActive(true);
                }

                else
                {
                    hat.SetActive(false);
                }
            }
        }
    }

    public void TurnOffAllHats()
    {
        foreach (GameObject hat in _hats)
        {
            hat.SetActive(false);
        }
    }

    public void SyncInitialState()
    {
        OnToggleValueChanged(_tg.isOn);
    }
}
