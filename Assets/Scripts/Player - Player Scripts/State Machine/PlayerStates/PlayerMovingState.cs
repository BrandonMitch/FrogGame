using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingState : PlayerState
{
    private Vector2 moveVec;
    public PlayerMovingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        SetMovementInputs(moveVec);
        SetLastMovementDirection(moveVec);
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        moveVec = GetCurrentMovementInputs();
        if(moveVec != Vector2.zero)
        {
            SetMovementInputs(moveVec);
            SetLastMovementDirection(moveVec);
        }
        else
        {
            SetMovementInputs(moveVec);
            player.stateMachine.ChangeState(player.slowingState);
        }
    }

    public override void PhysicsUpdate()
    {

    }
    // This is only ccalled when we leave the idle state
    public void setMoveVecToFirstInput(Vector2 firstMoveDiretion)
    {
        this.moveVec = firstMoveDiretion;
    }
}
