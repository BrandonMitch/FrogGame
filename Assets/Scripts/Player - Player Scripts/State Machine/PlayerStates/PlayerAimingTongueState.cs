using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingTongueState : PlayerState
{
    public PlayerAimingTongueState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        player.SetMovementInputs(Vector2.zero);
        Debug.Log("in aimingState");
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

        FindRightMouseInputs();
        if (rightMouseDown)
        {
            //Debug.Log("aiming");
            player.AimTongueCrossHair();
            return;
        } else if (rightMouseUp) {
            //Debug.Log("change state to throwing tongue");
            player.SpitOutTongueOnRelease();
            player.stateMachine.ChangeState(player.throwingState);
            return;
        } else {
            Debug.LogError("Error in aiming tongue state, could happen if game doesn't detect rightMouseUp or Down");
        }
    }

    public override void PhysicsUpdate()
    {
    }
}
