using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    private Animator _anim;
    private bool _isTiming;
    private float _curTime = 0f;
    private Coroutine _timerCoroutine;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void StartTimer()
    {
        if (_isTiming)
        {
            return;
        }

        _isTiming = true;
        _curTime = 0.0f;
        _timerCoroutine = StartCoroutine(Timer());
        ShowTimer();
    }

    public void StopTimer()
    {
        if (_timerCoroutine == null)
        {
            return;
        }
        
        _isTiming = false;
        StopCoroutine(_timerCoroutine);
        StartCoroutine(HideTimer());
    }

    private void ShowTimer()
    {
        UpdateTimeText();
        _anim.SetBool("active", true);
    }

    private void UpdateTimeText()
    {
        float minutes = Mathf.Floor(_curTime / 60);
        float seconds = _curTime - (minutes * 60);
        

        string timeString = $"{minutes.ToString("00")}:{seconds.ToString("00.00")}";
        _timerText.text = timeString;
    }

    public float GetTime()
    {
        return _curTime;
    }

    // Flashes the time 3 times before hiding the whole thing
    private IEnumerator HideTimer()
    {
        string time = _timerText.text;

        for (int i = 0; i < 5; i++)
        {
            _timerText.text = "";
            yield return new WaitForSeconds(0.5f);
            _timerText.text = time;
            yield return new WaitForSeconds(0.5f);
        }

        _anim.SetBool("active", false);
    }

    private IEnumerator Timer()
    {
        _curTime = 0f;

        while (_isTiming)
        {
            _curTime += Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
    }
}
