using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public bool isClimbing = false;
    public bool isJumping = false;
    public bool isGrounded = false;
    public bool longDrop = false;
    public bool isSprinting = false;

    [Space(10)]
    public Vector2 move = Vector2.zero;
    public Vector2 smoothedMove = Vector2.zero;
    public Vector2 turn = Vector2.zero;
    public Vector2 look = Vector2.zero;

    [Space(10)]
    public float submersion = 0f;
    public float turnAngle = 0.0f;
    public float zoom = 1.0f;
    public float lookSensitivity = 1.0f;
}
