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
    [Header("Miscellaneous")]
    public float ladderEjectForce = 10f;
    public float scrollAmt = 0.25f;
    public float inputSmoothingSpeed = 15f;
    public float cameraAngle = 15f;

    [Space(10)]
    [Header("Movement Parameters")]
    public float longDropVelocity = -9f;

    [Space(10)]
    [Header("Wall Running Vals")]
    public float wallRunningForwardOffset = 0.3f;
    public float wallRunningUpOffsetStart = 0.8f;
    public float wallRunningUpOffsetEnd = 1.8f;
    public float wallRunningRadius = 0.1f;

    [Space(10)]
    [Header("Step Up Vals")]
    public float stepUpForwardOffset = 0.1f;
    public float stepUpUpOffset = 0.3f;
    public float stepUpValue = 0.1f;
}
