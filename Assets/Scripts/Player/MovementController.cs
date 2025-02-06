using UnityEngine;

public class MovementController : MonoBehaviour {

    Rigidbody rb;
    Transform cam;
	InputController ic;
	bool runningIntoWall = false;

	// base speed (jogging) before applying sprint or walk modifiers
	float landSpeed = 6f;
	float landAcceleration = 5f;
	float swimSpeed = 4f;
	float swimAcceleration = 2f;

	[SerializeField] LayerMask collisionMask;

	void Awake() {
        GM.player = transform;
    }
    void Start() {
        rb = GetComponent<Rigidbody>();
        cam = GM.cam;
		ic = GM.playerInput;
    }

    void FixedUpdate() {
		CheckIfGrounded();
		CheckIfRunningIntoWall();
		StepUp();
		Jump();
		Rotate();
		Move();
		

		//StepUp();
    }

	void CheckIfGrounded() {
		//don't count as grounded while swimming
		if (ic.submersion > 0.8f) ic.grounded = false;
		else {
			float checkRadius = 0.2f;
			Vector3 checkOffset = Vector3.up * checkRadius;
			ic.grounded = Physics.CheckSphere(transform.position + checkOffset, checkRadius, ~collisionMask, QueryTriggerInteraction.Ignore);

			// if not submerged or grounded check for long drop
			if (!ic.grounded){
				CheckForLongDrop();
			}
		}
		// can't jump if not grounded
		if (!ic.grounded) ic.jumpInput = false;
	}
	void CheckForLongDrop () {
		// skip if longDrop is already triggered
		if (ic.longDrop) return;
		// if rb donwards velocity excedes 9 mps, it's been a long drop
		ic.longDrop = rb.velocity.y < -9f;
		//Debug.Log("Vertical velocity: " + rb.velocity.y.ToString("0.##"));
	}

	void Jump() {
		if (ic.jumpInput) {
			// rb.AddForce(Vector3.up * 8f, ForceMode.VelocityChange); // stackable for crazt high jumps
			rb.velocity = new Vector3(rb.velocity.x, 8f, rb.velocity.z);
			ic.triggerJump = true;
		}
	}

	void CheckIfRunningIntoWall() {
		// narrower radius for wall check than step up check allows running up along walls
		float radius = 0.1f;
		Vector3 startPos = transform.position + transform.forward * 0.3f + transform.up * 0.8f;
		Vector3 endPos = transform.position + transform.forward * 0.3f + transform.up * 1.8f;
		runningIntoWall = Physics.CheckCapsule(startPos, endPos, radius, ~collisionMask, QueryTriggerInteraction.Ignore);
		// if (runningIntoWall) Debug.Log("Running into wall!");
	}
	// if checksphere collides and you're not running into a wall, that means you can step up.
	void StepUp() {
		if (runningIntoWall) return;
		Vector3 checkPos = transform.position + transform.forward * 0.1f + transform.up * 0.3f;
		if (Physics.CheckSphere(checkPos, 0.2f, ~collisionMask, QueryTriggerInteraction.Ignore)){
			rb.position += Vector3.up * 0.1f;
			ic.grounded = true;
			// Debug.Log("Stepping up!");
		}
	}

	void Rotate() {
		Vector3 lookDir = Vector3.zero;
		// face towards direction of movement, or face camera forward if not moving
		if (ic.move.magnitude > 0.1f) {
			lookDir += ic.move.y * ProjectOnXZPlane(cam.forward);
			lookDir += ic.move.x * ProjectOnXZPlane(cam.right);
		}
		else { lookDir = ProjectOnXZPlane(cam.forward);	}
		// angle between player forward and target direction
		ic.turnAngle = Vector3.SignedAngle(transform.forward, lookDir, Vector3.up);
		// rotate towards target direction
		Vector3 rot = Vector3.RotateTowards(transform.forward, lookDir, TurnAngleBasedOnSubmersion(), 0f);
		// apply new rotation
		transform.rotation = Quaternion.LookRotation(rot);
	}
	float TurnAngleBasedOnSubmersion() {
		//turn slower in water
		return (10f - 5f * ic.submersion) * Time.fixedDeltaTime;
	}

    void Move() {
		// forward direction if 15 degrees up from camera forward
		Vector3 forwardDir = Vector3.RotateTowards(cam.forward, Vector3.up,Mathf.Deg2Rad * 15f, 0f);

		if (runningIntoWall) {
			ic.move *= new Vector3(0.2f, 1f, 0.2f); // slow movement

			// stop trying to move - stops stickiness when pressed into walls, but can't push things,
			// also can feel stuck against wall because desn't allow slide
			// ic.move = Vector3.zero;
		}

		// unnormalized direction vector
		Vector3 moveDir = (ic.move.magnitude > 0.1f)? (ic.move.y * forwardDir + ic.move.x * cam.right) : forwardDir;


		// NOT SWIMMING
		if (ic.submersion < 0.8f) {
			// normalised direction vector not allowing vertical movement input
			moveDir = ProjectOnXZPlane(moveDir).normalized;
			// run more slowly through deeper water and apply partial user input
			float moveSpeed = Mathf.Lerp(landSpeed, 1f, ic.submersion) * ic.move.magnitude;
			// calculate target movement speed
			Vector3 moveTarget = moveDir * moveSpeed;
			// current movment speed on XZ plane
			Vector3 moveCurrent = ProjectOnXZPlane(rb.velocity);
			// accelerate towards target movement speed using a lerp
			Vector3 moveSmooth = Vector3.Lerp(moveCurrent, moveTarget, landAcceleration);
			// set velocity to target and reapply vertical velocity
			rb.velocity = moveSmooth + Vector3.up * rb.velocity.y;
		}
		// SWIMMING
		else {
			// calculate target movement speed, applying partial user input
			Vector3 moveTarget = moveDir.normalized * swimSpeed * ic.move.magnitude;
			// limit vertical input near surface;
			moveTarget.y = Mathf.Lerp(0.1f, moveTarget.y, ic.submersion * 5f - 4f);
			// accelerate towards target movement speed using a lerp
			Vector3 moveSmooth = Vector3.Lerp(rb.velocity, moveTarget, swimAcceleration);
			// set velocity to target
			rb.velocity = moveSmooth;
		}
	}

	// Simple projection of a vector onto the xz-plane.
	Vector3 ProjectOnXZPlane(Vector3 vec) {
		vec.y = 0.0f;
		return vec;
	}

	/*
	void OnDrawGizmos() {
		// draw grounded CheckSphere
		Gizmos.color = Color.yellow;
		float checkRadius = 0.2f;
		Vector3 checkOffset = Vector3.up * checkRadius;
		Gizmos.DrawSphere(transform.position + checkOffset, checkRadius);

		// draw runningIntoWall CheckCapsule
		Gizmos.color = Color.red;
		checkRadius = 0.1f;
		Vector3 startPos = transform.position + transform.forward * 0.3f + transform.up * 0.8f;
		Vector3 endPos = transform.position + transform.forward * 0.3f + transform.up * 1.8f;
		Gizmos.DrawSphere(startPos, checkRadius);
		Gizmos.DrawSphere(endPos, checkRadius);

		// draw Step Up CheckSphere
		Gizmos.color = Color.green;
		checkRadius = 0.2f;
		Vector3 allowedPos = transform.position + transform.forward * 0.1f + transform.up * 0.3f;
		Gizmos.DrawSphere(allowedPos, checkRadius);
	}
	*/
}