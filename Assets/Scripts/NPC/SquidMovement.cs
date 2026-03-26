using UnityEngine;

/* 
 Movement:
 - Squid moves forward at a constant speed.
 - Adjust direction based on:
   1) Random Motion - Adds unpredictable movement.
   2) Obstacle Avoidance - Avoid obstacles within a set radius.
   3) Spawner Attraction - Prevent squid from straying too far.
 - Turn rate is limited.
 - Movement is handled via Transform (no physics). */

public class SquidMovement : MonoBehaviour {
	[SerializeField] GameObject squidObject;		// Squid object
	[SerializeField] Transform[] obstacles;         // Center position of obstacles to avoid
	[SerializeField] float regionRadius = 15f;       // Radius around move controller where the squid tries to stay
	[SerializeField] float regionVerticalStretch = .3f;	// Flatten spawn area into a spheroid (height / width)
	[SerializeField] private bool _showGizmos = false;
	[SerializeField] float moveSpeed = 4f;
	Animator squidAnim;
	readonly float turnSpeed = 360f;                // Rotation speed (degrees per second)
	readonly float obstacleAvoidanceRadius = 8f;    // Range at which boids start avoiding obstacles
	float noiseMoveRandomStartOffset = 0f;
	Vector3 randomOffset = Vector3.zero;            // random movement force
	Quaternion prevRot = Quaternion.identity;
	float smoothAngularRot = 0f;					// used for animation controller speed
	Vector3 smoothAngularVel = Vector3.zero;        // used for animation controller turns
	Vector3 prevAngularVelocityAxis = Vector3.up;
	[SerializeField] private Transform _upperPlane; 

	void Start() {
		squidAnim = squidObject.GetComponent<Animator>();
		noiseMoveRandomStartOffset = Random.Range(0f, 100f);
		RandomizeMaterialValues(squidObject);
	}

	void Update() {
		Move();
	}

	void Move() {
		Transform squid = squidObject.transform;

		// 1. Random movment
		randomOffset = OscillatingNoise(0.4f);
		Vector3 moveDirection = randomOffset;

		// 2. Avoid obstacles within a set radius
		foreach(Transform obstacle in obstacles) {
			Vector3 towardObstacle = obstacle.position - squid.position;
			float obstacleDistance = towardObstacle.magnitude;
			if(obstacleDistance < obstacleAvoidanceRadius)
				moveDirection += towardObstacle * (obstacleDistance - obstacleAvoidanceRadius);
		}

		// 3. Keep squid within the move region
		Vector3 towardSpawner = transform.position - squid.position;
		towardSpawner.y *= 1 / regionVerticalStretch; // Adjust for flattened spawn shape
		float spawnerDistance = towardSpawner.magnitude;
		if(spawnerDistance > regionRadius)
			moveDirection += towardSpawner * (spawnerDistance - regionRadius);

		// 4. Prioritize horizontal movement (reduce vertical jitter)
		moveDirection.y *= 0.3f;

		// 5. Smoothly rotate toward the movement direction
		float turn = turnSpeed * Time.deltaTime;
		Quaternion lookRotation = Quaternion.LookRotation(moveDirection);   // forward toward moveDirection
		Quaternion targetRotation = lookRotation * Quaternion.Euler(-90f, 0f, 0f); // rotate up to face toward movement
		squid.rotation = Quaternion.RotateTowards(squid.rotation, targetRotation, turn);

		// 6. Calculate angular velocity for use in animations
		Quaternion deltaRot = squid.rotation * Quaternion.Inverse(prevRot);
		prevRot = squid.rotation;	// Save for next frame
		deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
		if (angle > 180f)			// ensure continuity by flipping the axis if necessary
    		angle -= 360f;
		if (Vector3.Dot(axis, prevAngularVelocityAxis) < 0f){	// Sometimes the axis can flip sign arbitrarily; fix it using dot product
			axis = -axis;
			angle = -angle;
		}
		prevAngularVelocityAxis = axis;
		Vector3 angularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;

		// 7. Apply smoothAngularRot as movement speed in animator
		float clampedAngularVel = Mathf.Clamp(angularVelocity.sqrMagnitude, 0f, 2f);
		smoothAngularRot = Mathf.Lerp(smoothAngularRot, clampedAngularVel, Time.deltaTime);
		squidAnim.SetFloat("Speed", smoothAngularRot);

		// 8. Apply smoothAngulrVel as turn speeds in animator
		smoothAngularVel.x = Mathf.LerpAngle(smoothAngularVel.x, angularVelocity.x, 5 * Time.deltaTime);
		smoothAngularVel.y = Mathf.LerpAngle(smoothAngularVel.y, angularVelocity.y, 5 * Time.deltaTime);
		smoothAngularVel.z = Mathf.LerpAngle(smoothAngularVel.z, angularVelocity.z, 5 * Time.deltaTime);
		 // Debug.Log("Angular Velocity: " + smoothAngularVel.ToString());
		 // Debug.DrawRay(squid.position, Vector3.up * smoothAngularVel.y, Color.green);
		squidAnim.SetFloat("y", -smoothAngularVel.y);
		squidAnim.SetFloat("z", -smoothAngularVel.z);

		// 9. Move forward at a fixed speed
		squid.position += squid.up * moveSpeed * Time.deltaTime;

		if(transform.position.y >= _upperPlane.position.y)
        {
			transform.position = new Vector3(transform.position.x, _upperPlane.position.y, transform.position.z);
        }
	}

	Vector3 OscillatingNoise(float frequency) {
		float t = (Time.time + noiseMoveRandomStartOffset) * frequency;
		return new Vector3(Mathf.Sin(t), Mathf.Cos(t * 2.718f), Mathf.Sin(t * 1.618f));
	}









	void RandomizeMaterialValues(GameObject obj) {
		Renderer rend = obj.GetComponentInChildren<Renderer>();
		if (rend == null) // couldn't find renderer
			return;
		Material[] mats = rend.materials;
		if (mats.Length < 2) // couldn't find all materials
			return;
		// main at index 0
		Color bodyColor = ColorWarm();
		mats[0].SetColor("_TintR", bodyColor);
		Color headColor = ColorWarm();
		mats[0].SetColor("_TintG", headColor);
		Color eyeColor = ColorDark();
		mats[0].SetColor("_TintB", ColorDark());
		// eye at index 1
		eyeColor.a = Random.Range(0.3f,1f);
		mats[1].SetColor("_Color", eyeColor);
		mats[1].SetColor("_FresColor", ColorBright());
		// bulb at index 2
		bodyColor.a = 0;
		mats[2].SetColor("_TintR", bodyColor);
		headColor.a = Random.Range(0.5f,1f);
		mats[2].SetColor("_TintG", headColor);
		headColor.a = 0.26f;
		mats[2].SetColor("_FresColor", headColor);
	}
	Color ColorWarm() {
		return new Color(1 - Mathf.Pow(Random.value,2), Random.value, Mathf.Pow(Random.value,2));
	}
	Color ColorMuddy() {
		return new Color(Random.value, Random.value, Random.value);
	}
	Color ColorWarmBright() {
		return Random.ColorHSV(0.01f, 0.2f, 0.7f, 1f, 1f, 1f);
	}
	Color ColorDark() {
		return Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 0.5f);
	}
	Color ColorBright() {
		return Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f, 0.2f, 1f);
	}


	void OnDrawGizmos() {
		if(!_showGizmos)
			return;
		Matrix4x4 originalMatrix = Gizmos.matrix;
		// Create a scaled transformation matrix to draw the region spheroid
		Vector3 posYAdjusted = new Vector3(0f, transform.position.y * (1 - regionVerticalStretch), 0f);
		Gizmos.matrix = Matrix4x4.TRS(posYAdjusted, Quaternion.identity, new Vector3(1, regionVerticalStretch, 1));
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, regionRadius);
		// Restore the original Gizmos matrix
		Gizmos.matrix = originalMatrix;
		// Draw obstacle avoidance radius for each obstacle
		Gizmos.color = Color.red;
		foreach(Transform obstacle in obstacles)
			Gizmos.DrawWireSphere(obstacle.position, obstacleAvoidanceRadius);
	}
}