using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedMicrobes : MonoBehaviour
{
    const int MAX_MICROBES = 3;
    [SerializeField] private List<StringFloatPair> _microbes = new List<StringFloatPair>();
    [HideInInspector] public bool backpackFull = false;

    // Adds a new microbe to the player's backpack, handing amounts and duplicate entries
    public void AddMicrobe(StringFloatPair newMicrobe)
    {
        // Check whether the microbe being added is new or not
        bool addingNewMicrobe = true;
        foreach (StringFloatPair microbe in _microbes)
        {
            if (newMicrobe.name == microbe.name)
            {
                addingNewMicrobe = false;
            }
        }

        // Handle the case of too many microbes and adding a new one
        if (_microbes.Count >= MAX_MICROBES && addingNewMicrobe)
        {
            backpackFull = true;

            if (NotificationPanelManager.Instance.IsAnimating())
            {
                return;
            }

            NotificationPanelManager.Instance.ShowPanelForSeconds("Backpack Full");
            return;
        }

        // Ensure no duplicates (add population to existing pop)
        foreach (StringFloatPair microbe in _microbes)
        {
            if (microbe.name == newMicrobe.name)
            {
                microbe.amount += newMicrobe.amount;
                return;
            }
        }

        // Add the microbe as usual
        _microbes.Add(newMicrobe);
    }

    // Removes a microbe from the backpack, handlng the empty list
    public void RemoveMicrobe(string microbeName)
    {
        // Handle empty list
        if (_microbes.Count <= 0)
        {
            return;
        }

        // Find the target microbe and remove it from the list
        foreach (StringFloatPair microbe in _microbes)
        {
            if (microbe.name == microbeName)
            {
                _microbes.Remove(microbe);
                return;
            }
        }
    }

    // Returns a microbe from the backpack based on index
    public StringFloatPair GetMicrobe(int index)
    {
        if (index >= _microbes.Count || index < 0)
        {
            if (NotificationPanelManager.Instance.IsAnimating())
            {
                return null;
            }

            NotificationPanelManager.Instance.ShowPanelForSeconds("No microbe!");
            return null;
        }

        return _microbes[index];
    }

    public int GetMicrobeCount()
    {
        return _microbes.Count;
    }

    // Sets the microbe population, removing from backpack if necessary
    public void SetMicrobePopulation(string microbeName, float newPopulation)
    {
        // Iterate through every microbe
        foreach (StringFloatPair microbe in _microbes)
        {
            if (microbe.name != microbeName)
            {
                continue;
            }

            // Set the new population
            microbe.amount = newPopulation;

            // Remove if necessary
            if (microbe.amount <= 0)
            {
                RemoveMicrobe(microbeName);
            }

            return;
        }
    }

    public bool IsFull()
    {
        return _microbes.Count >= MAX_MICROBES;
    }

    // Checks that a certain microbe is present in the backpack
    public bool HasMicrobe(string microbeNameQuery)
    {
        foreach (StringFloatPair microbe in _microbes)
        {
            if (microbe.name == microbeNameQuery)
            {
                return true;
            }
        }

        return false;
    }

    public List<StringFloatPair> GetMicrobes()
    {
        return _microbes;
    }
}
