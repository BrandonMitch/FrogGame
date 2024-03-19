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
    bool needsToWaitForTongueToRetract;
    public override void EnterState()
    {
        needsToWaitForTongueToRetract = false;

        // If the tongue is still retracting, then we have to wait for it to retract
        if (!player.tongueStateMachine.isTongueOff())
        {
            needsToWaitForTongueToRetract = true;
        }
        else
        {
            Debug.Log("doesn't need to wait for tongue to retract");
            needsToWaitForTongueToRetract = false;
            player.tongueStateMachine.ChangeState(player.tongueAimState);
            // these lines make frog look in the direction of the mouse
            _lookDirectionForAnimation = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
            player.SetLastMoveDirection(_lookDirectionForAnimation);
        }
        Debug.Log("in player aimingState");
    }

    public override void ExitState()
    {
        Debug.Log("exit aiming state");
        activeCorotine = false;
    }

    bool activeCorotine = false;
    public override void FrameUpdate()
    {
        // we need to wait for tongue to retract if it is not off
        if (needsToWaitForTongueToRetract)
        {
            needsToWaitForTongueToRetract = !player.tongueStateMachine.isTongueOff();
        }
        if (rightMouseButton && !activeCorotine)
        {
            //Debug.Log("aiming");
            player.AimTongueCrossHair();

            if (!needsToWaitForTongueToRetract)
            {
                // these lines make frog look in the direction of the mouse
                _lookDirectionForAnimation = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
                player.SetLastMoveDirection(_lookDirectionForAnimation.normalized);
                return;
            }
        } else if (rightMouseUp) { // if the player release the aim button, the tongue will be thrown. 
            // But if we have to wait for the tongue to retract we start a coroutine
            if (!needsToWaitForTongueToRetract && !activeCorotine)
            {
                player.SpitOutTongueOnRelease();
                player.stateMachine.ChangeState(player.throwingState);
                return;
            }
            else
            {
                if (!activeCorotine)
                {
                    Debug.Log("trying to start Coroutine");
                    player.StartCoroutine(ChangeToThrowingState());
                    activeCorotine = true;
                }
            }
        } else {
            //Debug.LogError("Error in aiming tongue state, could happen if game doesn't detect rightMouseUp or Down");
        }
    }

    public override void PhysicsUpdate()
    {

    }
    private IEnumerator ChangeToThrowingState()
    {
        Debug.Break();
        while (needsToWaitForTongueToRetract)
        {

            needsToWaitForTongueToRetract = !player.tongueStateMachine.isTongueOff();
            Debug.Log("yield, needsTowaitForTongueToRetract?"+ needsToWaitForTongueToRetract);
            yield return null;
        }
        player.SpitOutTongueOnRelease();
        player.stateMachine.ChangeState(player.throwingState);
    }
}
