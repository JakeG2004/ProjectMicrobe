// PylonPlacementController.cs
// A script for managing pylon placement by the player
// Author:  Jake Gendreau
// Date:    5/22/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PylonPlacementController : MonoBehaviour
{
    // The pylon object that will be moved
    [SerializeField] private Transform _pylonObject;

    // The distances for the raycasts
    [SerializeField] private float _xRayDist = 5.0f;
    [SerializeField] private float _yRayDist = 1.0f;

    [SerializeField] private float _objectRotation = 180f;
    [SerializeField] private float _yOffset = 0.1f;

    [SerializeField] private UnityEvent _onPylonPlacedEvent;

    private CarriedPylon _cp;

    void Start()
    {
        _cp = Object.FindObjectOfType<CarriedPylon>();
    }

    void Update()
    {
        CalculatePylonPlacement();
    }

    void CalculatePylonPlacement()
    {
        if (!_cp.IsPlaceable())
        {
            _pylonObject.gameObject.SetActive(false);
            return;
        }

        // Set the object to have the camera's y rotation but no other rotation
        Quaternion _newRot = Camera.main.transform.rotation;
        _newRot.Set(0, _newRot.y, 0, _newRot.w);
        transform.rotation = _newRot;

        // Vertical raycast point
        Vector3 _vRayOrigin;

        // Our raycast hits
        RaycastHit _xHit;
        RaycastHit _yHit;

        // Layermask to ignore player
        int mask = ~LayerMask.GetMask("Player", "PylonRegion");

        bool gotHit = false;

        // Send a raycast forward. On hit
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _xHit, _xRayDist, mask))
        {
            _vRayOrigin = _xHit.point;
        }

        // On miss
        else
        {
            _vRayOrigin = transform.position + (_xRayDist * transform.TransformDirection(Vector3.forward));
        }

        // Send the raycast downward first, then upward
        // NOTE: This is a short-circuiting expression. Equivalent to
        // if(raycast down) ...
        // else if (raycast up)
        if (Physics.Raycast(_vRayOrigin, -Vector3.up, out _yHit, _yRayDist, mask) ||
        Physics.Raycast(_vRayOrigin, Vector3.up, out _yHit, _yRayDist, mask))
        {
            // Set position (subtract small amount to ensure collision with ground)
            _pylonObject.position = _yHit.point + new Vector3(0, _yOffset, 0);

            // Rotate by user defined amount
            _pylonObject.rotation = Quaternion.Euler(0, _newRot.eulerAngles.y + _objectRotation, 0);

            gotHit = true;
        }

        // set enabled based on whether there was a y hit
        _pylonObject.gameObject.SetActive(gotHit && _cp.IsPlaceable());
    }
}
