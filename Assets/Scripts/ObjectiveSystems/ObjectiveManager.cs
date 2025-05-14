using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectiveManager
{
    public Action<Objective> OnObjectiveAdded;

    public List<Objective> Objectives { get; } = new List<Objective>();
    private readonly Dictionary<string, List<Objective>> _objectiveMap = new Dictionary<string, List<Objective>>();

    // Adds objective to objective manager
    public void AddObjective(Objective objective)
    {
        Objectives.Add(objective);

        // If there is an event trigger for the added objective
        if(!string.IsNullOrEmpty(objective.EventTrigger))
        {
            // If the objective map does not contain the event trigger, add it
            if(!_objectiveMap.ContainsKey(objective.EventTrigger))
            {
                _objectiveMap.Add(objective.EventTrigger, new List<Objective>());
            }

            // Add the objective as an entry in the map for the event trigger
            _objectiveMap[objective.EventTrigger].Add(objective);
        }

        OnObjectiveAdded?.Invoke(objective);
    }

    // Adds progress to an objective
    public void AddProgress(string eventTrigger, int value)
    {
        // If the event trigger is not in the map, ignore
        if(!_objectiveMap.ContainsKey(eventTrigger))
        {
            return;
        }

        // Add progress to each objective that derives from the event trigger
        foreach(var objective in _objectiveMap[eventTrigger])
        {
            objective.AddProgress(value);
        }
    }
}
