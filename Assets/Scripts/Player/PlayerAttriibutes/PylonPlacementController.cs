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

        // Get camera y-rotation only
        Quaternion camRotation = Camera.main.transform.rotation;
        Quaternion newRot = Quaternion.Euler(0f, camRotation.eulerAngles.y, 0f);
        transform.rotation = newRot;

        // Raycast forward to find a point to check ground below
        int mask = ~LayerMask.GetMask("Player", "PylonRegion", "AlwaysOnTop", "Ignore Raycast");

        RaycastHit forwardHit;

        Vector3 forwardOrigin = transform.position;
        Vector3 forwardDir = transform.forward;

        Vector3 verticalRayOrigin;

        if (Physics.Raycast(forwardOrigin, forwardDir, out forwardHit, _xRayDist, mask))
        {
            verticalRayOrigin = forwardHit.point + Vector3.up * 2.0f;  // Start above the hit point
        }
        else
        {
            verticalRayOrigin = forwardOrigin + forwardDir * _xRayDist + Vector3.up * 2.0f;  // Start above max distance point
        }

        RaycastHit groundHit;

        // Cast downward from above the expected ground level
        if (Physics.Raycast(verticalRayOrigin, Vector3.down, out groundHit, _yRayDist + 2.0f, mask))
        {
            // Set position with small offset upwards to avoid clipping
            _pylonObject.position = groundHit.point + Vector3.up * _yOffset;

            // Align rotation to slope normal + user rotation offset
            _pylonObject.rotation = Quaternion.Euler(0, newRot.eulerAngles.y + _objectRotation, 0);

            _pylonObject.gameObject.SetActive(true);
        }
        else
        {
            // No ground found — hide the pylon object
            _pylonObject.gameObject.SetActive(false);
        }
    }

}