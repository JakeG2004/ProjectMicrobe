using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private UnityEvent _onExit;

    // --- 3D ---
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            _onExit.Invoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onExit.Invoke();
        }
    }

    // --- 2D ---
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            _onExit.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onExit.Invoke();
        }
    }
}
