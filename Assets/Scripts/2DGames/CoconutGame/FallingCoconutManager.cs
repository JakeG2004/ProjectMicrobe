using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCoconutManager : MonoBehaviour
{
    [SerializeField] private float _fallSpeed = 6.0f;
    [SerializeField] private float _horizontalSpeed = 2.0f;  // Frequency of sine wave
    [SerializeField] private float _horizontalRange = 1.5f;  // Amplitude of sine wave

    private float _baseX = 0.0f;
    private float _timer = 0f;

    void Start()
    {
        _timer = Random.Range(0f, 2f);
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), Random.Range(5f, 15f), 0f);
        _baseX = transform.localPosition.x;
    }

    void Update()
    {
        // If it falls below a certain point, reset position
        if (transform.localPosition.y <= -2.78f)
        {
            _baseX = Random.Range(-3f, 3f);
            transform.localPosition = new Vector3(_baseX, Random.Range(7f, 15f), transform.localPosition.z);
            return;
        }

        // Increase timer for sine wave
        _timer += Time.deltaTime;

        // Calculate horizontal sine wave offset
        float horizontalOffset = Mathf.Sin(_timer * _horizontalSpeed) * _horizontalRange;

        // Calculate new position
        float newY = transform.localPosition.y - _fallSpeed * Time.deltaTime;
        float newX = _baseX + horizontalOffset;

        transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
    }
}
