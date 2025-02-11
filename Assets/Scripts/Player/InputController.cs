using UnityEngine;

public class InputController : MonoBehaviour {

	public Vector2 move = Vector2.zero; // player movement
	public Vector2 turn = Vector2.zero; // player rotation
	public float turnAngle = 0f; // angle between player forward and target direction (set by movement controller)

	public Vector2 look = Vector2.zero; // camera rotation
	readonly float lookSensitivity = 5f;

	public float zoom = 0.5f; // camera distance ratio
	readonly float zoomSensitivity = 0.1f;

	public float submersion = 0f;

	// these are mostly set in other places when conditions are met and can be retrieved from here
    public bool grounded = false;
	public bool longDrop = false;
	public bool jumpInput = false;
	public bool triggerJump = false;
	public bool climbing = false;
	public bool sprinting = false;

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
		GetLook();
		GetZoom();
		GetMove();
        GetTurn();
		GetJump();
		GetSubmergence();
    }

	void GetLook() {
		look = new Vector2(Input.GetAxis("CamHoz"), Input.GetAxis("CamVert")) * lookSensitivity;
	}
	void GetZoom() {
		zoom = Mathf.Clamp01(zoom + Input.GetAxis("Zoom") * zoomSensitivity);
	}
	void GetMove() {
		sprinting = Input.GetButton("Sprint") || Input.GetAxis("SprintXbox") > 0.5f;
		Vector2 moveInput = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		float speedMult = sprinting ? 2f : Input.GetButton("Walk") ? 0.5f : 1f;
		move = Vector2.Lerp(move, moveInput * speedMult, 5f * Time.deltaTime);
	}
	void GetTurn() {
		float vertAngle = NormalizeAngle(cam.eulerAngles.x) / -60f;
		turn.y = Mathf.Clamp(Mathf.Lerp(turn.y, vertAngle, 5f * Time.deltaTime), -1f, 1f);
		// hoz turn towards camera direction. max turn adjustment when cam angle is 20+ degrees from player forward
		turn.x = Mathf.Lerp(turn.x, Mathf.Clamp(turnAngle / 20f, -1f, 1f), 8f * Time.fixedDeltaTime);
	}
	void GetJump(){
		jumpInput = Input.GetButton("Jump") || Input.GetAxis("JumpXbox") > 0.5f;
	}
    void GetSubmergence() {
        submersion = Mathf.Clamp01(player.position.y * -0.5f);
    }

	// Convert given anlge to range -180 to 180
	float NormalizeAngle(float angle) {
		angle %= 360;
		if (angle > 180)
			angle -= 360;
		return angle;
	}
}





// controller joystick input info https://discussions.unity.com/t/how-to-use-right-analog-stick-from-xbox-controller-to-look-around/198462/2

/*************** TEMP BACKUP KEYBOARD HARDCODED VERSION *******************************
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
**************************************************************************************/