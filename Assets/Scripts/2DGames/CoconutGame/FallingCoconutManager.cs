using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCoconutManager : MonoBehaviour
{
    [SerializeField] private float _fallSpeed = 6.0f;
    [SerializeField] private float _horizontalSpeed = 2.0f;  // Frequency of sine wave
    [SerializeField] private float _horizontalRange = 1.5f;  // Amplitude of sine wave
    private float _curFallSpeed = 6.0f;
    private float _curHorSpeed = 2.0f;
    private float _curHorRange = 1.5f;
    private float _baseX = 0.0f;
    private float _timer = 0f;
    private BoolGameEventTrigger _bget;

    void Start()
    {
        _timer = Random.Range(0f, 2f);
        ResetCoconut();

        _bget = GetComponent<BoolGameEventTrigger>();
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
        float horizontalOffset = Mathf.Sin(_timer * _curHorSpeed) * _curHorRange;

        // Calculate new position
        float newY = transform.localPosition.y - _curFallSpeed * Time.deltaTime;
        float newX = _baseX + horizontalOffset;

        // Prevent from shooting over the edge
        if (Mathf.Abs(newX) > 3.0f)
        {
            newX = Mathf.Sign(newX) * 3;
        }

        transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
    }

    private void ResetCoconut()
    {
        float randX = Random.Range(-3f, 3f);
        float randY = Random.Range(7f, 15f);

        transform.localPosition = new Vector3(randX, randY, 0);
        _baseX = randX;

        _curFallSpeed = _fallSpeed + Random.Range(-2f, 4f);
        _curHorSpeed = _horizontalSpeed + Random.Range(-1f, 1f);
        _curHorRange = _horizontalRange + Random.Range(-1f, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SoundManager.PlaySound(SoundType.EIGHT_BIT_COLLECTED);
            ResetCoconut();
            _bget.TriggerEvent(true);
        }
    }
}
