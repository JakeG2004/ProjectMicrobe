/*
* Script to manage an Objective Entry (i.e. set its fields)
*
* Author:   Jake Gendreau
* Date:     5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveEntryScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _objText;
    [SerializeField] private TMP_Text _completedText;

    void Start()
    {
        //_objText.text = "";
        //_completedText.text = "";
    }

    public void SetObjText(string objText)
    {
        _objText.text = objText;
    }

    public void CompleteObjective()
    {
        _completedText.text = "X";
    }
}
