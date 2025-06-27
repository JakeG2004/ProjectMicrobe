using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController Instance { get; private set; }

    [SerializeField] private PlayerControlVals _vals;
    [SerializeField] private LayerMask _collisionMask;
    private Rigidbody _rb;
    private Transform _cam;
    private PlayerStates _states;
    private Vector2 _inputVector;
    private float _lookSensitivity = 3.0f;
    private bool _playerCanMove = true;
    private bool _runningIntoWall = false;

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

        // Add our generated input sytem actions
        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Bind functions
        playerInputActions.Player.Jump.started += Jump;

        // Movement lambdas
        playerInputActions.Player.Movement.performed += ctx => _inputVector = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Movement.canceled += ctx => _inputVector = Vector2.zero;

        // Sprint lambda to toggle
        playerInputActions.Player.Sprint.started += ctx => _states.isSprinting = !_states.isSprinting;

        // Look lambdas
        playerInputActions.Player.Look.performed += ctx => _states.look = ctx.ReadValue<Vector2>() * _lookSensitivity;
        playerInputActions.Player.Look.canceled += ctx => _states.look = Vector2.zero;

        // zoom lambda
        playerInputActions.Player.Zoom.performed += ctx => _states.zoom = Mathf.Clamp01(_states.zoom + Mathf.Sign(ctx.ReadValue<float>()) * _vals._scrollAmt);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        _states.move = _inputVector;

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
        if (Physics.CheckSphere(checkPos, 0.2f, ~_collisionMask, QueryTriggerInteraction.Ignore))
        {
            _rb.position += Vector3.up * 0.1f;
            _states.isGrounded = true;
        }
    }

    private void Rotate()
    {
        Vector3 lookDir = Vector3.zero;

        // face towards direction of movement
        if (_inputVector.magnitude > 0.1f)
        {
            lookDir += _inputVector.y * ProjectOnXZPlane(_cam.forward);
            lookDir += _inputVector.x * ProjectOnXZPlane(_cam.right);
        }

        // Face camera forward
        else
        {
            lookDir = ProjectOnXZPlane(_cam.forward);
        }

        // Disable rotation if climbing
        if (_states.isClimbing)
        {
            lookDir = ProjectOnXZPlane(_cam.forward);
        }

        // Calculate the angle between the player forward and the target direction
        _states.turnAngle = Vector3.SignedAngle(transform.forward, lookDir, Vector3.up);

        // Rotate towards the target direction
        Vector3 rot = Vector3.RotateTowards(transform.forward, lookDir, TurnAngleBasedOnSubmersion(), 0f);

        // Apply the new rotation
        transform.rotation = Quaternion.LookRotation(rot);
    }

    private float TurnAngleBasedOnSubmersion()
    {
        return (10f - 5f * _states.submersion) * Time.fixedDeltaTime;
    }

    private void Move()
    {
        // forward direction if 15 degrees up from camera forward
        Vector3 forwardDir = Vector3.RotateTowards(_cam.forward, Vector3.up, Mathf.Deg2Rad * 15f, 0f);

        // unnormalized direction vector
        Vector3 moveDir = (_inputVector.magnitude > 0.1f) ? (_inputVector.y * forwardDir + _inputVector.x * _cam.right) : forwardDir;

        // Reset controls to be effectively zero when player not allowed to move
        if (!_playerCanMove)
        {
            _inputVector = Vector2.zero;
        }

        // Swimming movement
        if (_states.submersion >= 0.8f)
        {
            HandleSwimMovement(_inputVector, moveDir);
        }

        // Land movement
        else
        {
            HandleLandMovement(_inputVector, moveDir);
        }
    }

    private void HandleLandMovement(Vector2 _inputVector, Vector3 moveDir)
    {
        // Normalize direction vector and disallow vertical movement input
        moveDir = new Vector3(moveDir.x, 0, moveDir.z).normalized;

        // Run more slowly through deeper water
        float moveSpeed = Mathf.Lerp(_vals.landSpeed, 1f, _states.submersion) * _inputVector.magnitude;

        // Calculate target movement speed
        Vector3 moveTarget = _states.isSprinting ? moveDir * moveSpeed * _vals.sprintMod : moveDir * moveSpeed;

        // Current speed movement on XZ plane
        Vector3 moveCurrent = ProjectOnXZPlane(moveTarget).normalized;

        // Accelerate towards target
        Vector3 moveSmooth = Vector3.Lerp(moveCurrent, moveTarget, _vals.landAcceleration);

        // Apply velocity
        _rb.velocity = moveSmooth + Vector3.up * _rb.velocity.y;
    }

    private void HandleSwimMovement(Vector2 _inputVector, Vector3 moveDir)
    {
        // Calculate the target movement speed, applying user input
        Vector3 moveTarget = moveDir.normalized * _vals.swimSpeed * _inputVector.magnitude;

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

        float checkRadius = 0.2f;
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
    [SerializeField] public float _scrollAmt = 0.25f;
}