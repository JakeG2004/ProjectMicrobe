// PlayerCarry.cs
// A script to manage player movement when being carried by a drone
// Author:  Jake Gendreau
// Date:    7/18/25

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCarry : MonoBehaviour
{
    private PlayerStatesSO _states;

    private Rigidbody _rb;
    private GameObject _drone;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _states = GetComponent<PlayerController>().GetStates();
    }

    void FixedUpdate()
    {
        if (_states.isBeingCarried && _drone != null)
        {
            transform.position = _drone.transform.position;
            transform.eulerAngles = _drone.transform.eulerAngles;
        }
    }

    public void StartCarry(GameObject drone)
    {
        _states.isBeingCarried = true;

        // Allow the rb to be fully manipulated by the carrying body
        _rb.isKinematic = true;
        _rb.constraints = RigidbodyConstraints.None;

        _drone = drone;
    }

    public void EndCarry()
    {
        _states.isBeingCarried = false;

        // Re-enable control over the rb
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        _drone = null;
    }
}
