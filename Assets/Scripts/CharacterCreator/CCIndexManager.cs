// CCIndexManager.cs
// A script for saving and loading the character creator indeces from the file
// Author:  Jake Gendreau
// Date:    6/16/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CCIndexManager : MonoBehaviour
{
    [SerializeField] private string _type;
    [SerializeField] private Slider _slider;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private UnityEvent<float> _onLoadEvent;

    // Returns the slider value
    public float GetSliderValue()
    {
        return _slider.value;
    }

    public void SetSliderValueNoNotify(float val)
    {
        _slider.SetValueWithoutNotify(val);
    }

    public void SetSliderValueWithNotify(float val)
    {
        _slider.value = val;
    }

    // Returns the index of the active toggle
    public int GetToggleGroupValue()
    {
        for (int i = 0; i < _toggleGroup.gameObject.transform.childCount; i++)
        {
            Toggle curChild = _toggleGroup.gameObject.transform.GetChild(i).gameObject.GetComponent<Toggle>();
            if (curChild.isOn)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetToggleGroupValue(int val)
    {
        for (int i = 0; i < _toggleGroup.gameObject.transform.childCount; i++)
        {
            Toggle curChild = _toggleGroup.gameObject.transform.GetChild(i).gameObject.GetComponent<Toggle>();

            curChild.isOn = false;

            if (i == val)
            {
                //curChild.isOn = true;
                curChild.SetIsOnWithoutNotify(true);

                // Find the outline, and highlight
                foreach (Transform child in curChild.transform)
                {
                    if (child.gameObject.name == "Outline")
                    {
                        GameObject outline = child.gameObject;
                        outline.SetActive(true);
                        outline.GetComponent<Image>().color = Color.yellow;
                    }
                }
            }
        }
    }

    public string GetValType()
    {
        return _type;
    }

    public void OnLoad(float val)
    {
        _onLoadEvent.Invoke(val);
    }
}
