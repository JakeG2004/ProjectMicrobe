using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    private Queue<Objective> _objQ;
    private Objective _curObj;
    private ObjectiveEntryScript _oes;

    // Start is called before the first frame update
    void Start()
    {
        _objQ = new();
    }

    // Adds a new objective to the queue
    public void AddObjective(Objective obj)
    {
        // Set as the current objective if this is the first item
        if (_objQ.Count == 0)
        {
            _curObj = obj;
        }

        _objQ.Enqueue(obj);
    }

    // Update the objective
    public void UpdateObjective()
    {
        _oes.Hide();

        // Handle empty queue
        if (_objQ.Count == 0)
        {
            return;
        }

        _curObj = _objQ.Dequeue();
        _oes.Hide();
        _oes.SwitchObjective(2, _curObj);
    }
}
