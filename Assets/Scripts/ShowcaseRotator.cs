using System.Collections;
using UnityEngine;

public class ShowcaseRotator : MonoBehaviour
{
    [Header("Axis to rotate on")]
    [SerializeField] private bool _xAxis = true;
    [SerializeField] private bool _yAxis = true;
    [SerializeField] private bool _zAxis = true;

    [Space(10)]
    [Header("Axis Rotation Speeds (seconds per full cycle)")]
    [SerializeField] private Vector3 _rotateSpeed = new Vector3(1, 1, 1);

    private Vector3 _initialRot;

    void Start()
    {
        _initialRot = transform.localEulerAngles;
    }

    public void StartRotation()
    {
        if (_xAxis) StartCoroutine(RotateAxis(Vector3.right, _rotateSpeed.x));
        if (_yAxis) StartCoroutine(RotateAxis(Vector3.up, _rotateSpeed.y));
        if (_zAxis) StartCoroutine(RotateAxis(Vector3.forward, _rotateSpeed.z));
    }

    public void StopRotation()
    {
        StopAllCoroutines();
    }

    private IEnumerator RotateAxis(Vector3 axis, float secondsPerCycle)
    {
        if (secondsPerCycle <= 0f) yield break; // avoid divide by zero

        float degreesPerSecond = 360f / secondsPerCycle;

        while (true)
        {
            transform.Rotate(axis, degreesPerSecond * Time.deltaTime, Space.Self);
            yield return null;
        }
    }
}
