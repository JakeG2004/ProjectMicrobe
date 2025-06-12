using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedMicrobes : MonoBehaviour
{
    /*
    const int MAX_MICROBES = 3;
    [SerializeField] private List<Microbe> _microbes = new List<Microbe>();

    public void AddMicrobe(Microbe newMicrobe)
    {
        bool addingNewMicrobe = true;
        foreach (Microbe microbe in _microbes)
        {
            if (newMicrobe.microbeName == microbe.microbeName)
            {
                addingNewMicrobe = false;
            }
        }
        
        // Handle case of too many microbes
        if (_microbes.Count >= MAX_MICROBES && addingNewMicrobe)
        {
            //Debug.LogWarning("Player attempting to add microbe to full backpack");
            return;
        }

        // Ensure no duplicates
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == newMicrobe.microbeName)
            {
                microbe.population += newMicrobe.population;
                return;
            }
        }

        // Add the microbe
        _microbes.Add(newMicrobe);
    }

    public void RemoveMicrobe(string microbeName)
    {
        // Handle empty list
        if (_microbes.Count <= 0)
        {
            return;
        }

        // Find target microbe and destroy (remove from list)
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == microbeName)
            {
                _microbes.Remove(microbe);
                return;
            }
        }
    }

    public Microbe GetMicrobe(int index)
    {
        if (index >= _microbes.Count || index < 0)
        {
            if (NotificationPanelManager.Instance.IsAnimating() == true)
            {
                return null;
            }

            NotificationPanelManager.Instance.ShowPanelForSeconds("No microbes to remove!");
            Debug.Log("Attempting to take from outside of bounds.");
            return null;
        }

        return _microbes[index].Clone();
    }

    public int GetMicrobeCount()
    {
        return _microbes.Count;
    }

    public void SetMicrobePopulation(string microbeName, float newPopulation)
    {
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName != microbeName)
            {
                continue;
            }

            microbe.population = newPopulation;
            if (microbe.population <= 0)
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

    public bool HasMicrobe(string microbeQueryName)
    {
        foreach (Microbe microbe in _microbes)
        {
            if (microbe.microbeName == microbeQueryName)
            {
                return true;
            }
        }

        return false;
    }

    public List<StringFloatPair> GetMicrobes()
    {
        List<StringFloatPair> retVal = new();

        foreach (Microbe microbe in _microbes)
        {
            StringFloatPair microbeEntry = new();
            microbeEntry.name = microbe.microbeName;
            microbeEntry.amount = microbe.population;
            retVal.Add(microbeEntry);
        }

        return retVal;
    }*/

    const int MAX_MICROBES = 3;
    [SerializeField] private List<StringFloatPair> _microbes = new List<StringFloatPair>();

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

            NotificationPanelManager.Instance.ShowPanelForSeconds("No microbes to remove!");
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
