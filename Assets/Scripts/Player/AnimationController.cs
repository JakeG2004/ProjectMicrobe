using UnityEngine;

public class AnimationController : MonoBehaviour {

    public Animator ac;
    private PlayerStates _states;

	void Awake()
	{
		_states = GetComponent<PlayerStates>();
	}

    void Start() {
        ac = GetComponent<Animator>();
    }
    void FixedUpdate() {
		ac.SetFloat("Move", _states.move.magnitude * (_states.isSprinting ? 1.5f : 1f));
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