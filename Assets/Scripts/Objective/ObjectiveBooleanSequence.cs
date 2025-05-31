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
    [SerializeField] private bool _allowMultiToggle = true;

    private int _lastSetFrame = -1;

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
        // Handle multiple toggle settings
        if (!_allowMultiToggle)
        {
            // Prevent setting multiple in the same frame
            if (Time.frameCount == _lastSetFrame)
                return;

            _lastSetFrame = Time.frameCount;
        }

        if (!(_bools.ContainsKey(boolName)))
        {
            Debug.LogWarning($"Objective `{boolName}` not found!");
            return;
        }

        // Find the index of the current bool name iin _boolNames
        int index = System.Array.IndexOf(_boolNames, boolName);
        
        // Handle bool name not in array
        if(index == -1)
        {
            Debug.LogWarning($"Objective `{boolName}` not found in ordered array!");
            return;
        }

        // Unsequenced logic
        if(!_isSequenced)
        {
            _bools[boolName] = true;
        }

        // Sequenced logic
        else
        {
            // First element can always be set
            if(index == 0)
            {
                _bools[boolName] = true;
            }

            // Only allow if previous element has been set
            else
            {
                string prevName = _boolNames[index - 1];
                if(_bools.TryGetValue(prevName, out bool prevComplete) && prevComplete)
                {
                    _bools[boolName] = true;
                }

                else
                {
                    //Debug.Log($"Cannot set `{boolName}` yet. Previous objective not set");
                }
            }
        }


        CheckComplete();
    }

    // Public function to set a named bool to false
    public void SetFalse(string boolName)
    {
        if (!_allowMultiToggle)
        {
            // Prevent setting multiple in the same frame
            if (Time.frameCount == _lastSetFrame)
                return;

            _lastSetFrame = Time.frameCount;
        }

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



    public void PrintBoolStates()
    {
        string statement = "";
        foreach (var kvp in _bools)
        {
            statement += "\n" + kvp.Key + ": " + kvp.Value;
        }

        Debug.Log(statement);
    }
}