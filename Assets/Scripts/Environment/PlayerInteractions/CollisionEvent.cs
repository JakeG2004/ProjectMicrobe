using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private UnityEvent _onExit;
    [SerializeField] private bool _oneShot = false;
    private bool activated = false;

    // --- 3D ---
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            Enter();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            Exit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            Enter();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            Exit();
        }
    }

    // --- 2D ---
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            Enter();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            Exit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            Enter();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            Exit();
        }
    }

    private void Enter()
    {
        if (_oneShot && activated)
        {
            return;
        }

        activated = true;
        _onEnter.Invoke();
    }

    private void Exit()
    {
        if (_oneShot && activated)
        {
            return;
        }

        _onExit.Invoke();
    }
}
