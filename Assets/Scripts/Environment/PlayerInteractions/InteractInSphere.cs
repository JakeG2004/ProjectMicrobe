using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractInSphere : MonoBehaviour
{
    [SerializeField] private UnityEvent _interactEvent;
    private Transform _playerTransform;
    private bool _playerInRange = false;
    private KeyCode _interact = KeyCode.E;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(_interact))
        {
            _interactEvent.Invoke();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            _playerInRange = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            _playerInRange = false;
        }
    }
}
