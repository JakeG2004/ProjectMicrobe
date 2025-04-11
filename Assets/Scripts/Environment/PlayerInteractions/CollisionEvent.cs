using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private UnityEvent _onExit;

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == _targetTag)
        {
            _onEnter.Invoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == _targetTag)
        {
            _onEnter.Invoke();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if(col.gameObject.tag == _targetTag)
        {
            _onExit.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == _targetTag)
        {
            _onExit.Invoke();
        }
    }
}
