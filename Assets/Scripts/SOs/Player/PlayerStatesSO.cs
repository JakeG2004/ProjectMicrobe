// PlayerStates.cs
// A scriptable object which manages player states
// Author:  Jake Gendreau
// Date:    7/18/25

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStates", menuName = "ScriptableObjects/Player/PlayerStates")]
public class PlayerStatesSO : ScriptableObject
{
    [Header("Player Movement Vals")]
    public PlayerMovementValsSO movementVals;

    [Space(10)]
    [Header("Movement Flags")]
    public bool isClimbing = false;
    public bool isJumping = false;
    public bool isGrounded = false;
    public bool longDrop = false;
    public bool isSprinting = false;
    public bool isFlying = false;
    public bool runningIntoWall = false;
    public bool isBeingCarried = false;
    public bool playerCanMove = true;

    [Space(10)]
    [Header("Movement Vectors")]
    public Vector2 move = Vector2.zero;
    public Vector2 smoothedMove = Vector2.zero;
    public Vector2 turn = Vector2.zero;
    public Vector2 look = Vector2.zero;

    [Space(10)]
    [Header("Drone Vectors")]
    public float verticalMove = 0;

    [Space(10)]
    [Header("Environmental Factors")]
    public float submersion = 0f;

    [Space(10)]
    [Header("Animation / Visual")]
    public float turnAngle = 0.0f;
    public float zoom = 1.0f;
}