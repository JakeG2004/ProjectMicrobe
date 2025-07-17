// PlayerController.cs
// A script to manage all of the other player movement scripts
// Author:  Jake Gendreau
// Date:    7/18/25

using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerClimbing))]
[RequireComponent(typeof(PlayerCarry))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Controls and States")]
    [SerializeField] private PlayerStatesSO _states;
    [SerializeField] private PlayerMovementValsSO _vals;

    [Space(10)]
    [Header("Dependencies")]
    private PlayerInputHandler _inputHandler;
    private PlayerMovement _playerMovement;
    private PlayerClimbing _playerClimbing;
    private PlayerCarry _playerCarry;

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
    }


    // Subscribes to events
    void OnEnable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnJumpStarted += HandleJump;
            _inputHandler.OnSprintToggled += HandleSprintToggle;
        }
    }

    // Unsubscribes from events
    void OnDisable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnJumpStarted -= HandleJump;
            _inputHandler.OnSprintToggled -= HandleSprintToggle;
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

    private void HandleSprintToggle()
    {
        _states.isSprinting = !_states.isSprinting;
    }

    // Public methods for other systems to interact with player movement
    public void SetMovementState(bool state)
    {
        _states.playerCanMove = false;
    }

    public void SetLookSensitivity(float val)
    {
        if (_inputHandler != null)
        {
            _inputHandler.SetLookSensitivity(val);
        }
    }

    public float GetLookSensitivity()
    {
        if (_inputHandler != null)
        {
            return _inputHandler.GetLookSensitivity();
        }

        return 0f;
    }

    public PlayerStatesSO GetStates()
    {
        return _states;
    }

    public PlayerMovementValsSO GetVals()
    {
        return _vals;
    }

    public void StartCarry(GameObject drone)
    {
        if (_playerCarry != null)
        {
            _playerCarry.StartCarry(drone);
        }

        // Disable other movement
        if (_playerMovement != null) _playerMovement.enabled = false;
        if (_playerClimbing != null) _playerClimbing.enabled = false;
    }

    public void EndCarry()
    {
        if (_playerCarry != null)
        {
            _playerCarry.EndCarry();
        }

        // Re-enable movement
        if (_playerMovement != null) _playerMovement.enabled = true;
        if (_playerClimbing != null) _playerClimbing.enabled = true;
    }

    // Ladder interaction
    public void SetClimbPos(Transform ladder)
    {
        if (_playerClimbing != null)
        {
            _playerClimbing.SetClimbPos(ladder);
        }
    }

    public void EndClimb()
    {
        if (_playerClimbing != null)
        {
            _playerClimbing.EndClimb();
        }
    }

    private void GetComponentReferences()
    {
        if (_inputHandler == null) _inputHandler = GetComponent<PlayerInputHandler>();
        if (_playerMovement == null) _playerMovement = GetComponent<PlayerMovement>();
        if (_playerClimbing == null) _playerClimbing = GetComponent<PlayerClimbing>();
        if (_playerCarry == null) _playerCarry = GetComponent<PlayerCarry>();
    }
}
