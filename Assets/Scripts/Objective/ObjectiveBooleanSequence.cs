using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct NamedBool
{
    public string name;
    public bool isComplete;
}

//
public class ObjectiveBooleanSequence : MonoBehaviour
{
    [SerializeField] private List<NamedBool> _objectives = new();
    [SerializeField] private UnityEvent _completedEvent;
    [SerializeField] private bool _isSequenced = false;
    [SerializeField] private bool _allowMultiToggle = true;
    [SerializeField] private bool _debug = false;

    private int _lastSetFrame = -1;

    // Public function to set a named bool to true
    public void SetTrue(string boolName)
    {
        if (!_allowMultiToggle)
        {
            if (Time.frameCount == _lastSetFrame)
                return;
            _lastSetFrame = Time.frameCount;
        }

        int index = _objectives.FindIndex(o => o.name == boolName);
        if (index == -1)
        {
            Debug.LogWarning($"NamedBool `{boolName}` not found!");
            return;
        }

        if (!_isSequenced)
        {
            SetObjectiveState(index, true);
        }
        else
        {
            if (index == 0 || _objectives[index - 1].isComplete)
            {
                SetObjectiveState(index, true);
            }
        }

        CheckComplete();
    }

    public void SetFalse(string boolName)
    {
        if (!_allowMultiToggle)
        {
            if (Time.frameCount == _lastSetFrame)
                return;
            _lastSetFrame = Time.frameCount;
        }

        int index = _objectives.FindIndex(o => o.name == boolName);
        if (index == -1)
            return;

        SetObjectiveState(index, false);
    }

    private void SetObjectiveState(int index, bool value)
    {
        NamedBool obj = _objectives[index];
        obj.isComplete = value;
        _objectives[index] = obj;

        if (_debug)
        {
            PrintBoolStates();   
        }
    }

    public void CheckComplete()
    {
        foreach (var obj in _objectives)
        {
            if (!obj.isComplete)
                return;
        }

        _completedEvent.Invoke();
    }

    public void PrintBoolStates()
    {
        string statement = "<b>NamedBool States:</b>";
        for (int i = 0; i < _objectives.Count; i++)
        {
            string color = _objectives[i].isComplete ? "green" : "red";
            statement += $"\n{i + 1}. <color={color}>{_objectives[i].name}: {_objectives[i].isComplete}</color>";
        }
        Debug.Log(statement);
    }
}
