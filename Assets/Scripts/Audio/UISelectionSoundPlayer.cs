// UISelectionSoundPlayer.cs
// A script to play menu sounds on selection automatically
// Author:  Jake Gendreau
// Date:    7/15/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelectionSoundPlayer : MonoBehaviour
{
    private GameObject _lastSelected;
    private HashSet<Slider> _slidersWithListeners = new HashSet<Slider>();
    private HashSet<Button> _buttonsWithListeners = new HashSet<Button>();

    void Update()
    {
        GameObject curSelected = EventSystem.current.currentSelectedGameObject;

        if (curSelected != null && curSelected != _lastSelected)
        {
            // Only play sound if GO has Selectable
            if (curSelected.GetComponent<Selectable>() != null)
            {
                AddSliderListener(curSelected.GetComponent<Slider>());
                AddButtonListener(curSelected.GetComponent<Button>());

                SoundManager.PlayRapidSound(SoundType.MENU_SELECTED);
            }

            _lastSelected = curSelected;
        }
    }

    void AddButtonListener(Button button)
    {
        if (button == null)
        {
            return;
        }

        if (_buttonsWithListeners.Contains(button))
        {
            return;
        }

        button.onClick.AddListener(() =>
        {
            SoundManager.PlayRapidSound(SoundType.MENU_CLICK);
        });
    }

    void AddSliderListener(Slider slider)
    {
        if (slider == null)
        {
            return;
        }
        
        if (_slidersWithListeners.Contains(slider))
        {
            return;
        }

        slider.onValueChanged.AddListener((value) =>
        {
            SoundManager.PlayRapidSound(SoundType.MENU_SLIDER);
        });

        _slidersWithListeners.Add(slider);
    }
}
