using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutPlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    private PlayerInputActions _pia;
    private Rigidbody2D _rb;
    private Vector2 _moveVector;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        // Movement lambdas
        _pia.Minigames.Move.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        _pia.Minigames.Move.canceled += ctx => _moveVector = Vector2.zero;
    }

    void Update()
    {
        _rb.velocity = new Vector2(_moveVector.x * _moveSpeed, 0f);

        if (Mathf.Abs(transform.localPosition.x) > 3.5f)
        {
            transform.localPosition = new Vector3(Mathf.Sign(transform.localPosition.x) * 3.5f, transform.localPosition.y, transform.localPosition.z);
        }
    }

    void OnEnable()
    {
        NewInputController.Instance.SetMinigameMode();
    }
}
