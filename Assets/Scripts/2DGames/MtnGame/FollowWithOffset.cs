using UnityEngine;

public class FollowWithOffset : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _target;
    [SerializeField] private bool _followX = true;
    [SerializeField] private bool _followY = true;
    [SerializeField] private bool _followZ = true;

    void Update()
    {
        Vector3 newPos = transform.position;

        if (_followX)
        {
            newPos.x = _target.position.x + _offset.x;
        }

        if (_followY)
        {
            newPos.y = _target.position.y + _offset.y;
        }

        if (_followZ)
        {
            newPos.z = _target.position.z + _offset.z;
        }

        transform.position = newPos;
    }
}
