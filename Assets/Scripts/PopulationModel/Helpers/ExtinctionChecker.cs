// ExtinctionChecker.cs
// A script that will check for extinction and do an event when it happens
// Author:  Jake Gendreau
// Date:    6/2/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExtinctionChecker : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> _extinctEvent;
    private List<Microbe> _newMicrobes;
    private List<Microbe> _oldMicrobes = null;

    public void CheckExtinction()
    {
        // Get the microbes from the simulation
        _newMicrobes = GetComponent<MicrobePopSim>().GetMicrobes();

        // Skip first time step
        if (_oldMicrobes == null)
        {
            _oldMicrobes = new List<Microbe>();
            foreach (Microbe m in _newMicrobes)
            {
                _oldMicrobes.Add(m.Clone()); // assumes Clone() returns a copy of the microbe
            }
            return;
        }

        foreach (Microbe newMicrobe in _newMicrobes)
        {
            // Find the corresponding microbe in oldMicrobes
            foreach (Microbe oldMicrobe in _oldMicrobes)
            {
                if (oldMicrobe.microbeName == newMicrobe.microbeName)
                {
                    // If the old pop > 0 and the new pop = 0, then trigger the event
                    if (oldMicrobe.population > 0 && newMicrobe.population <= 0)
                    {
                        _extinctEvent.Invoke(newMicrobe.microbeName);
                    }
                }
            }
        }

        _oldMicrobes = new List<Microbe>();
        foreach (Microbe m in _newMicrobes)
        {
            _oldMicrobes.Add(m.Clone()); // assumes Clone() returns a copy of the microbe
        }

    }
}
