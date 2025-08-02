using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutPlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStatesSO _states;
    [SerializeField] private float _moveSpeed = 5.0f;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _rb.velocity = new Vector2(_states.minigameMove.x * _moveSpeed, 0f);

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
