using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController Instance { get; private set; }

    [SerializeField] private PlayerControlVals _vals;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _gcRadius = 0.5f;

    private Rigidbody _rb;
    private Transform _cam;
    private PlayerStates _states;
    private Vector3 _smoothedLookDir = Vector3.forward;
    private float _lookSensitivity = 3.0f;
    private bool _playerCanMove = true;
    private bool _runningIntoWall = false;
    private Coroutine _curCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }

        // Get components
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main.transform;
        _states = GetComponent<PlayerStates>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Add our generated input sytem actions
        PlayerInputActions playerInputActions = NewInputController.Instance.GetPlayerInputActions();

        // Bind functions
        playerInputActions.Player.Jump.started += Jump;

        // Movement lambdas
        playerInputActions.Player.Movement.performed += ctx => _states.move = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Movement.canceled += ctx => _states.move = Vector2.zero;

        // Sprint lambda to toggle
        playerInputActions.Player.Sprint.started += ctx => _states.isSprinting = !_states.isSprinting;

        // Look lambdas
        playerInputActions.Player.Look.performed += ctx => _states.look = ctx.ReadValue<Vector2>() * _lookSensitivity;
        playerInputActions.Player.Look.canceled += ctx => _states.look = Vector2.zero;

        // zoom lambda
        playerInputActions.Player.Zoom.performed += ctx => _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(ctx.ReadValue<float>()) * _vals.scrollAmt);
    }

    void FixedUpdate()
    {
        _states.smoothedMove = Vector2.Lerp(_states.smoothedMove, _states.move, _vals.inputSmoothingSpeed * Time.fixedDeltaTime);
        if (_states.move.magnitude < 0.1f && _states.smoothedMove.magnitude < 0.1f)
        {
            _states.smoothedMove = Vector2.zero;
        }

        GetSubmergence();
        CheckIfGrounded();
        CheckIfRunningIntoWall();
        StepUp();
        Rotate();
        Move();
        Climb();
    }

    void GetSubmergence()
    {
        _states.submersion = Mathf.Clamp01(transform.position.y * -0.5f);
    }

    void GetTurn()
    {
        float vertAngle = NormalizeAngle(_cam.eulerAngles.x) / -60f;
        _states.turn.y = Mathf.Clamp(Mathf.Lerp(_states.turn.y, vertAngle, 5f * Time.deltaTime), -1f, 1f);
        // hoz turn towards camera direction. max turn adjustment when cam angle is 20+ degrees from player forward
        _states.turn.x = Mathf.Lerp(_states.turn.x, Mathf.Clamp(_states.turnAngle / 20f, -1f, 1f), 8f * Time.fixedDeltaTime);
    }

    private void Climb()
    {
        if (!_states.isClimbing || _states.isJumping)
        {
            return;
        }

        _rb.velocity = new Vector3(_rb.velocity.x * 0.1f, _vals.jumpForce, _rb.velocity.z * 0.1f);

        if (_states.submersion > 0f)
        {
            _rb.position += Vector3.up * 0.1f;
        }
    }

    private void StepUp()
    {
        if (_runningIntoWall)
        {
            return;
        }

        Vector3 checkPos = transform.position + transform.forward * 0.1f + transform.up * 0.3f;
        if (Physics.CheckSphere(checkPos, _gcRadius, ~_collisionMask, QueryTriggerInteraction.Ignore))
        {
            _rb.position += Vector3.up * 0.1f;
            _states.isGrounded = true;
        }
    }

    private void Rotate()
    {
        Vector3 targetLookDir;

        // If moving, look in direction of input
        if (_states.smoothedMove.magnitude > 0.1f)
        {
            targetLookDir = _states.smoothedMove.y * ProjectOnXZPlane(_cam.forward) +
                            _states.smoothedMove.x * ProjectOnXZPlane(_cam.right);
        }
        else
        {
            targetLookDir = ProjectOnXZPlane(_cam.forward);
        }

        // If climbing, don't rotate
        if (_states.isClimbing)
        {
            _smoothedLookDir = ProjectOnXZPlane(_cam.forward);
            return;
        }

        // Normalize target direction
        targetLookDir = ProjectOnXZPlane(targetLookDir.normalized);

        // Smoothly interpolate direction
        _smoothedLookDir = Vector3.Slerp(_smoothedLookDir, targetLookDir, TurnAngleBasedOnSubmersion());

        // Update turn angle for animation
        _states.turnAngle = Vector3.SignedAngle(transform.forward, _smoothedLookDir, Vector3.up);

        // Apply rotation
        if (_smoothedLookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_smoothedLookDir);
        }
    }

    private float TurnAngleBasedOnSubmersion()
    {
        return (10f - 5f * _states.submersion) * Time.fixedDeltaTime;
    }

    private void Move()
    {
        // forward direction if 15 degrees up from camera forward
        Vector3 forwardDir = Vector3.RotateTowards(_cam.forward, Vector3.up, Mathf.Deg2Rad * 15f, 0f);

        if (_runningIntoWall)
        {
            if (_states.smoothedMove.y > 0)
            {
                _states.smoothedMove.y = 0;
            }

            _states.smoothedMove *= new Vector3(0.2f, 1f, 0.2f);
        }

        // unnormalized direction vector
        Vector3 moveDir = (_states.smoothedMove.magnitude > 0.1f) ? (_states.smoothedMove.y * forwardDir + _states.smoothedMove.x * _cam.right) : forwardDir;
        moveDir.y = 0;

        // Reset controls to be effectively zero when player not allowed to move
        if (!_playerCanMove)
        {
            _states.move = Vector2.zero;
        }

        // Swimming movement
        if (_states.submersion >= 0.8f)
        {
            HandleSwimMovement(moveDir);
        }

        // Land movement
        else
        {
            HandleLandMovement(moveDir);
        }
    }

    private void HandleLandMovement(Vector3 moveDir)
    {
        // Normalize direction vector and disallow vertical movement input
        moveDir = new Vector3(moveDir.x, 0, moveDir.z).normalized;

        // Run more slowly through deeper water
        float moveSpeed = Mathf.Lerp(_vals.landSpeed, 1f, _states.submersion) * _states.smoothedMove.magnitude;

        // Calculate target movement speed
        Vector3 moveTarget = moveDir * moveSpeed;
        if (_states.isSprinting)
        {
            moveTarget *= _vals.sprintMod;
        }

        // Current speed movement on XZ plane
        Vector3 moveCurrent = ProjectOnXZPlane(moveTarget).normalized;

        // Accelerate towards target
        Vector3 moveSmooth = Vector3.Lerp(moveCurrent, moveTarget, _vals.landAcceleration);

        // Apply velocity
        _rb.velocity = moveSmooth + Vector3.up * _rb.velocity.y;
    }

    private void HandleSwimMovement(Vector3 moveDir)
    {
        // Calculate the target movement speed, applying user input
        Vector3 moveTarget = moveDir.normalized * _vals.swimSpeed * _states.smoothedMove.magnitude;

        // Limit vertical input near the surface
        moveTarget.y = Mathf.Lerp(0.1f, moveTarget.y, _states.submersion * 5f - 4f);

        // Accelerate towardss the target speed
        Vector3 moveSmooth = Vector3.Lerp(_rb.velocity, moveTarget, _vals.swimAcceleration);

        // Apply the velocity
        _rb.velocity = moveSmooth;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!_playerCanMove || !_states.isGrounded)
        {
            return;
        }

        _states.isJumping = true;

        // Eject from ladder if climbing
        if (_states.isClimbing)
        {
            _states.isClimbing = false;

            Vector3 launchVelocity = new Vector3(0, _vals.ladderEjectForce, 0);
            _rb.velocity = launchVelocity;
            return;
        }

        if (!_rb)
        {
            return;
        }

        // Handle normal jump
        _rb.velocity += new Vector3(0, _vals.jumpForce, 0);
    }

    private void CheckIfRunningIntoWall()
    {
        float radius = 0.1f;
        Vector3 startPos = transform.position + transform.forward * 0.3f + transform.up * 0.8f;
        Vector3 endPos = transform.position + transform.forward * 0.3f + transform.up * 1.8f;
        _runningIntoWall = Physics.CheckCapsule(startPos, endPos, radius, ~_collisionMask, QueryTriggerInteraction.Ignore);
    }

    private void CheckIfGrounded()
    {
        // Not grounded while swimming
        if (_states.submersion > 0.8f)
        {
            _states.isGrounded = false;
            return;
        }

        float checkRadius = _gcRadius;
        Vector3 checkOffset = Vector3.up * checkRadius;

        _states.isGrounded = Physics.CheckSphere(transform.position + checkOffset, checkRadius, ~_collisionMask, QueryTriggerInteraction.Ignore);

        // If not groudned or submerged, check for long drop
        if (!_states.isGrounded)
        {
            CheckForLongDrop();
        }
    }

    private void CheckForLongDrop()
    {
        if (_states.longDrop)
        {
            return;
        }

        _states.longDrop = _rb.velocity.y < -9f;
    }

    private Vector3 ProjectOnXZPlane(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }

    // Convert given anlge to range -180 to 180
    float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            angle -= 360;
        return angle;
    }

    public PlayerStates GetStates()
    {
        return _states;
    }

    public void SetMovementState(bool state)
    {
        _playerCanMove = state;
    }

    public void SetLookSensitivity(float val)
    {
        _lookSensitivity = val;
    }

    public float GetLookSensitivity()
    {
        return _lookSensitivity;
    }

    // Sets the rotation and position of the player to correspond with the ladder
    public void SetClimbPos(Transform ladder)
    {
        if (_curCoroutine != null)
        {
            return;
        }

        // Set rotation
        Vector3 newRot = ladder.eulerAngles;
        newRot.x = (newRot.x + 90f) % 360;

        // Set position
        Vector3 newPos = ladder.position + ladder.up * 0.2f;
        newPos.y = transform.position.y;

        // Call the subroutine
        _curCoroutine = StartCoroutine(SnapToLadder(newPos, newRot));
    }

    // Positions the player to a reasonable pos and rotation to climb the ladder over the course of .1 seconds
    private IEnumerator SnapToLadder(Vector3 targetPos, Vector3 targetRot)
    {
        float elapsedTime = 0.0f;
        float totalTime = 0.1f;

        Vector3 initPos = transform.position;
        Vector3 initRot = transform.eulerAngles;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / totalTime;

            transform.position = Vector3.Lerp(initPos, targetPos, ratio);
            transform.eulerAngles = Vector3.Lerp(initRot, targetRot, ratio);

            yield return null;
        }

        transform.position = targetPos;
        transform.eulerAngles = targetRot;

        _curCoroutine = null;
    }

    // void OnDrawGizmos()
    // {
    //     Vector3 drawPos = transform.position + (transform.forward * 2f);
    // 
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(drawPos, 1f);
    // }
}


[System.Serializable]
public class PlayerControlVals
{
    [SerializeField] public float jumpForce = 8f;
    [SerializeField] public float ladderEjectForce = 10f;

    [Space(10)]
    [SerializeField] public float swimSpeed = 4f;
    [SerializeField] public float swimAcceleration = 2f;

    [Space(10)]
    [SerializeField] public float landSpeed = 6f;
    [SerializeField] public float landAcceleration = 5f;
    [SerializeField] public float sprintMod = 1.5f;

    [Space(10)]
    [SerializeField] public float scrollAmt = 0.25f;

    [Space(10)]
    [SerializeField] public float inputSmoothingSpeed = 15f;
}