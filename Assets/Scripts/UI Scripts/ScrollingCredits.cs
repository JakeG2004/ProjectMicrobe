using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScrollingCredits : MonoBehaviour
{
    private float _startingValue = -250;
    private float _endValue = -2400;
    private float _timeToComplete = 30;

    [SerializeField] private UnityEvent _onFinishedScrolling;
    private float _curTime = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        _curTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _curTime += Time.deltaTime;

        if(_curTime / _timeToComplete == 1)
        {
            _onFinishedScrolling.Invoke();
            _curTime = 0;
        }

        Vector3 oldPos = transform.position;

        oldPos.y = Mathf.Lerp(_startingValue, -_endValue, _curTime / _timeToComplete);

        transform.position = oldPos;
    }
}
