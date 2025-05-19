/*
* Script to create new entries in the Objectives Box
*
* Author:   Jake Gendreau
* Date:     5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveAdder : MonoBehaviour
{
    [SerializeField] private GameObject _objectivePrefab;

    // Creates an instance of the objective prefab, initializes it, and sets it as the child of this object
    public void AddObjective(Objective obj)
    {
        GameObject _newObj = Instantiate(_objectivePrefab, transform);
        _newObj.GetComponent<ObjectiveEntryScript>().InitEntry(obj);
    }
}
