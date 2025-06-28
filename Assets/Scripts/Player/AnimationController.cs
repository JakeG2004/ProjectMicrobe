using UnityEngine;

public class AnimationController : MonoBehaviour {

    public Animator ac;
    private PlayerStates _states;
	private Rigidbody _rb;

	void Awake()
	{
		_states = GetComponent<PlayerStates>();
		_rb = GetComponent<Rigidbody>();
	}

    void Start() {
        ac = GetComponent<Animator>();
    }
    void FixedUpdate() {
		ac.SetFloat("Move", _rb.velocity.magnitude * 0.25f);
		ac.SetFloat("Submersion", _states.submersion);
		ac.SetFloat("LookVert", _states.turn.y);
		ac.SetFloat("LookHoz", _states.turn.x);
		if (_states.isJumping) {
			ac.SetTrigger("Jump");
			_states.isJumping = false;
		}
		ac.SetBool("Climbing", _states.isClimbing);
		ac.SetBool("LongDrop", _states.longDrop);
		ac.SetBool("Grounded", _states.isGrounded);
		if (_states.isGrounded) {
			_states.longDrop = false;
		}
	}
}