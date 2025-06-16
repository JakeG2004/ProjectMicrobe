// CCIndexManager.cs
// A script for saving and loading the character creator indeces from the file
// Author:  Jake Gendreau
// Date:    6/16/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCIndexManager : MonoBehaviour
{
    [SerializeField] private string _type;
    [SerializeField] private Slider _slider;
    [SerializeField] private ToggleGroup _toggleGroup;

    // Returns the slider value
    public float GetSliderValue()
    {
        return _slider.value;
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

    public Vector2 GetOutfitToggleGroupValue()
    {
        //int numCostumes = transform.childCount;
        //for (int i = 0; i < transform.childCount; i++)
    }

    public string GetValType()
    {
        return _type;
    }
}
