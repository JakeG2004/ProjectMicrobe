// PlayerMovement.cs
// A script for moving the player, interfaces with other scripts
// Author:  Jake Gendreau
// DatE:    7/18/25

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _gcRadius = 0.5f;

    private Rigidbody _rb;
    private Transform _cam;
    private Vector3 _smoothedLookDir = Vector3.forward;
    private PlayerStatesSO _states;
    private PlayerMovementValsSO _vals;

    public void Init(PlayerStatesSO states, PlayerMovementValsSO vals, float gcRadius, LayerMask collisionMask)
    {
        _collisionMask = collisionMask;
        _gcRadius = gcRadius;
        _states = states;
        _vals = vals;
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main.transform;
        
        _states = GetComponent<PlayerController>().GetStates();
        _vals = GetComponent<PlayerController>().GetVals();
    }

    void FixedUpdate()
    {
        if (_states.isFlying)
        {
            // Defer to player carry controller
            return;
        }

        // Apply input smoothing regardless of movement type
        _states.smoothedMove = Vector2.Lerp(_states.smoothedMove, _states.move, _vals.inputSmoothingSpeed * Time.fixedDeltaTime);
        if (_states.move.magnitude < 0.1f && _states.smoothedMove.magnitude < 0.1f)
        {
            _states.smoothedMove = Vector2.zero;
        }

        CheckEnvironmentalConditions();
        HandleRotation();
        HandleMovement();
        HandleSteppingUp();
    }

    // Checks several environmental conditions at once - cleans up code
    private void CheckEnvironmentalConditions()
    {
        GetSubmergence();
        CheckIfGrounded();
        CheckIfRunningIntoWall();
        CheckForLongDrop();
    }

    // Sets the states submergence value based on y pos assuming constant water height
    private void GetSubmergence()
    {
        _states.submersion = Mathf.Clamp01(transform.position.y * -0.5f);
    }

    // Sets the isGrounded flag if the player is on solid ground
    private void CheckIfGrounded()
    {
        // Can't be grounded while swimming
        if (_states.submersion > 0.8f)
        {
            _states.isGrounded = false;
            return;
        }

        Vector3 checkOffset = Vector3.up * _gcRadius;
        _states.isGrounded = Physics.CheckSphere(transform.position + checkOffset, _gcRadius, ~_collisionMask, QueryTriggerInteraction.Ignore);
    }

    // Sets the long drop flag if the player has been falling for sufficient time
    // long drop has unique animation
    private void CheckForLongDrop()
    {
        if (_states.longDrop)
        {
            return;
        }

        _states.longDrop = _rb.velocity.y < _vals.longDropVelocity;
    }

    // Sets the runningintowall flag if the player is running into the wall
    private void CheckIfRunningIntoWall()
    {
        Vector3 startPos = transform.position + transform.forward * _vals.wallRunningForwardOffset + transform.up * _vals.wallRunningUpOffsetStart;
        Vector3 endPos = transform.position + transform.forward * _vals.wallRunningForwardOffset + transform.up * _vals.wallRunningUpOffsetEnd;
        _states.runningIntoWall = Physics.CheckCapsule(startPos, endPos, _vals.wallRunningRadius, ~_collisionMask, QueryTriggerInteraction.Ignore);
    }

    // Sets the velocity for step up if the player encounters a slope
    private void HandleSteppingUp()
    {
        if (_states.runningIntoWall || _states.isClimbing || _states.move.magnitude < 0.1f)
        {
            return;
        }

        Vector3 checkPos = transform.position + transform.forward * _vals.stepUpForwardOffset + transform.up * _vals.stepUpUpOffset;
        if (Physics.CheckSphere(checkPos, _gcRadius, ~_collisionMask, QueryTriggerInteraction.Ignore))
        {
            _rb.position += Vector3.up * _vals.stepUpValue;
            _states.isGrounded = true;
        }
    }

    // Handles the rotation of the player based on the camera
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
            transform.rotation = Quaternion.LookRotation(_smoothedLookDir);
        }
    }

    private float TurnAngleBasedOnSubmersion()
    {
        return (_vals.maxSwimTurnSpeed * (_vals.swimSpeedTurnScaler * _states.submersion) * Time.fixedDeltaTime);
    }

    // Handle input processing and passes that to specific movement functions
    private void HandleMovement()
    {
        if (_states.isClimbing || !_states.playerCanMove)
        {
            return;
        }

        if (_states.runningIntoWall && _states.move.y > 0)
        {
            _states.move.y = 0;
        }

        Vector3 forwardDir = Vector3.RotateTowards(_cam.forward, Vector3.up, Mathf.Deg2Rad * _vals.cameraAngle, 0f);

        // Calculate the direction of movement
        Vector3 moveDir = Vector3.zero;
        if (_states.smoothedMove.magnitude > 0.1f)
        {
            moveDir = _states.smoothedMove.y * forwardDir + _states.smoothedMove.x * _cam.right;
        }

        if (_states.runningIntoWall && _states.smoothedMove.y > 0)
        {
            moveDir.y = 0;
            moveDir.x *= 0.2f;
            moveDir.z *= 0.2f;
        }

        if (_states.submersion >= 0.8f)
        {
            HandleSwimMovement(moveDir);
        }

        else
        {
            HandleLandMovement(moveDir);
        }
    }

    // Land movement
    private void HandleLandMovement(Vector3 moveDir)
    {
        moveDir = new Vector3(moveDir.x, 0, moveDir.z).normalized;

        // Slow movement based on submersion
        float moveSpeed = Mathf.Lerp(_vals.landSpeed, 1f, _states.submersion);

        Vector3 moveTarget = moveDir * moveSpeed;

        // Handle sprinting
        if (_states.isSprinting)
        {
            moveTarget *= _vals.sprintMod;
        }

        // Lerp from current velocity to target velocity
        Vector3 moveSmooth = Vector3.Lerp(ProjectOnXZPlane(_rb.velocity), ProjectOnXZPlane(moveTarget), _vals.landAcceleration);

        // We re-add the vertical velocity because it gets flattened
        _rb.velocity = moveSmooth + Vector3.up * _rb.velocity.y;
    }

    // Swimming movement
    private void HandleSwimMovement(Vector3 moveDir)
    {
        Vector3 moveTarget = moveDir.normalized * _vals.swimSpeed * _states.smoothedMove.magnitude;

        // Set the vertical target for movement.
        moveTarget.y = Mathf.Lerp(0.1f, moveTarget.y, _states.submersion * 5f - 4f); //FIGURE OUT WHAT THESE CONSTANTS MEAN

        Vector3 moveSmooth = Vector3.Lerp(_rb.velocity, moveTarget, _vals.swimAcceleration);

        _rb.velocity = moveSmooth;
    }

    // Called by PlayerInputHandler Event
    public void Jump()
    {
        
        if (!_states.playerCanMove || !_states.isGrounded)
        {
            return;
        }

        _states.isJumping = true;
        _rb.velocity += new Vector3(0, _vals.jumpForce, 0);
    }

    private Vector3 ProjectOnXZPlane(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }
}
