using UnityEngine;

public class NPCMoveController : MonoBehaviour {

	[SerializeField] LayerMask collisionMask;

	Rigidbody rb;
	Animator anim;
	readonly float moveSpeed = 1.5f;
	readonly float acceleration = 3f;
	bool move = false;
	Vector3 currentVelocity = Vector3.zero;
	Vector3 currentPosition;
	Vector3 realVelocity; // used by animation
	Vector3 moveDirection;
	float attentionSpan = 0; // How long the NPC is interested in walking a direction or looking at a thing


	void Start() {
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		currentPosition = transform.position;
	}
	void FixedUpdate() {
		Wander();
		RotateBasedOnMovement(4f);
		AnimateBasedOnMovement();
	}

	void Wander() {
		// if attention has run out, set a new direction and decide whether ot move or idle for next span
		if (attentionSpan <= 0) {
			attentionSpan = Random.Range(3f, 6f);
			moveDirection = RandomDirectionOnYAxis();
			// don't idle twice in a row. if moving, coin flip to idle or change move direction (true if false before OR rolled true)
			move = !move || Random.value >= 0.5f;
			if (!move) RandomiseIdleAction();
		}
		else if (move) {
			Move(false, 6);
		}
		else {
			Idle();
		}
		attentionSpan -= Time.fixedDeltaTime;
	}
	void Idle() {
		// Debug.Log(name + " is idling.");
		rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
	}

	// move may call itself again up to maxAttempts times to try to turn if it runs into an obstacle
	void Move(bool turn, int maxAttempts) {
		if (maxAttempts <= 0) {
			// Debug.Log(name + " didn't find a move.");
			rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
			return;
		}
		if (turn) {
			// try small turns if you have many attempts left
			// try larger turns as you run out of trys
			// (6 -> 5, 5 -> 7.2, 4 -> 11.25, 3 -> 20, 2 -> 45, 1 -> 180)
			float directionAdjustmentAngle = 180f / (maxAttempts * maxAttempts);
			AdjustMoveDirection(directionAdjustmentAngle);
		}

		// collision check
		bool obstacle = false;
		Vector3 checkSphereStart = transform.position + Vector3.up * 0.45f;

		if (Physics.CheckSphere(checkSphereStart + moveDirection, 0.3f, ~collisionMask, QueryTriggerInteraction.Ignore)) {
			//// Later maybe search for player or other npcs and respond to them differently from other obstacles?
			obstacle = true;
		}
		// recursion! try again
		if (obstacle) {
			Move(true, maxAttempts-1);
			return;
		}
		// move
		Vector3 targetVelocity = moveSpeed * moveDirection;
		currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
		rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);
		// Debug.Log(name + " succesfully moved with " + maxAttempts + " attempts remaining.");
		// Debug.DrawRay(rayStart, moveDirection * checkDistance, Color.green);
	}


	void AdjustMoveDirection(float maxAdjustAngle) {
		float angleAdjust = Random.Range(-maxAdjustAngle, maxAdjustAngle);
		moveDirection = Quaternion.AngleAxis(angleAdjust, Vector3.up) * moveDirection;
	}
	Vector3 RandomDirectionOnYAxis() {
		return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
	}
	void RotateBasedOnMovement(float turnSpeed) {
		Quaternion rotToVel = Quaternion.FromToRotation(Vector3.forward, new Vector3(rb.velocity.x, 0, rb.velocity.z));
		transform.rotation = Quaternion.Lerp(transform.rotation, rotToVel, turnSpeed * Time.fixedDeltaTime);
	}
	void AnimateBasedOnMovement() {
		realVelocity = (transform.position - currentPosition) / Time.fixedDeltaTime;
		currentPosition = transform.position;
		float realMoveSpeed = realVelocity.magnitude;
		// Debug.Log("Real move speed: " + realMoveSpeed.ToString("0.##"));

		anim.SetFloat("Move", realMoveSpeed / 2.8f);
	}
	void RandomiseIdleAction() {
		int index = Random.Range(0, 7);
		anim.SetFloat("Idle", (float)index);
	}
	/*
	void OnDrawGizmos() {
		// visual of checksphere
		Gizmos.color = Color.red;
		float radius = 0.3f;
		float forward = 1f;
		float up = 0.45f;
		Vector3 Pos = transform.position + transform.forward * forward + transform.up * up;
		Gizmos.DrawSphere(Pos, radius);
	}
	*/
}