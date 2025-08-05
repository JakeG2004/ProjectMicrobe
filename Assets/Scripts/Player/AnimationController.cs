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

	void FixedUpdate()
	{
		// Lerp towards the movement speed from the preious frame so that animation blending gets a chance
		moveVal = Mathf.Lerp(moveVal, _states.smoothedMove.magnitude * (_states.isSprinting ? 2f : 1f), 0.3f);

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
		if (_states.isJumping)
		{
			if (!_wasJumping)
			{
				_ac.SetTrigger("Jump");
			}
		}
		_wasJumping = _states.isJumping;
	}
}