using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    #region Movement Variables
    protected float playerSpeed;
    protected float playerMaxSpeed;
    protected float playerRestingDrag;
    protected float playerRunningDrag;
    protected float playerDragSlowDownTime;
    protected float playerRunForceModifier;

    protected void getMovementVaraibles()
    {
        float[] vars = player.getMovementVaraibles();
        playerSpeed = vars[0];
        playerMaxSpeed = vars[1];
        playerRestingDrag = vars[2];
        playerRunningDrag = vars[3];
        playerDragSlowDownTime = vars[4];
        playerRunForceModifier = vars[5];
    }
    #endregion
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
        if (leftMouseDown)
        {
            player.stateMachine.ChangeState(player.attackChargingState);
            return;
        }
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
