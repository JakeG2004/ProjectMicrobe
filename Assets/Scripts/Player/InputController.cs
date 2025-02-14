using UnityEngine;

public class InputController : MonoBehaviour {

	[HideInInspector] public Vector2 move = Vector2.zero; // player movement
	[HideInInspector] public Vector2 turn = Vector2.zero; // player rotation
	[HideInInspector] public float turnAngle = 0f; // angle between player forward and target direction (set by movement controller)

	[HideInInspector] public Vector2 look = Vector2.zero; // camera rotation
	readonly float lookSensitivity = 5f;

	[HideInInspector] public float zoom = 0.5f; // camera distance ratio
	readonly float zoomSensitivity = 0.1f;

	[HideInInspector] public float submersion = 0f;

	// these are mostly set in other places when conditions are met and can be retrieved from here
	[HideInInspector] public bool grounded = false;
	[HideInInspector] public bool longDrop = false;
	[HideInInspector] public bool jumpInput = false;
	[HideInInspector] public bool triggerJump = false;
	[HideInInspector] public bool climbing = false;
	[HideInInspector] public bool sprinting = false;

	public bool useMobileJoysticks = false;
	[HideInInspector] public bool sprintInvert = false;
	[HideInInspector] public Vector2 mobileMoveInput = Vector2.zero;
	[HideInInspector] public Vector2 mobileLookInput = Vector2.zero;
	public bool mobileJumpInput;

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
		if (useMobileJoysticks) look = mobileLookInput * lookSensitivity;
		else look = new Vector2(Input.GetAxis("CamHoz"), Input.GetAxis("CamVert")) * lookSensitivity;
	}
	void GetZoom() {
		zoom = Mathf.Clamp01(zoom + Input.GetAxis("Zoom") * zoomSensitivity);
	}
	void GetMove() {
		sprinting = Input.GetButton("Sprint") || Input.GetAxis("SprintXbox") > 0.5f;
		if (sprintInvert) sprinting = !sprinting;
		Vector2 moveInput = useMobileJoysticks? mobileMoveInput : new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
		jumpInput = Input.GetButton("Jump") || Input.GetAxis("JumpXbox") > 0.5f || mobileJumpInput;
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


	//public functions that can be called from buttons for mobile input
	public void Jump(bool value) {
		mobileJumpInput = value;
	}
	public void Zoom(bool dir) {
		float speed = dir ? -zoomSensitivity : zoomSensitivity;
		zoom = Mathf.Clamp01(zoom + speed);
	}
}



// controller joystick input info:
// https://discussions.unity.com/t/how-to-use-right-analog-stick-from-xbox-controller-to-look-around/198462/2