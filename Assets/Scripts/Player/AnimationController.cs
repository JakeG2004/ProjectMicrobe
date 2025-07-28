using UnityEngine;

<<<<<<< Updated upstream
public class AnimationController : MonoBehaviour 
{
    [SerializeField] private PlayerStatesSO _states;
    public Animator ac;
=======
public class AnimationController : MonoBehaviour {

    [HideInInspector] public Animator ac;
    private PlayerStates _states;
>>>>>>> Stashed changes
	private Rigidbody _rb;

	private float moveVal = 0.0f;

	void Awake()
	{
		_rb = GetComponent<Rigidbody>();
	}

    void Start() {
        ac = GetComponent<Animator>();
    }
    void FixedUpdate() {
		// Lerp towards the movement speed from the preious frame so that animation blending gets a chance
		moveVal = Mathf.Lerp(moveVal, _states.smoothedMove.magnitude * (_states.isSprinting ? 2f : 1f), 0.3f);

		ac.SetFloat("Move", moveVal);
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