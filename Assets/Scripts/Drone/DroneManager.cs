// DroneManager.cs
// A sript for managing drone transport
// Author:  Jake Gendreau
// Date:    7/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneManager : MonoBehaviour
{
    [SerializeField] private Transform _homeBase;
    [SerializeField] private UnityEngine.AI.NavMeshAgent _drone;
    [SerializeField] private Transform _dest;
    private Transform _player;
    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = _drone.GetComponent<NavMeshAgent>();
        CallDrone();
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _drone.gameObject.SetActive(false);
    }

    public void CallDrone()
    {
        _drone.gameObject.SetActive(true);
        _drone.transform.position = _homeBase.position;
        _agent.SetDestination(_dest.position);
    }
}
