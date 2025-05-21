// ObjectiveBooleanSequence.cs
// This script is a helper script for objectives.
// It creates a list of named objectives which must be complete
// and will broadcast a unityevent on completion
// 
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveBooleanSequence : MonoBehaviour
{
    [SerializeField] private string[] _boolNames;
    private Dictionary<string, bool> _bools;

    [SerializeField] private UnityEvent _completedEvent;
    [SerializeField] private bool _isSequenced = false;

    void Start()
    {
        // Initialize the dictionary with all of the strings
        _bools = new();
        foreach (string name in _boolNames)
        {
            _bools.Add(name, false);
        }
    }

    // Public function to set a named bool to true
    public void SetTrue(string boolName)
    {
        // Handle the sequenced logic
        if (_isSequenced)
        {
            for (int i = 0; i < _boolNames.Length; i++)
            {
                // Skip to the relevant entry
                if (_boolNames[i] != boolName)
                {
                    continue;
                }
                
                // Handle relevant entry being 0
                if (i == 0)
                {
                    _bools[_boolNames[i]] = true;
                    return;
                }
                
                // Check previous entry
                if (_bools[_boolNames[i - 1]] == false)
                {
                    return;
                }

                // Set it the previous was true
                _bools[_boolNames[i]] = true;
            }
            return;
        }
        
        // Unesequenced logic
        if (_bools.ContainsKey(boolName))
        {
            _bools[boolName] = true;
        }

        CheckComplete();
    }

    // Public function to set a named bool to false
    public void SetFalse(string boolName)
    {
        if (_bools.ContainsKey(boolName))
        {
            _bools[boolName] = false;
        }
    }

    // Function to check whether or not every objective has been completed
    public void CheckComplete()
    {
        foreach (var kvp in _bools)
        {
            if (!kvp.Value)
            {
                return;
            }
        }

        _completedEvent?.Invoke();
    }
}