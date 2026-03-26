using UnityEngine;
using System.Collections;

public class Boidy : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] GameObject[] boidPrefabs;   // Multiple prefabs
    [SerializeField] int boidCount = 10;
    [SerializeField] float spawnRadius = 15f;
    [SerializeField] float spawnVerticalStretch = 0.3f;
    [SerializeField] bool spawnOnStart = true;

    [Header("Movement")]
    [SerializeField] Vector2 moveSpeedRange = new Vector2(4f, 10f);
    [SerializeField] Transform[] obstacles;

    [Header("Debug")]
    [SerializeField] bool showGizmos = false;

    readonly float turnSpeed = 360f;
    readonly float neighborDistanceGoal = 3f;
    readonly float obstacleAvoidanceRadius = 8f;
    readonly float neighborCheckTime = 0.2f;

    public Transform[] boids;
    Transform[] neighbors;
    float[] neighborDistances;
    float[] moveSpeeds;

    int numBoids = 0;

    float timer = 0f;
    float noiseOffset;
    Vector3 randomOffset;

    void Start()
    {
        boids = new Transform[boidCount];
        neighbors = new Transform[boidCount];
        neighborDistances = new float[boidCount];
        moveSpeeds = new float[boidCount];

        noiseOffset = Random.Range(0f, 100f);

        if (spawnOnStart)
            SpawnBoids();
    }

    GameObject GetRandomPrefab()
    {
        if (boidPrefabs == null || boidPrefabs.Length == 0)
        {
            Debug.LogError("No boid prefabs assigned!");
            return null;
        }

        return boidPrefabs[Random.Range(0, boidPrefabs.Length)];
    }

    public void SpawnBoids()
    {
        for (int i = 0; i < boidCount; i++)
        {
            SpawnSingleBoid(i);
        }
    }

    public void SlowSpawnBoids()
    {
        StartCoroutine(ISlowSpawn());
    }

    IEnumerator ISlowSpawn()
    {
        for (int i = 0; i < boidCount; i++)
        {
            SpawnSingleBoid(i);
            yield return new WaitForSeconds(5f);
        }
    }

    void SpawnSingleBoid(int index)
    {
        GameObject prefab = GetRandomPrefab();
        if (prefab == null) return;

        Vector3 pos = Random.insideUnitSphere * spawnRadius;
        pos.y *= spawnVerticalStretch;
        pos += transform.position;

        GameObject boid = Instantiate(prefab, pos, Quaternion.identity);

        float scale = Random.Range(0.5f, 1.5f);
        boid.transform.localScale = Vector3.one * scale;

        moveSpeeds[index] = Random.Range(moveSpeedRange.x, moveSpeedRange.y);

        RandomizeMaterialValues(boid);

        boids[index] = boid.transform;
        numBoids++;
    }

    void Update()
    {
        FindNearestBoids();
        MoveBoids();
    }

    void FindNearestBoids()
    {
        timer += Time.deltaTime;
        if (timer < neighborCheckTime) return;

        timer = 0f;

        for (int i = 0; i < numBoids; i++)
        {
            Transform nearest = null;
            float nearestDist = float.MaxValue;

            for (int j = 0; j < numBoids; j++)
            {
                if (i == j) continue;

                float dist = Vector3.SqrMagnitude(boids[j].position - boids[i].position);

                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = boids[j];
                }
            }

            neighbors[i] = nearest;
            neighborDistances[i] = Mathf.Sqrt(nearestDist);
        }
    }

    void MoveBoids()
    {
        float turn = turnSpeed * Time.deltaTime;
        randomOffset = OscillatingNoise(0.2f);

        for (int i = 0; i < numBoids; i++)
        {
            Transform boid = boids[i];

            Vector3 moveDir = randomOffset;

            // Neighbor influence
            if (neighbors[i] != null)
            {
                Vector3 toNeighbor = (neighbors[i].position - boid.position).normalized;
                moveDir += toNeighbor * (neighborDistances[i] - neighborDistanceGoal);
            }

            // Obstacle avoidance
            foreach (Transform obstacle in obstacles)
            {
                Vector3 toObstacle = obstacle.position - boid.position;
                float dist = toObstacle.magnitude;

                if (dist < obstacleAvoidanceRadius)
                {
                    moveDir += toObstacle * (dist - obstacleAvoidanceRadius);
                }
            }

            // Stay near spawner
            Vector3 toCenter = transform.position - boid.position;
            toCenter.y *= 1f / spawnVerticalStretch;

            float centerDist = toCenter.magnitude;

            if (centerDist > spawnRadius)
            {
                moveDir += toCenter * (centerDist - spawnRadius);
            }

            // Reduce vertical jitter
            moveDir.y *= 0.3f;

            if (moveDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                boid.rotation = Quaternion.RotateTowards(boid.rotation, targetRot, turn);
            }

            boid.position += boid.forward * moveSpeeds[i] * Time.deltaTime;
        }
    }

    public void ResetBoidPositions(Vector3 newPos)
    {
        foreach (Transform boid in boids)
        {
            if (boid != null)
                boid.parent = transform;
        }

        transform.position = newPos;
    }

    Vector3 OscillatingNoise(float frequency)
    {
        float t = (Time.time + noiseOffset) * frequency;

        return new Vector3(
            Mathf.Sin(t),
            Mathf.Cos(t * 2.718f),
            Mathf.Sin(t * 1.618f)
        );
    }

    void RandomizeMaterialValues(GameObject obj)
    {
        Renderer r = obj.GetComponentInChildren<Renderer>();
        if (r == null) return;

        Material mat = r.material;

        mat.SetColor("_TintR", RandomColor());
        mat.SetColor("_TintG", RandomColor());
        mat.SetColor("_TintB", RandomColor());
    }

    Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Matrix4x4 original = Gizmos.matrix;

        Vector3 offset = new Vector3(0f, transform.position.y * (1 - spawnVerticalStretch), 0f);
        Gizmos.matrix = Matrix4x4.TRS(offset, Quaternion.identity, new Vector3(1, spawnVerticalStretch, 1));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.matrix = original;

        Gizmos.color = Color.red;
        if (obstacles != null)
        {
            foreach (Transform obs in obstacles)
            {
                if (obs != null)
                    Gizmos.DrawWireSphere(obs.position, obstacleAvoidanceRadius);
            }
        }
    }
}