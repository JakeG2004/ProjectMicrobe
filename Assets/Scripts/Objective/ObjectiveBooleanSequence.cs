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
        if(!_allowMultiToggle)
        {
            // Prevent setting multiple in the same frame
            if (Time.frameCount == _lastSetFrame)
                return;

            _lastSetFrame = Time.frameCount;
        }

        // Sequenced Logic
        if (_isSequenced)
        {
            int prevIndex = 0;

            // Get the key of the previous entry
            for (int i = 0; i < _boolNames.Length; i++)
            {
                if (_boolNames[i] == boolName)
                {
                    // Is First entry in sequence
                    if (i == 0)
                    {
                        _bools[boolName] = true;
                        break;
                    }

                    // Assign the previous Index
                    prevIndex = i - 1;
                }
            }

            // Check if the previous entry is complete
            if (_bools[_boolNames[prevIndex]] == true)
            {
                _bools[boolName] = true;
            }
        }

        // Unsequenced logic
        else
        {
            if (_bools.ContainsKey(boolName))
            {
                _bools[boolName] = true;
            }
        }

        CheckComplete();
    }

    // Public function to set a named bool to false
    public void SetFalse(string boolName)
    {
        if(!_allowMultiToggle)
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

    public void SetAllToFalse()
    {
        Start();
    }
}