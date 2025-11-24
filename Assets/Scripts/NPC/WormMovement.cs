using UnityEngine;
using System.Collections.Generic;

/* 
 Movement:
 - Squid moves forward at a constant speed.
 - Adjust direction based on:
   1) Random Motion - Adds unpredictable movement.
   2) Obstacle Avoidance - Avoid obstacles within a set radius.
   3) Spawner Attraction - Prevent squid from straying too far.
 - Turn rate is limited.
 - Movement is handled via Transform (no physics). */

public class WormMovement : MonoBehaviour {
	[SerializeField] GameObject wormObject;     // worm object
	[SerializeField] Transform[] obstacles;         // Center position of obstacles to avoid
	[SerializeField] float regionRadius = 15f;       // Radius around move controller where the squid tries to stay
	[SerializeField] float regionVerticalStretch = .3f; // Flatten spawn area into a spheroid (height / width)
	[SerializeField] private bool _showGizmos = false;
	[SerializeField] float moveSpeed = 12f;
	readonly float turnSpeed = 360f;                // Rotation speed (degrees per second)
	readonly float obstacleAvoidanceRadius = 8f;    // Range at which boids start avoiding obstacles
	float noiseMoveRandomStartOffset = 0f;
	Vector3 randomOffset = Vector3.zero;            // random movement force


	float chaseTimer = 0f;      // Timer for target update
	[SerializeField] Boidy fishSchool;  // object with Boidy script for follow targets
	Transform[] targets;
	Transform chaseTarget;
	bool hasTargetList = false;


	//tail follow stuff
	[SerializeField] Transform tailStart;  // Root bone of the tail
	List<Transform> bones;		// Bone chain
	float[] boneLength;         // distances between bones
	Vector3[] bonePos;			// Current world positions of each bone


	void Start() {
		noiseMoveRandomStartOffset = Random.Range(0f, 100f);
		RandomizeMaterialValues(wormObject);
		TailInitialize();
	}

	void Update() {
		FindTarget(5f);
		Move();
		TailFollow();
	}

	void Move() {
		Transform worm = wormObject.transform;

		// 1. Random movment
		randomOffset = OscillatingNoise(3f);
		Vector3 moveDirection = randomOffset;

		// 2. Avoid obstacles within a set radius
		foreach(Transform obstacle in obstacles) {
			Vector3 towardObstacle = obstacle.position - worm.position;
			float obstacleDistance = towardObstacle.magnitude;
			if(obstacleDistance < obstacleAvoidanceRadius)
				moveDirection += towardObstacle * (obstacleDistance - obstacleAvoidanceRadius);
		}

		// don't worry about move region... use fishes!
		/*
		// 3. Keep squid within the move region
		Vector3 towardSpawner = transform.position - worm.position;
		towardSpawner.y *= 1 / regionVerticalStretch; // Adjust for flattened spawn shape
		float spawnerDistance = towardSpawner.magnitude;
		if(spawnerDistance > regionRadius)
			moveDirection += towardSpawner * (spawnerDistance - regionRadius);
		*/

		// 4. Prioritize horizontal movement (reduce vertical jitter)
		moveDirection.y *= 0.4f;

		// 5. Adjust movement towards target
		if(chaseTarget != null) {
			Vector3 towardTarget = (chaseTarget.position - worm.position).normalized;
			moveDirection += towardTarget * 2f;
			// Debug.DrawLine(worm.position, chaseTarget.position, Color.magenta);
		}

		// 6. Smoothly rotate toward the movement direction
		float turn = turnSpeed * Time.deltaTime;
		Quaternion lookRotation = Quaternion.LookRotation(moveDirection);   // forward toward moveDirection
		worm.rotation = Quaternion.RotateTowards(worm.rotation, lookRotation, turn);

		// 7. Move forward at a fixed speed
		worm.position += worm.forward * moveSpeed * Time.deltaTime;
	}

	Vector3 OscillatingNoise(float frequency) {
		float t = (Time.time + noiseMoveRandomStartOffset) * frequency;
		return new Vector3(Mathf.Sin(t), Mathf.Cos(t * 2.718f), Mathf.Sin(t * 1.618f));
	}


	void FindTarget(float chaseDuration) {
		chaseTimer += Time.deltaTime;
		if (chaseTimer < chaseDuration) return;
		chaseTimer = 0f;
		if(!hasTargetList)
			GetTargetList();
		int index = Random.Range(0, targets.Length);
		chaseTarget = targets[index];
	}
	void GetTargetList() {
		if(fishSchool != null && fishSchool.boids != null && fishSchool.boids.Length > 0) {
			targets = fishSchool.boids;
			hasTargetList = true;
		}
		else Debug.Log("can't find targets");
	}


	void TailInitialize() {
		// Gather bones
		bones = new List<Transform>();
		Transform current = tailStart;
		bones.Add(current);

		// Walk down the chain linearly
		while(current.childCount > 0) {
			current = current.GetChild(0);
			bones.Add(current);
		}

		// Setup
		int count = bones.Count;
		boneLength = new float[count];  // one extra for tailStart.parent
		bonePos = new Vector3[count + 1];

		// Cache distances:
		// [0] = parent to firstBone, then between each bone pair
		boneLength[0] = Vector3.Distance(tailStart.parent.position, bones[0].position);
		for(int i = 1; i < count; i++)
			boneLength[i] = Vector3.Distance(bones[i - 1].position, bones[i].position);

		// Initialize world positions
		bonePos[0] = tailStart.parent.position;
		for(int i = 0; i < count; i++)
			bonePos[i + 1] = bones[i].position;
	}
	void TailFollow() {
		// Always update root position (the parent of the first bone)
		bonePos[0] = tailStart.parent.position;

		// Make each bone follow the previous transform
		for(int i = 0; i < bones.Count; i++) {
			Vector3 parentPos = bonePos[i];
			float dist = boneLength[i];

			Vector3 dir = bonePos[i + 1] - parentPos;
			if(dir == Vector3.zero)
				dir = bones[i].forward;

			// Maintain fixed spacing
			Vector3 targetPos = parentPos + dir.normalized * dist;
			bonePos[i + 1] = targetPos;

			// Apply to actual bone
			bones[i].position = targetPos;
			bones[i].rotation = Quaternion.LookRotation(parentPos - targetPos);
		}
	}








	void RandomizeMaterialValues(GameObject obj) {
		Material mat = obj.GetComponentInChildren<Renderer>().material;
		mat.SetColor("_TintR", ColorBright());
		mat.SetColor("_TintG", ColorBright());
		mat.SetColor("_TintB", ColorWarmDull());
	}
	Color ColorMuddy() {
		return new Color(Random.value, Random.value, Random.value);
	}
	Color ColorBright() {
		return Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.2f, 0.8f);
	}
	Color ColorLight() {
		return Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f);
	}
	Color ColorWarmDull() {
		return Random.ColorHSV(0f, 0.5f, 0f, 0.3f, 0.3f, 1f);
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