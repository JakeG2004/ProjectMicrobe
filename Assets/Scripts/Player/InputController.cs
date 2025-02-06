using UnityEngine;

public class InputController : MonoBehaviour {

	public Vector2 move = Vector2.zero;
	public float lookVert = 0f;
    public float lookHoz = 0f;
	public float turnAngle = 0f; // set by movement controller
    public float submersion = 0f;
    public bool grounded = false;
	public bool longDrop = false;
	public bool jumpInput = false;
	public bool triggerJump = false;

    Transform cam;
	Transform player;

	void Awake() {
        GM.playerInput = this;
    }
    void Start() {
        cam = GM.cam;
		player = GM.player;
    }
    void Update() {        
        SetMove();
        SetLookVert();
        SetLookHoz();
        SetSubmergence();
		JumpInput();
    }

	void JumpInput(){
		jumpInput = Input.GetKey(KeyCode.Space);
	}
    void SetSubmergence() {
        submersion = Mathf.Clamp01(player.position.y * -0.5f);
    }
    void SetMove() {
		Vector2 moveInput = Vector2.zero;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveInput.y += 1f;
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveInput.y -= 1f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveInput.x += 1f;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveInput.x -= 1f;
		moveInput = Vector2.ClampMagnitude(moveInput, 1f);
		float speed = Input.GetKey(KeyCode.LeftShift) ? 2f : Input.GetKey(KeyCode.CapsLock) ? 0.5f : 1f;
		move = Vector2.Lerp(move, moveInput * speed, 5f * Time.deltaTime);
	}
    void SetLookVert() {
        float vertAngle = NormalizeAngle(cam.eulerAngles.x);
        float scaledVert = Mathf.Lerp(lookVert, -vertAngle / 60f, 5f * Time.deltaTime);
        lookVert = Mathf.Clamp(scaledVert, -1f, 1f);
    }
    void SetLookHoz() {
		// max look adjustment when cam angle is 20+ degrees from player forward
		lookHoz = Mathf.Lerp(lookHoz, Mathf.Clamp(turnAngle / 20f, -1f, 1f), 8f * Time.fixedDeltaTime);
	}


	// Convert given anlge to range -180 to 180
	float NormalizeAngle(float angle) {
		angle %= 360;
		if (angle > 180)
			angle -= 360;
		return angle;
	}
	// Simple projection for a vector onto the xz-plane.
	Vector3 ProjectOnXZPlane(Vector3 vec) {
		vec.y = 0.0f;
		return vec;
	}
}