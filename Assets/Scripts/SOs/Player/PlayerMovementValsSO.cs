using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementValues", menuName = "ScriptableObjects/Player/PlayerMovementValues")]
public class PlayerMovementValsSO : ScriptableObject
{
    [Header("Water Movement Values")]
    public float swimSpeed = 4f;
    public float swimAcceleration = 2f;
    public float maxSwimTurnSpeed = 10f;
    public float swimSpeedTurnScaler = 5f;

    [Space(10)]
    [Header("Land Movement values")]
    public float landSpeed = 6f;
    public float landAcceleration = 5f;
    public float sprintMod = 1.5f;
    public float jumpForce = 8f;

    [Space(10)]
    [Header("Drone Movement Values")]
    public float droneXSpeed = 15f;
    public float droneYSpeed = 5f;
    public float droneTiltAmt = 15f;
    public float droneTurnSpeed = 0.5f;
    public float droneDrag = 0.7f;

    [Space(10)]
    [Header("Miscellaneous")]
    public float ladderEjectForce = 10f;
    public float scrollAmt = 0.25f;
    public float inputSmoothingSpeed = 15f;
    public float cameraAngle = 15f;
    public float turnSpeed = 0.2f;
    public float lookSensitivity = 3f;

    [Space(10)]
    [Header("Movement Parameters")]
    public float longDropVelocity = -9f;

    [Space(10)]
    [Header("Wall Collision Vals")]
    public float wallForwardModifier = 0.2f;
    public float wallUpModifier = 1f;
    public float wallHorizontalModifier = 0.25f;
    public float wallCheckRadius = 0.1f;

    [Space(10)]
    [Header("Step Up Vals")]
    public float stepUpForwardOffset = 0.1f;
    public float stepUpUpOffset = 0.3f;
    public float stepUpValue = 0.1f;
}
