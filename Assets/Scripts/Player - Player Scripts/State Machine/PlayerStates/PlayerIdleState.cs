using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType); // UHHHH IMPLEMENT
    }

    public override void EnterState()
    {
        SetMovementInputs(Vector2.zero, 0.0f);   
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        Vector2 moveVec = GetCurrentMovementInputs();
        // Check tongue first
        FindRightMouseInputs();
        FindLeftMouseInputs();
        //Debug.Log("Right mouse inputs: up " + rightMouseUp + " down " + rightMouseDown);
        if (rightMouseDown) {
            player.stateMachine.ChangeState(player.aimingTongueState);
            return;
        }
        // Then check attack inputs

        // Then check movement

        if(moveVec != Vector2.zero)
        {
            player.movingState.setMoveVecToFirstInput(moveVec);
            player.stateMachine.ChangeState(player.movingState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {

    }
}
