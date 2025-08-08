// PlayerController.cs
// A script to manage all of the other player movement scripts
// Author:  Jake Gendreau
// Date:    7/18/25

using UnityEngine;

// [RequireComponent(typeof(PlayerMovement))]
// [RequireComponent(typeof(PlayerInputHandler))]
// [RequireComponent(typeof(PlayerClimbing))]
// [RequireComponent(typeof(PlayerCarry))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Controls and States")]
    [SerializeField] private PlayerStatesSO _states;
    [SerializeField] private PlayerMovementValsSO _vals;

    [Space(10)]
    [Header("Passthrough variables")]
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _gcRadius = 0.5f;

    [Space(10)]
    [Header("Dependencies")]
    private PlayerInputHandler _inputHandler;
    private PlayerMovement _playerMovement;
    private PlayerClimbing _playerClimbing;
    private PlayerCarry _playerCarry;
    private PlayerDroneController _droneController;

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

        GetComponentReferences();
        AddNonReferenceComponents();
    }

    void Start()
    {
        NewInputController.Instance.Set3DMode();
        
        _inputHandler = NewInputController.Instance.playerInput;
        _inputHandler.OnJumpDown += HandleJump;
        _inputHandler.OnSprintPressed += HandleSprintPressed;
        _inputHandler.OnSprintCancelled += HandleSprintCancelled;
        _inputHandler.OnDroneToggled += HandleDroneToggle;

        _states.ResetValues();
    }

    // Unsubscribes from events
    void OnDisable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnJumpDown -= HandleJump;
            _inputHandler.OnSprintPressed -= HandleSprintPressed;
            _inputHandler.OnSprintCancelled -= HandleSprintCancelled;
            _inputHandler.OnDroneToggled -= HandleDroneToggle;
        }
    }

    private void HandleJump()
    {
        if (_states.isClimbing)
        {
            _playerClimbing.JumpFromClimb();
        }

        else
        {
            _playerMovement.Jump();
        }
    }

    private void HandleSprintPressed()
    {
        if (_states.sprintIsToggle)
        {
            HandleSprintToggle();
        }

        _states.isSprinting = true;
    }

    private void HandleSprintCancelled()
    {
        if (_states.sprintIsToggle)
        {
            return;
        }

        _states.isSprinting = false;
    }

    private void HandleSprintToggle()
    {
        _states.isSprinting = !_states.isSprinting;
    }

    private void HandleDroneToggle()
    {
        _states.isFlying = !_states.isFlying;

        if ((_states.isFlying && _states.submersion > 0.5f) || _states.isClimbing)
        {
            _states.isFlying = false;
        }
    }

    // Public methods for other systems to interact with player movement
    public void SetMovementState(bool state)
    {
        _states.playerCanMove = false;
    }

    public void SetLookSensitivity(float val)
    {
        _states.movementVals.lookSensitivity = val;
    }

    public float GetLookSensitivity()
    {
        return _states.movementVals.lookSensitivity;
    }

    public void SetSprintToggle(bool state)
    {
        _states.sprintIsToggle = state;
    }

    public bool GetSprintToggle()
    {
        return _states.sprintIsToggle;
    }

    public PlayerStatesSO GetStates()
    {
        return _states;
    }

    public PlayerMovementValsSO GetVals()
    {
        return _vals;
    }

    // Ladder interaction
    public void SetClimbPos(Transform ladder)
    {
        if (_playerClimbing == null || _states.isFlying)
        {
            return;
        }
        
        _playerClimbing.SetClimbPos(ladder);
    }

    public void EndClimb()
    {
        if (_playerClimbing != null)
        {
            _playerClimbing.EndClimb();
        }
    }

    // Creates / gets component references during gameplay
    private void GetComponentReferences()
    {
        _playerMovement = gameObject.AddComponent<PlayerMovement>();
        _playerMovement.Init(_states, _vals, _gcRadius, _collisionMask);

        _playerClimbing = gameObject.AddComponent<PlayerClimbing>();
        _playerClimbing.Init(_states, _vals);

        _droneController = GetComponent<PlayerDroneController>();
    }

    // Adds components to the player that are not needed by this script, but are required nonetheless
    private void AddNonReferenceComponents()
    {
        gameObject.AddComponent<PlayerSoundController>();
        gameObject.AddComponent<CarriedMicrobes>();
        gameObject.AddComponent<CarriedPylon>();

        FootIK footIK = gameObject.AddComponent<FootIK>();
        footIK.Init(_collisionMask);

        AnimationController animController = gameObject.AddComponent<AnimationController>();
        animController.Init(_states);
    }
}
