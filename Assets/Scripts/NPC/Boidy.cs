using UnityEngine;
using System.Collections;

/* Boidy - Manages the spawning and movement of boids.
 
 Spawning:
 - Spawns a set number of boids randomly within a squashed spherical area.
 - Applies random scale, speed, and material variations.

 Movement:
 - Boids move forward at a constant speed.
 - Adjust direction based on:
   1) Random Motion - Adds slight unpredictable movement.
   2) Neighbor Distance - Maintain an ideal separation from the nearest boid.
   3) Obstacle Avoidance - Avoid obstacles within a set radius.
   4) Spawner Attraction - Prevent boids from straying too far.
 - Turn rate is limited.
 - Movement is handled via Transform (no physics). */

public class Boidy : MonoBehaviour {
    [SerializeField] GameObject boidPrefab;         // Boid object
    [SerializeField] Transform[] obstacles;         // Center position of obstacles to avoid
    [SerializeField] int boidCount = 10;            // Number of boids to spawn
    [SerializeField] float spawnRadius = 15f;       // Radius around the spawner where boids try to stay
    [SerializeField] private bool _spawnOnStart = true;
    [SerializeField] float spawnVerticalStretch = .3f;     // Flatten spawn area into a spheroid (height / width)
    [SerializeField] private bool _showGizmos = false;
	[SerializeField] Vector2 moveSpeedRange = Vector2.up;
    readonly float turnSpeed = 360f;                // Rotation speed (degrees per second)
    readonly float neighborDistanceGoal = 3f;		// Ideal distance between neighbors
    readonly float obstacleAvoidanceRadius = 8f;    // Range at which boids start avoiding obstacles
    readonly float neighborCheckTime = 0.2f;        // Time interval for updating nearest neighbors

	float noiseMoveRandomStartOffset = 0f;
    float timer = 1f;                               // Timer for neighbor updates
    Vector3 randomOffset = Vector3.zero;            // Shared random movement force
    [HideInInspector] public Transform[] boids;                              // All spawned boids
    Transform[] neighbors;                          // The nearest neighbor of each boid
    float[] neighborDistances;                      // Distance between each boid and it's neighbor
    float[] moveSpeeds;                             // Movement speed of each boid
    private float _numBoids;
    void Start() {
        boids = new Transform[boidCount];
        neighbors = new Transform[boidCount];
        neighborDistances = new float[boidCount];
        moveSpeeds = new float[boidCount];
		noiseMoveRandomStartOffset = Random.Range(0f, 100f);

		if (_spawnOnStart)
        {
            SpawnBoids();   
        }
    }

    public void SlowSpawnBoids()
    {
        StartCoroutine(ISlowSpawnBoids());
    }

    private IEnumerator ISlowSpawnBoids()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
            spawnPosition.y *= spawnVerticalStretch; // Apply vertical squashing
            spawnPosition += transform.position;

            GameObject boid = Instantiate(boidPrefab, spawnPosition, Quaternion.identity);

            // Apply random scale, speed, and material variance
            float scale = Random.Range(0.5f, 1.5f);
            boid.transform.localScale = Vector3.one * scale;
            moveSpeeds[i] = Random.Range(moveSpeedRange.x, moveSpeedRange.y);
            RandomizeMaterialValues(boid);

            _numBoids++;
            boids[i] = boid.transform;

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Update() {
        FindNearestBoids();
        MoveBoids();
    }

    void SpawnBoids() {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
            spawnPosition.y *= spawnVerticalStretch; // Apply vertical squashing
            spawnPosition += transform.position;

            GameObject boid = Instantiate(boidPrefab, spawnPosition, Quaternion.identity);

            // Apply random scale, speed, and material variance
            float scale = Random.Range(0.5f, 1.5f);
            boid.transform.localScale = Vector3.one * scale;
            moveSpeeds[i] = Random.Range(4f, 10f);
            RandomizeMaterialValues(boid);

            boids[i] = boid.transform;
            _numBoids++;
        }
    }

	void RandomizeMaterialValues(GameObject obj) {
		Material mat = obj.GetComponentInChildren<Renderer>().material;
		// GetComponent<Renderer>().material;
		mat.SetColor("_TintR", ColorMuddy());
		mat.SetColor("_TintG", ColorMuddy());
		mat.SetColor("_TintB", ColorMuddy());
		// mat.SetColor("_TintA", ColorWarmBright());
	}
	Color ColorMuddy() {
		return new Color(Random.value, Random.value, Random.value);
	}
	Color ColorWarmBright() {
		return Random.ColorHSV(0.01f, 0.2f, 0.7f, 1f, 1f, 1f);
	}


	void FindNearestBoids() {
        timer += Time.deltaTime;
        if (timer < neighborCheckTime) return;

        for (int i = 0; i < _numBoids; i++) {
            Transform nearestBoid = null;
            float nearestDistance = float.MaxValue;

            for (int j = 0; j < _numBoids; j++) {
                if (i == j) continue;

                float distance = Vector3.SqrMagnitude(boids[j].position - boids[i].position);
                if (distance < nearestDistance) {
                    nearestDistance = distance;
                    nearestBoid = boids[j];
                }
            }
            neighbors[i] = nearestBoid;
            neighborDistances[i] = Mathf.Sqrt(nearestDistance);
        }
    }

    void MoveBoids() {
        float turn = turnSpeed * Time.deltaTime;
        randomOffset = OscillatingNoise(0.2f);

        for (int i = 0; i < _numBoids; i++) {
            Transform boid = boids[i];
            
            // 1. Shared random movment
            Vector3 moveDirection = randomOffset;

            // 2. Adjust movement based on nearest neighbor
            if (neighbors[i] != null) {
                Vector3 towardNeighbor = (neighbors[i].position - boid.position).normalized;
                moveDirection += towardNeighbor * (neighborDistances[i] - neighborDistanceGoal);
            }

            // 3. Avoid obstacles within a set radius
            foreach (Transform obstacle in obstacles) {
                Vector3 towardObstacle = obstacle.position - boid.position;
                float obstacleDistance = towardObstacle.magnitude;
                if (obstacleDistance < obstacleAvoidanceRadius) {
                    moveDirection += towardObstacle * (obstacleDistance - obstacleAvoidanceRadius);
                }
            }

            // 4. Keep boids within the spawn area
            Vector3 towardSpawner = transform.position - boid.position;
            towardSpawner.y *= 1 / spawnVerticalStretch; // Adjust for flattened spawn shape
            float spawnerDistance = towardSpawner.magnitude;
            if (spawnerDistance > spawnRadius) {
                moveDirection += towardSpawner * (spawnerDistance - spawnRadius);
            }

            // 5. Prioritize horizontal movement (reduce vertical jitter)
            moveDirection.y *= 0.3f;

            // 6. Smoothly rotate toward the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            boid.rotation = Quaternion.RotateTowards(boid.rotation, targetRotation, turn);

            // 7. Move forward at a fixed speed
            boid.position += boid.forward * moveSpeeds[i] * Time.deltaTime;
        }
    }

    Vector3 OscillatingNoise(float frequency) {
        float t = (Time.time + noiseMoveRandomStartOffset) * frequency;
        return new Vector3(Mathf.Sin(t), Mathf.Cos(t * 2.718f), Mathf.Sin(t * 1.618f));
    }

    
    void OnDrawGizmos() {
        if (!_showGizmos)
        {
            return;
        }
        
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // Create a scaled transformation matrix to draw the spawn spheroid
        Vector3 posYAdjusted = new Vector3(0f, transform.position.y * (1 - spawnVerticalStretch), 0f);
        Gizmos.matrix = Matrix4x4.TRS(posYAdjusted, Quaternion.identity, new Vector3(1, spawnVerticalStretch, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        // Restore the original Gizmos matrix
        Gizmos.matrix = originalMatrix;
        // Draw obstacle avoidance radius for each obstacle
        Gizmos.color = Color.red;
        foreach (Transform obstacle in obstacles) {
            Gizmos.DrawWireSphere(obstacle.position, obstacleAvoidanceRadius);
        }
    }
    
}