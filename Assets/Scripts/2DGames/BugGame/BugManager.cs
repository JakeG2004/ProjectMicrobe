// BugManager.cs
// A script for managing the bug minigame
// Author:  Jake Gendreau
// Date:    6/16/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class BugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private int _totalTime = 30;
    [SerializeField] private int _goalScore = 75;

    [Space(10)]
    [SerializeField] private UnityEvent _OnTimerEndEvent;
    [SerializeField] private UnityEvent _OnGoalReachedEvent;
    private int _curScore = 0;

    void OnEnable()
    {
        _curScore = 0;

        _scoreText.text = "Score: 0";
        _timeText.text = $"Time: {_totalTime}";

        StartCoroutine(IManageTimer());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnBugClicked(int numClicked)
    {
        _curScore += numClicked;
        _scoreText.text = $"Score: {_curScore}"; 
    }

    private IEnumerator IManageTimer()
    {
        int curTime = _totalTime;

        while (curTime > 0)
        {
            // Update the time text
            _timeText.text = $"Timer: {curTime}";

            curTime -= 1;
            yield return new WaitForSeconds(1.0f);
        }

        _timeText.text = "Timer: 0";
        _OnTimerEndEvent.Invoke();

        NewInputController.Instance.SetMenuMode();

        if (_curScore >= _goalScore)
        {
            _OnGoalReachedEvent.Invoke();
        }
    }
}
