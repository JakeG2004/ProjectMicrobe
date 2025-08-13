using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesSaveManager
{
    private SaveObject _currentState;

    public ObjectivesSaveManager(SaveObject state)
    {
        _currentState = state;
    }

    public void UpdateState(SaveObject state)
    {
        _currentState = state;
    }
    
    // Saves the objectives sorted into objective groups
    public void SaveObjectives()
    {
        // Get all of the objective groups
        ObjectiveGroup[] objGroups = Object.FindObjectsByType<ObjectiveGroup>(FindObjectsSortMode.None);

        // Iterate through every objective group
        foreach (ObjectiveGroup objGroup in objGroups)
        {
            bool foundGroup = false;

            // Try to find a matching existing entry, then update it
            foreach (ObjectiveGroupItem ogi in _currentState.objectives)
            {
                if (ogi.name == objGroup.GetName())
                {
                    foundGroup = true;

                    ogi.completeObjectives = objGroup.GetCompletedObjectives();
                    ogi.currentObjective = objGroup.GetCurrentObjective();
                    ogi.complete = objGroup.IsComplete();
                    break;
                }
            }

            if (foundGroup)
            {
                continue;
            }

            // Create a new entry and add it
            ObjectiveGroupItem newOGI = new();
            newOGI.name = objGroup.GetName();
            newOGI.completeObjectives = objGroup.GetCompletedObjectives();
            newOGI.currentObjective = objGroup.GetCurrentObjective();
            newOGI.complete = objGroup.IsComplete();

            _currentState.objectives.Add(newOGI);
        }
    }

    // Load the objectives based on their objective groups
    public void LoadObjectives()
    {
        // Get all of the objective groups
        ObjectiveGroup[] objGroups = Object.FindObjectsOfType<ObjectiveGroup>();
        System.Array.Sort(objGroups, (a, b) => a.GetGroupIdx().CompareTo(b.GetGroupIdx()));


        foreach (ObjectiveGroup objGroup in objGroups)
        {
            foreach (ObjectiveGroupItem ogi in _currentState.objectives)
            {
                if (objGroup.GetName() == ogi.name)
                {
                    objGroup.CompleteObjectives(ogi);
                }
            }
        }
    }
}
