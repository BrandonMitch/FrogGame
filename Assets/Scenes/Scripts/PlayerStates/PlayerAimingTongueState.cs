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
        activeAimCoroutine = false;
        activeThrowCoroutine = false;

        if (!activeAimCoroutine)
        {
            aimCoroutine = player.StartCoroutine(ChangeToTongueAimingState());
        }
/*
        // If the tongue is still retracting, then we have to wait for it to retract
        player.tongueStateMachine.ChangeState(player.tongueAimState);
        // these lines make frog look in the direction of the mouse
        _lookDirectionForAnimation = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
        player.SetLastMoveDirection(_lookDirectionForAnimation);
*/
    }

    public override void ExitState()
    {
        activeAimCoroutine = false;
        activeThrowCoroutine = false;
        aimCoroutine = null;
        throwCoroutine = null;
        Debug.Log("exit aiming state");
    }


    public override void FrameUpdate()
    {
        /// new code
        if (fKeyDown)
        {
            StopCoroutines();
            TryToDestroyEndOfTongueAndChangeToOffState();
            playerStateMachine.ChangeState(player.idleState);
            return;
        }
        if (rightMouseButton) // aim while right mouse button is down
        {
            player.AimTongueCrossHair();
            // these lines make frog look in the direction of the mouse
            _lookDirectionForAnimation = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
            player.SetLastMoveDirection(_lookDirectionForAnimation.normalized);
        } else if (rightMouseUp) // if mouse is let go, start coroutine 
        {
            if (!activeThrowCoroutine)
            {
                player.StartCoroutine(ChangeToThrowingState());
            }
        }
    }
    private bool activeAimCoroutine = false;
    private bool activeThrowCoroutine = false;
    private Coroutine aimCoroutine = null;
    private Coroutine throwCoroutine = null;
    private IEnumerator ChangeToTongueAimingState()
    {
        activeAimCoroutine = true;
        // If the tongue is still retracting, then we have to wait for it to retract
        while (!player.tongueStateMachine.isTongueOff()) // while the tongue is not off, wait for it to be off
        {
            yield return new WaitForFixedUpdate(); // wait a fixed update frame
        }
        // After the tongue is off change the tongue to the aiming state
        player.tongueStateMachine.ChangeState(player.tongueAimState);
        activeAimCoroutine = false;
    }
    private IEnumerator ChangeToThrowingState()
    {
        activeThrowCoroutine = true;
        while (!player.tongueStateMachine.isTongueAiming()) // while the tongue is not aiming wait for it to be aiming
        {
            yield return new WaitForFixedUpdate(); // wait a fixed update frame
        }
        player.SpitOutTongueOnRelease();
        player.stateMachine.ChangeState(player.throwingState);
        activeThrowCoroutine = false;
    }
    private void StopCoroutines()
    {
        if (aimCoroutine != null)
        {
            player.StopCoroutine(aimCoroutine);
        }
        if (throwCoroutine != null)
        {
            player.StopCoroutine(throwCoroutine);
        }
    }
    private void TryToDestroyEndOfTongueAndChangeToOffState()
    {
        if (player.tongueStateMachine.isTongueAiming())
        {
            player.tongueStateMachine.DestroyEndOfTongue();
            player.tongueStateMachine.ChangeState(player.tongueOffState);
        }
    }
    public override void PhysicsUpdate()
    {

    }
}
