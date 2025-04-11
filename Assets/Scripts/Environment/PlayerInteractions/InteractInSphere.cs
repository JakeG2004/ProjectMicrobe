using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractInSphere : MonoBehaviour
{
    [SerializeField] private float _interactDist = 7.0f;
    [SerializeField] private UnityEvent _interactEvent;
    private Transform _playerTransform;
    private bool _playerInRange;
    private KeyCode _interact = KeyCode.E;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if(GetComponent<SphereCollider>())
        {
            _interactDist = GetComponent<SphereCollider>().radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, _playerTransform.position) <= _interactDist && Input.GetKeyDown(_interact))
        {
            _interactEvent.Invoke();
        }
    }
}
