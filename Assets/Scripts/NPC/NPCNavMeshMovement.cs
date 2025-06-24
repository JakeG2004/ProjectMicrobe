using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNavMeshMovement : NavmeshMovement
{
    private Animator _anim;
    private float _moveBlendConst = 0.75f;
    private bool _talkingToPlayer;

    protected override void Start()
    {
        _anim = GetComponent<Animator>();

        base.Start();
        StartCoroutine(IControlMovementAnimation());
    }

    protected IEnumerator IFacePlayer()
    {
        Transform target = GameObject.FindGameObjectWithTag("Player").transform;

        while (_talkingToPlayer)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4.0f);

            yield return null;
        }
    }

    protected IEnumerator IControlIdleAnimation()
    {
        // Set the idle activity
        int index = Random.Range(0, 7);

        // Lerp to the animation
        float curElapsedTime = 0;
        while (curElapsedTime < 1.0f)
        {
            curElapsedTime += Time.deltaTime;
            _anim.SetFloat("Idle", curElapsedTime * index);
            yield return null;
        }

        // Snap to it
        _anim.SetFloat("Idle", (float)index);

        // Wait for a random amount of time, then start new movement
        yield return new WaitForSeconds(Random.Range(1, 5));

        // Lerp to the animation
        curElapsedTime = 0;
        while (curElapsedTime < 1.0f)
        {
            curElapsedTime += Time.deltaTime;
            _anim.SetFloat("Idle", index - (curElapsedTime * index));
            yield return null;
        }

        // Stop idling, go back to walking
        _anim.SetFloat("Idle", 0);

        PickNewMovementTarget();
        StartCoroutine(IControlMovementAnimation());
    }

    protected IEnumerator IControlMovementAnimation()
    {
        _anim.SetFloat("Idle", 0);

        // Let it pathfind
        yield return new WaitForSeconds(0.1f);

        // Initialize variables
        Vector3 lastPos = transform.position;
        float speed = 0.0f;

        // Do the bulk of the walking
        while (_agent.remainingDistance > 1.0f)
        {
            float rawSpeed = (transform.position - lastPos).magnitude / Time.deltaTime;
            speed = Mathf.Lerp(speed, rawSpeed, 0.1f);
            lastPos = transform.position;

            _anim.SetFloat("Move", speed * _moveBlendConst);
            yield return null;
        }

        // Slow down walking with speed
        while (_agent.remainingDistance > 0.01f)
        {
            _anim.SetFloat("Move", _agent.remainingDistance * _moveBlendConst);
            yield return null;
        }

        //Snap anim to 0
        _anim.SetFloat("Move", 0);

        StartCoroutine(IControlIdleAnimation());
    }

    public void SetMoveStatus(bool state)
    {
        _talkingToPlayer = !state;
        StopAllCoroutines();

        if (!state)
        {
            // Save speed and stop movement
            PauseMovement();

            _anim.SetFloat("Move", 0);
            _anim.SetFloat("Idle", 1);

            StartCoroutine(IFacePlayer());

            return;
        }

        UnpauseMovement();

        StartCoroutine(IControlMovementAnimation());
    }
}