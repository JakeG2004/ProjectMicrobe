using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCaller : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;

    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void CallDrone()
    {
        Debug.Log("call drone called");
        Vector3 currentPos = transform.position;
        DroneManager.Instance.StartFlight(_player.position, _targetTransform.position);
    }
}
