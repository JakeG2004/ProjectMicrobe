using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCourseManager : MonoBehaviour
{
    [SerializeField] private List<DroneCourseRing> _rings = new();
    private Queue<DroneCourseRing> _ringQueue = new();
    private DroneCourseRing _curRing = null;
    private int _curRingIdx = 0;
    private BoolGameEventTrigger _timeEvent;
    private VoidGameEventTrigger _completionEvent;
    private static int NUM_RINGS_ACTIVE = 15;

    void Awake()
    {
        _timeEvent = GetComponent<BoolGameEventTrigger>();
        _completionEvent = GetComponent<VoidGameEventTrigger>();
        ConstructRingList();
    }

    public void ActivateCourse()
    {
        _curRingIdx = 0;
        DirectionArrowScript.Instance?.StorePosition();
        ResetRingStates();
        SetCurRing(_rings[_curRingIdx]);
    }

    public void DeactivateCourse()
    {
        UnsubscribeFromRings();
        CompleteCourse();
    }

    void OnDisable()
    {
        UnsubscribeFromRings();
        CompleteCourse();
    }

    private void UnsubscribeFromRings()
    {
        foreach (DroneCourseRing ring in _rings)
        {
            ring.OnPlayerPassthrough -= IncreaseRingScore;
        }
    }

    // Goes through all of the children and gets drone course ring script that exists in their children
    // Parent is the object, child is the trigger for progression
    private void ConstructRingList()
    {
        _curRingIdx = 0;

        _rings.Clear();
        foreach (Transform child in transform)
        {
            _rings.Add(child.GetComponentInChildren<DroneCourseRing>(true));
            child.gameObject.SetActive(false);
        }
    }

    private void SetCurRing(DroneCourseRing ring)
    {
        // Unsubscribe from old ring
        if (_curRing != null)
        {
            _curRing.OnPlayerPassthrough -= IncreaseRingScore;
        }

        // Subsribe to new ring
        _curRing = ring;
        _curRing.OnPlayerPassthrough += IncreaseRingScore;

        SetActiveRings();
        _curRing.ActivateRing();
    }

    private void SetActiveRings()
    {
        // Initialize the queue
        if (_curRingIdx == 0 && _ringQueue.Count == 0)
        {
            for (int i = 0; i < Mathf.Floor(NUM_RINGS_ACTIVE / 2) && _curRingIdx + i < _rings.Count; i++)
            {
                _rings[_curRingIdx + i].ShowRing();
                _ringQueue.Enqueue(_rings[_curRingIdx + i]);
            }

            return;
        }

        if (_ringQueue.Count >= NUM_RINGS_ACTIVE)
        {
            DroneCourseRing oldestRing = _ringQueue.Dequeue();
            oldestRing.HideRing();
        }

        if (!_rings[_rings.Count - 1].gameObject.activeInHierarchy)
        {
            DroneCourseRing nextRing = GetNextRing();
            nextRing.ShowRing();
            _ringQueue.Enqueue(nextRing);
        }
    }

    private DroneCourseRing GetNextRing()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                continue;
            }

            DroneCourseRing curRing = child.GetComponentInChildren<DroneCourseRing>();

            if (curRing.IsComplete())
            {
                continue;
            }

            return child.GetComponentInChildren<DroneCourseRing>();
        }

        return null;
    }

    private void IncreaseRingScore()
    {
        // Start timer when first ring passed through
        if (_curRingIdx == 0)
        {
            _timeEvent.TriggerEvent(true);
        }

        _curRingIdx++;

        if (_curRingIdx >= _rings.Count)
        {
            CompleteCourse();
            return;
        }

        SetCurRing(_rings[_curRingIdx]);
    }

    private void CompleteCourse()
    {
        foreach (DroneCourseRing ring in _rings)
        {
            if (ring.gameObject.activeInHierarchy)
            {
                ring.HideRing();
            }
        }

        _ringQueue.Clear();

        DirectionArrowScript.Instance.RestorePosition();
        _timeEvent?.TriggerEvent(false);
        _completionEvent?.TriggerEvent();
    }

    private void ResetRingStates()
    {
        foreach (DroneCourseRing ring in _rings)
        {
            ring.ResetRing();
        }
    }
}
