using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAri : MonoBehaviour
{
    [SerializeField] private GameObject Ari;
    [SerializeField] private float _rotationSpeed = 5f;
    private float _minRotation;
    private float _maxRotation;
    private float _initialYRotation;
    private float _curRotation;

    void Start()
    {
        // Store the initial rotation's Y component as the start rotation
        _initialYRotation = Ari.transform.rotation.eulerAngles.y;

        // Set the min and max rotation values based on the initial rotation
        _minRotation = _initialYRotation; // You can adjust the range here as needed
        _maxRotation = _initialYRotation + 360f; // You can adjust the range here as needed

        _curRotation = _initialYRotation;
    }

    public void RotateAriByAmt(float amt)
    {
        // Clamp amt between 0 and 1
        amt = Mathf.Clamp01(amt);

        // Lerp the rotation value between the min and max rotations
        float targetRotation = Mathf.Lerp(_minRotation, _maxRotation, amt);

        // Set the Y rotation of the object, keeping the X and Z values intact
        Ari.transform.rotation = Quaternion.Euler(Ari.transform.rotation.eulerAngles.x, targetRotation, Ari.transform.rotation.eulerAngles.z);
    }

    public void RotateAriStick(InputAction.CallbackContext ctx)
    {
        Vector2 stickVal = ctx.ReadValue<Vector2>();
        _curRotation += (stickVal.x * _rotationSpeed * Time.deltaTime);

        RotateAriByAmt(_curRotation);
    }
}
