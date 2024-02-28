using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingTongueState : PlayerState
{
    private Vector3 _lookDirectionForAnimation;
    public PlayerAimingTongueState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        player.tongueStateMachine.ChangeState(player.tongueAimState);
        
        // these lines make frog look in the direction of the mouse
        _lookDirectionForAnimation = (player.GetCrossHairPosition()-player.GetPosition()).normalized;
        player.SetLastMoveDirection(_lookDirectionForAnimation);

        Debug.Log("in player aimingState");
    }

    public override void ExitState()
    {
        Debug.Log("exit aiming state");
    }

    public override void FrameUpdate()
    {

        FindRightMouseInputs();
        if (rightMouseDown)
        {
            //Debug.Log("aiming");
            player.AimTongueCrossHair();

            // these lines make frog look in the direction of the mouse
            _lookDirectionForAnimation = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
            player.SetLastMoveDirection(_lookDirectionForAnimation.normalized);
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
