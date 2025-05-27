/*
Objective.cs:   Interfaces with an objectives system to provide guidance
                to the player.

Author:         Jake Gendreau
Date:           5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Objective : MonoBehaviour
{
    [SerializeField] private ObjectiveChannelsSO _objectiveChannelsSO;
    [Space(10)]

    [SerializeField] protected Objective _nextObjective;
    [Space(10)]

    [SerializeField] protected string _objectiveText;
    [Space(10)]

    [SerializeField] protected bool _isFirstObjective = false;
    [Space(10)]

    [SerializeField] protected Transform _targetObject;

    [SerializeField] protected UnityEvent _onActivate;
    [SerializeField] protected UnityEvent _onComplete;
    [SerializeField] protected UnityEvent _onFail;

    protected bool _isActivated = false;
    protected bool _isComplete = false;
    protected bool _isFailed = false;
    protected bool _isOneShot = true;
    protected ObjectiveEntryScript _objEntry;

    // IEnumerator so that it can have built in waiting functionality
    private IEnumerator Start()
    {
        // Wait 2 seconds then activate if set as first objective
        if (_isFirstObjective)
        {
            yield return new WaitForSeconds(2);
            ActivateObjective();
        }

        yield return null;
    }

    // Broadcasts an objective over the SO for listeners to receive
    public void ActivateObjective()
    {
        if (_objectiveChannelsSO == null || _objectiveChannelsSO.objectiveAddChannelSO == null || (_isActivated && _isOneShot))
        {
            return;
        }

        _isActivated = true;

        _onActivate?.Invoke();
        _objectiveChannelsSO.objectiveAddChannelSO.Raise(this);
    }

    // Call to complete an objective
    public void RaiseObjectiveComplete()
    {
        if (_isComplete || !_isActivated)
        {
            return;
        }

        _isComplete = true;

        _onComplete?.Invoke();
        _objectiveChannelsSO.objectiveCompleteChannelSO.Raise(this);
        if (_objEntry)
        {
            _objEntry.CompleteObjective();
        }
        ActivateNextObjective();
    }

    // Call to fail an objective
    public void RaiseObjectiveFailed()
    {
        if (_isComplete || _isFailed)
        {
            return;
        }

        _isComplete = true;
        _isFailed = true;

        _onFail?.Invoke();
        _objectiveChannelsSO.objectiveFailedChannelSO.Raise(this);
        ActivateNextObjective();
    }

    public void ActivateNextObjective()
    {
        if (_nextObjective == null)
        {
            return;
        }

        // Set the gameobject to be true just in case
        _nextObjective.gameObject.SetActive(true);

        // Activate it
        _nextObjective.ActivateObjective();
    }

    // Link the objective to its entry in the objectives UI
    public void LinkToPopup(ObjectiveEntryScript objEntry)
    {
        _objEntry = objEntry;
    }

    public void SetObject(Transform newObj)
    {
        _targetObject = newObj;
    }

    /*
    *
    *    GETTERS
    *
    */

    public string GetObjectiveText()
    {
        return _objectiveText;
    }

    public bool IsActivated()
    {
        return _isActivated;
    }

    public bool IsComplete()
    {
        return _isComplete;
    }

    public bool IsFailed()
    {
        return _isFailed;
    }

    public Transform GetTarget()
    {
        return _targetObject;
    }
}
