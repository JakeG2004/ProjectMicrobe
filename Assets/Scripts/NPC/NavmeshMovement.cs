// NavmeshMovement.cs
// A script for moving things with navmesh
// Author:  Jake Gendreau
// Date:    6/24/25
// Following tutorial: https://learn.unity.com/tutorial/working-with-navmesh-agents#601751eeedbc2a03202b775f

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshMovement : MonoBehaviour
{
    [SerializeField] protected float _walkRadius = 8.0f;
    [SerializeField] protected bool _debug = false;
    protected NavMeshAgent _agent;
    protected Vector3 _initPos;
    protected float _oldSpeed = 0.0f;

    protected virtual void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _initPos = transform.position;

        PickNewMovementTarget();
    }
    
    protected virtual void PickNewMovementTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;
        randomDirection += _initPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _walkRadius, 1);
        _agent.destination = hit.position;
    }

    protected virtual void PauseMovement()
    {
        _oldSpeed = _agent.speed;
        _agent.speed = 0;
    }

    protected virtual void UnpauseMovement()
    {
        _agent.speed = _oldSpeed;
    }

    protected void OnDrawGizmos()
    {
        if (!_debug)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _walkRadius);
    }
}