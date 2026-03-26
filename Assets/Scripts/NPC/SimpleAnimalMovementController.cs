using UnityEngine;

public class SimpleAnimalMovementController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float directionChangeTime = 3f;
    [SerializeField] float turnSpeed = 4f;
    [SerializeField] private float _animSpeedScaler = 0.45f;

    Rigidbody rb;
    Animator anim;

    Vector3 homePos;
    Vector3 moveDirection;
    float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        homePos = transform.position;
        PickNewDirection();
    }

    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

        if (timer <= 0f)
        {
            PickNewDirection();
        }

        Move();
        Rotate();
        Animate();
    }

    void PickNewDirection()
    {
        timer = directionChangeTime;

        // Random horizontal direction
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        // If too far from home, bias back toward center
        Vector3 toHome = homePos - transform.position;
        if (toHome.magnitude > wanderRadius)
        {
            moveDirection = toHome.normalized;
        }
    }

    void Move()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    void Rotate()
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            turnSpeed * Time.fixedDeltaTime
        );
    }

    void Animate()
    {
        float speed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        anim.SetFloat("Speed", speed * _animSpeedScaler);
    }
}