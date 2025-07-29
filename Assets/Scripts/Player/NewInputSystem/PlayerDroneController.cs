using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneInputHandler))]
public class PlayerDroneController : MonoBehaviour
{
    [Header("Controls and States")]
    [SerializeField] private PlayerStatesSO _states;
    [SerializeField] private PlayerMovementValsSO _vals;
    [SerializeField] private Drone _drone;
    private bool _isCurrentlyFlying = false;
    private Rigidbody _rb;
    private Transform _cam;
    private Vector3 _smoothedLookDir = Vector3.forward;

    void Awake()
    {
        _states = GetComponent<PlayerController>().GetStates();
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main.transform;
    }

	void Start()
	{
        DroneInputHandler.Instance.OnDismountPressed += (() => SetDroneActivationState(false));
	}

    void OnDisable()
    {
        DroneInputHandler.Instance.OnDismountPressed -= (() => SetDroneActivationState(false));
    }

    void FixedUpdate()
    {
        // Handle switch from not flying -> flying
        if (_states.isFlying && !_isCurrentlyFlying)
        {
            SetDroneActivationState(true);
        }

        // flying -> not flying
        else if (!_states.isFlying && _isCurrentlyFlying)
        {
            SetDroneActivationState(false);
        }

        else if (_states.isFlying)
        {
            GetSubmergence();
            HandleDroneMovement();
            HandleRotation();
        }
    }

    private void SetDroneActivationState(bool state)
    {
        _isCurrentlyFlying = state;
        _rb.useGravity = !_isCurrentlyFlying;
        _states.isFlying = _isCurrentlyFlying;
        _drone.SetDroneDeployed(_isCurrentlyFlying);

        // Set control mode
        if (state)
        {
            NewInputController.Instance.SetDroneMode();
        }

        else
        {
            NewInputController.Instance.Set3DMode();
        }
    }

    private void HandleDroneMovement()
    {
        Vector3 moveDir = CalculateForwardDirection();

        moveDir = new Vector3(moveDir.x, 0, moveDir.z).normalized;

        // Get off drone if too much water
        if (_states.submersion > 0.5f)
        {
            SetDroneActivationState(false);
            return;
        }

        // X Accelerationfdsaf
        Vector3 targetVel = new Vector3(moveDir.x * _states.move.y, 0, moveDir.z * _states.move.y) * _vals.droneXSpeed;

        // Y Acceleration
        targetVel.y = _states.verticalMove * _vals.droneYSpeed;

        // Set the new velocity as the midway-ish of the current vel and the target vel
        Vector3 newVel = Vector3.Lerp(_rb.velocity, targetVel, _vals.droneDrag * Time.deltaTime);

        // Clamp values
        newVel = new Vector3(Mathf.Clamp(newVel.x, -_vals.droneXSpeed, _vals.droneXSpeed),
                            Mathf.Clamp(newVel.y, -_vals.droneYSpeed, _vals.droneYSpeed),
                            Mathf.Clamp(newVel.z, -_vals.droneXSpeed, _vals.droneXSpeed));

        if (Mathf.Abs(_states.verticalMove) <= 0.1f)
        {
            newVel.y = Mathf.Lerp(newVel.y, 0, _vals.droneDrag * Time.deltaTime);
        }

        if (_states.move.magnitude < 0.1f)
        {
            newVel.x = Mathf.Lerp(newVel.x, 0, _vals.droneDrag * Time.deltaTime);
            newVel.z = Mathf.Lerp(newVel.z, 0, _vals.droneDrag * Time.deltaTime);
        }

        _rb.velocity = newVel;
    }

    private void HandleRotation()
    {
        if (_states.isClimbing)
        {
            return;
        }

        // Move look dir towards camera forward
        Vector3 targetLookDir;
        if (_states.smoothedMove.magnitude > 0.1f)
        {
            targetLookDir = _states.smoothedMove.y * ProjectOnXZPlane(_cam.forward) + _states.smoothedMove.x * ProjectOnXZPlane(_cam.right);
        }

        // Snap look dir to forward if no input
        else
        {
            targetLookDir = ProjectOnXZPlane(_cam.forward);
        }

        // Flatten and normalize the vector
        targetLookDir = ProjectOnXZPlane(targetLookDir.normalized);
        _smoothedLookDir = Vector3.Slerp(_smoothedLookDir, targetLookDir, _vals.turnSpeed);

        _states.turnAngle = Vector3.SignedAngle(transform.forward, _smoothedLookDir, Vector3.up);

        // Rotate the player to the desired angle
        if (_smoothedLookDir != Vector3.zero)
        {
            // Base yaw rotation
            Quaternion yawRot = Quaternion.LookRotation(_smoothedLookDir, Vector3.up);

            // Calculate tilt from input
            float pitch = Mathf.Abs(_states.smoothedMove.y) * _vals.droneTiltAmt;
            float roll = -_states.smoothedMove.x * _vals.droneTiltAmt;

            // Calculate and combine Combine yaw and tilt
            Quaternion tiltRot = Quaternion.Euler(pitch, 0f, roll);
            Quaternion targetRot = yawRot * tiltRot;

            // Lerp from the current rotaiton to the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _vals.droneDrag * 2 * Time.deltaTime);
        }
    }

    private Vector3 CalculateForwardDirection()
    {
        // Apply input smoothing regardless of movement type
        _states.smoothedMove = Vector2.Lerp(_states.smoothedMove, _states.move, _vals.inputSmoothingSpeed * Time.fixedDeltaTime);
        if (_states.move.magnitude < 0.1f && _states.smoothedMove.magnitude < 0.1f)
        {
            _states.smoothedMove = Vector2.zero;
        }

        Vector3 forwardDir = _cam.forward;
        return forwardDir;
    }

    private void GetSubmergence()
    {
        _states.submersion = Mathf.Clamp01(transform.position.y * -0.5f);
    }

    private Vector3 ProjectOnXZPlane(Vector3 inVec)
    {
        return new Vector3(inVec.x, 0, inVec.z);
    }
}
