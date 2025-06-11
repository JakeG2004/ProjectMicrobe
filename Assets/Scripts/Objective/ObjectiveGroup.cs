// ObjectiveGroup.cs
// A script for synchronizing objectives with the save system
// Author:  Jake Gendreau
// Date:    6/11/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGroup : MonoBehaviour
{
    [SerializeField] private string _groupName;
    private List<string> _completedObjectives = new();
    private string _currentObjective;
    private bool _objGroupComplete = false;

    public void MarkComplete(string objectiveName)
    {
        // Prevent duplicates
        if (_completedObjectives.Contains(objectiveName))
        {
            return;
        }

        _completedObjectives.Add(objectiveName);
    }

    public void CompleteObjectives(ObjectiveGroupItem ogi)
    {
        List<string> objectiveProgress = ogi.completeObjectives;
        _currentObjective = ogi.currentObjective;
        _objGroupComplete = ogi.complete;

        // Early return
        if (_objGroupComplete)
        {
            foreach (Transform child in transform)
            {
                Objective objective = child.gameObject.GetComponent<Objective>();

                // Silently complete and activate the objective
                objective.SkipObjective();
            }

            return;
        }

        // Iterate through every element and silently activate and complete
        for (int i = 0; i < objectiveProgress.Count; i++)
        {
            string obj = objectiveProgress[i];

            // Find the corresponding child
            foreach (Transform child in transform)
            {
                if (child.name == obj)
                {
                    Objective objective = child.gameObject.GetComponent<Objective>();

                    // Silently complete and activate the objective
                    objective.SkipObjective();
                }
            }
        }

        // Find the current objective and activate it
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == _currentObjective)
            {
                Objective curObj = child.gameObject.GetComponent<Objective>();
                curObj.ActivateObjective();
            }
        }
    }

    public void SetGroupComplete(bool state)
    {
        _objGroupComplete = state;
    }

    public void SetCurrentObjective(string objName)
    {
        _currentObjective = objName;
    }

    public List<string> GetCompletedObjectives()
    {
        return _completedObjectives;
    }

    public string GetCurrentObjective()
    {
        return _currentObjective;
    }

    public bool IsComplete()
    {
        return _objGroupComplete;
    }

    public string GetName()
    {
        return _groupName;
    }
}
