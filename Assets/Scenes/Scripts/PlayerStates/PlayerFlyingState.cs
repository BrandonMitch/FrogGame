using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class PlayerFlyingState : PlayerState, IState
{
    public PlayerFlyingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void EnterState()
    {
        //Vector2 InitalflyingDirection = player.GetPlayerRigidBody().velocity;
        playerStateMachine.positionStatus = PositionStatus.Flying;
    }

    public override void ExitState()
    {
        player.lungingState.ResetRotation(); // this resets rotation of the player
        player.AnimateLunge(false); // makes sure we can't go into the lunge
        player.ResetColliderDirection(); // this resets the rotation of the collider
        player.AnimateRetract_Reset(); // this clears the sprite of the flying animation
        playerStateMachine.positionStatus = PositionStatus.OnTheGround;
    }

    public override void FrameUpdate()
    {
        Vector2 input = GetCurrentMovementInputs();
        if (input != Vector2.zero)
        {
            return;
        }
        else
        {
            playerStateMachine.ChangeState(player.slowingState);
        }
    }

    public override void PhysicsUpdate()
    {
        // Check distance from end of tongue every physics frame
        if (player.lungingState.DistanceCheckForForwardLunge(lastTonguePos))
        {

            playerStateMachine.ChangeState(player.slowingState);
        }
    }

    public override void ResetValues()
    {
    }

    /// <summary>
    /// Required for tracking where the last position was before our flying period ends
    /// </summary>
    /// <param name="pos">last position of the end of the tongue</param>
    public void SetLastPositionOfTongue(Vector3 pos)
    {
        lastTonguePos = pos;
    }
    Vector3 lastTonguePos;
}

