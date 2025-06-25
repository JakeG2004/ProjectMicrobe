using UnityEngine;

public class PingPongPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float _timeOff = 0.0f;
    private float _time;

    void Update()
    {
        _time += Time.deltaTime * speed;

        // PingPong returns a value that goes back and forth between 0 and 1
        float t = Mathf.PingPong(_time + _timeOff, 1f);

        // Move the platform between pointA and pointB
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }
}
