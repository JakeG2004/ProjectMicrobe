// PlayerAnimationController.cs
// A script for managing the animation state of the player based on the current controls and states
// Author:  Jake Gendreau
// Date:    7/15/25

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerStatesSO _states;
    private Animator _anim;

    // Animator Parameter names string consts
    private const string MOVE_X_PARAM = "MoveX";
    private const string MOVE_Y_PARAM = "MoveY";
    private const string IS_GROUNDED_PARAM = "IsGrounded";
    private const string IS_SPRINTING_PARAM = "IsSprinting";
    private const string IS_JUMPING_PARAM = "IsJumping";
    private const string IS_CLIMBING_PARAM = "IsClimbing";
    private const string SUBMERSION_PARAM = "Submersion";
    private const string TURN_ANGLE_PARAM = "TurnAngle";
    private const string RUNNING_INTO_WALL_PARAM = "RunningIntoWall";
    private const string LONG_DROP_PARAM = "LongDrop";

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (_anim == null)
        {
            return;
        }

        _anim.SetFloat(MOVE_X_PARAM, _states.smoothedMove.x);
    }
}
