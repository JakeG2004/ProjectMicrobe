// HatController.cs
// A script for controlling the hats
// Author:  Jake Gendreau
// Date:    6/17/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatController : BaseToggleGroupController
{
    [SerializeField] private string _hatName;
    [SerializeField] private CosmeticController _hairSlider;
    private GameObject[] _hairStyles;
    private GameObject[] _hats;

    protected override void Start()
    {
        base.Start();

        _hairStyles = CosmeticContainer.Instance.GetHairStyles();
        _hats = CosmeticContainer.Instance.GetHats();
    }

    // Hats take priority over hair, so turn all hair off
    protected override void OnToggleValueChanged(bool isOn)
    {
        base.OnToggleValueChanged(isOn);

        // Early return and check for no hat, turn on hair
        if (isOn && _hatName == "")
        {
            foreach (GameObject hat in _hats)
            {
                _hairSlider.UpdateCosmetic(_hairSlider.GetVal());
            }

            return;
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
                    hat.GetComponent<HatPositionManager>().ResetHat();
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
