using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveAdder : MonoBehaviour
{
    [SerializeField] private GameObject _objectivePrefab;

    public void AddObjective(Objective obj)
    {
        GameObject _newObj = Instantiate(_objectivePrefab);
        _newObj.GetComponent<ObjectiveEntryScript>().SetObjText(obj.GetObjectiveText());
        _newObj.transform.SetParent(this.transform);
    }
}
