// DirectionArrowScript.cs
// A script for managing the direction arrow above the player
// Author:  Jake Gendreau
// Date:    5/21/35

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrowScript : MonoBehaviour
{
    public static DirectionArrowScript Instance { get; private set; }
    [SerializeField] private Transform _target;
    private List<GameObject> _collidingObjs;
    private MeshRenderer _rend;
    private Transform _oldDir;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _collidingObjs = new();
        _rend = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Hide if no target
        if (_target == null)
        {
            _rend.enabled = false;
            return;
        }

        // Show based on the toggle and collisiosn
        _rend.enabled = true;

        // Look at the target
        transform.LookAt(_target);

        // Offset rotation by 90 degrees
        transform.rotation *= Quaternion.Euler(0, 90, 0);
    }

    // Public function to change the target to a new onefdsa
    public void ChangeTarget(Transform newTarget)
    {
        if (!newTarget)
        {
            RemoveTarget();
        }

        _target = newTarget;
    }

    public void RemoveTarget()
    {
        _target = null;
    }

    void OnTriggerEnter(Collider col)
    {
        _collidingObjs.Add(col.gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        _collidingObjs.Remove(col.gameObject);
    }

    public void StorePosition()
    {
        _oldDir = _target;
    }

    public void RestorePosition()
    {
        _target = _oldDir;
    }
}
