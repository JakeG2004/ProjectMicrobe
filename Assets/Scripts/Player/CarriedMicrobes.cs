using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedMicrobes : MonoBehaviour
{
    const int MAX_MICROBES = 3;
    [SerializeField] private List<Microbe> _microbes = new List<Microbe>();

    public void AddMicrobe(Microbe newMicrobe)
    {
        // Handle case of too many microbes
        if(_microbes.Count >= MAX_MICROBES)
        {
            Debug.LogWarning("Player attempting to add microbe to full backpack");
            return;
        }

        // Ensure no duplicates
        foreach(Microbe microbe in _microbes)
        {
            if(microbe.microbeName == newMicrobe.microbeName)
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
        if(_microbes.Count <= 0)
        {
            return;
        }

        // Find target microbe and destroy (remove from list)
        foreach(Microbe microbe in _microbes)
        {
            if(microbe.microbeName == microbeName)
            {
                _microbes.Remove(microbe);
                return;
            }
        } 
    }

    public Microbe GetMicrobe(int index)
    {
        if(index >= _microbes.Count || index < 0)
        {
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
        foreach(Microbe microbe in _microbes)
        {
            if(microbe.microbeName != microbeName)
            {
                continue;
            }

            microbe.population = newPopulation;
            if(microbe.population <= 0)
            {
                RemoveMicrobe(microbeName);
            }
            return;
        }
    }
}
