using UnityEngine;

public class AnimationController : MonoBehaviour 
{
    [SerializeField] private PlayerStatesSO _states;
    private Animator _ac;
	private Rigidbody _rb;

	private float moveVal = 0.0f;
	private bool _wasJumping = false;

	public void Init(PlayerStatesSO states)
	{
		_states = states;
	}

	void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_ac = GetComponent<Animator>();
	}

	void LateUpdate()
	{
		// Get horizontal velocity magnitude (ignoring Y)
		Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
		float rawVelocityMag = horizontalVelocity.magnitude;

		// Set a threshold for what counts as "not moving"
		const float velocityThreshold = 0.1f;

		float targetMoveVal = _states.smoothedMove.magnitude * (_states.isSprinting ? 2f : 1f);

		// Prevent small jitters from triggering movement animations
		if (rawVelocityMag < velocityThreshold)
		{
			moveVal = Mathf.Lerp(moveVal, 0f, 0.3f);
		}
		else
		{
			moveVal = Mathf.Lerp(moveVal, targetMoveVal, 0.3f);
		}

		// Handle animation floats
		_ac.SetFloat("Move", moveVal);
		_ac.SetFloat("Submersion", _states.submersion);
		_ac.SetFloat("LookVert", _states.turn.y);
		_ac.SetFloat("LookHoz", _states.turn.x);

		// Handle animation bools
		_ac.SetBool("Climbing", _states.isClimbing);
		_ac.SetBool("LongDrop", _states.longDrop);
		_ac.SetBool("Grounded", _states.isGrounded);
		_ac.SetBool("Flying", _states.isFlying);

		// Handle jump
		if (_states.isJumping && !_wasJumping)
		{
			_ac.SetTrigger("Jump");
		}
		_wasJumping = _states.isJumping;
	}
}