// PlayerClimbing.cs
// A script to manage player climbing
// Author:  Jake Gendreau
// Date:7/18/25

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class PlayerClimbing : MonoBehaviour
{
    private PlayerStatesSO _states;
    private PlayerMovementValsSO _vals;

    private Rigidbody _rb;
    private Coroutine _snapToLadderCoroutine;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _states = GetComponent<PlayerController>().GetStates();
        _vals = GetComponent<PlayerController>().GetVals();
    }

    void FixedUpdate()
    {
        ApplyClimbMovement();
    }

    // Jumps off the ladder early
    public void JumpFromClimb()
    {
        if (!_states.isClimbing)
        {
            return;
        }

        _rb.velocity = new Vector3(0f, _vals.ladderEjectForce, 0f);

        EndClimb();
    }

    public void ApplyClimbMovement()
    {
        if (!_states.isClimbing || _states.isJumping)
        {
            return;
        }

        // Apply upward velocity to "climb" and reduce movement speed in all other directions
        _rb.velocity = new Vector3(_rb.velocity.x * 0.1f, _vals.jumpForce, _rb.velocity.z * 0.1f);

        // If submerged, adjust position to move up more easily
        if (_states.submersion > 0f)
        {
            _rb.position += Vector3.up * 0.1f;
        }
    }

    public void SetClimbPos(Transform ladder)
    {
        _rb.velocity = Vector2.zero;
        _states.move = Vector2.zero;
        _states.smoothedMove = Vector2.zero;

        // Stop any existing snap coroutine
        if (_snapToLadderCoroutine != null)
        {
            StopCoroutine(_snapToLadderCoroutine);
        }

        // Calculate new target rotation and position
        Vector3 newRot = ladder.eulerAngles;
        newRot.x = (newRot.x + 90f) % 360;

        // Prevent rotation more than 180 degrees
        if (transform.rotation.x - newRot.x > 180)
        {
            newRot.x -= 360;
        }

        // Calculate the pos that the player should be, but use current player height
        Vector3 newPos = ladder.position + ladder.up * 0.2f;
        newPos.y = transform.position.y;

        _snapToLadderCoroutine = StartCoroutine(SnapToLadder(newPos, newRot));
    }

    private IEnumerator SnapToLadder(Vector3 targetPos, Vector3 targetRot)
    {
        float elapsedTime = 0.0f;
        float totalTime = 0.1f;

        Vector3 initPos = transform.position;
        Quaternion initRot = transform.rotation;
        Quaternion finalRot = Quaternion.Euler(targetRot);

        // Set the state
        _states.isClimbing = true;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / totalTime;

            _rb.MovePosition(Vector3.Lerp(initPos, targetPos, ratio));
            _rb.MoveRotation(Quaternion.Slerp(initRot, finalRot, ratio));

            yield return null;
        }

        // Snap to final pos
        transform.position = targetPos;
        transform.rotation = finalRot;

        // Update the rigidbody
        _rb.velocity = Vector3.zero;
        _rb.useGravity = false;

        // Reset the coroutine
        _snapToLadderCoroutine = null;
    }

    public void EndClimb()
    {
        if (!_states.isClimbing)
        {
            return;
        }

        _states.isClimbing = false;
        _rb.useGravity = true;
    }
}
