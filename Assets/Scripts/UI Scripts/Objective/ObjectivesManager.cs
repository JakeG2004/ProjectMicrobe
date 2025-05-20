// ObjectivesManager.cs
// A script for managing new objectives as they are added
// Author:  Jake Gendreau
// Date:    5/20/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    private Queue<Objective> _objQ;
    private Objective _curObj;
    private bool _firstObjective = true;
    [SerializeField] private ObjectiveEntryScript _oes;

    // Start is called before the first frame update
    void Start()
    {
        _objQ = new();
    }

    // Adds a new objective to the queue
    public void AddObjective(Objective obj)
    {
        _curObj = obj;

        // Set as the current objective if this is the first item
        if (_firstObjective)
        {
            _oes.InitEntry(_curObj);
            _firstObjective = false;
        }

        else
        {
            UpdateObjective();
        }
    }

    // Update the objective
    public void UpdateObjective()
    {
        // Handle empty queue
        if (_firstObjective)
        {
            return;
        }

        _oes.SwitchObjective(.75f, _curObj);
    }
}
