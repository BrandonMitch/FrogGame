using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingState : PlayerState
{
    private Vector2 moveVec;
    private Rigidbody2D playerRB;

    #region Movement Variables
    static protected float playerSpeed;
    static protected float playerMaxSpeed;
    static protected float playerRestingDrag;
    static protected float playerRunningDrag;
    static protected float playerDragSlowDownTime;
    static protected float playerRunForceModifier;
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
    public PlayerMovingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        playerRB = player.GetPlayerRigidBody();
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        getMovementVaraibles(); // TODO: Remove this, only needed for testing
        playerRB.drag = playerRunningDrag;
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
        Vector2 velocity = playerRB.velocity;

        float velocityMagnitude = velocity.magnitude;

        if(velocityMagnitude > playerMaxSpeed)
        {
            playerRB.velocity = Vector2.ClampMagnitude(velocity, playerMaxSpeed);
        }
        else
        {
            playerRB.AddForce(playerRunForceModifier*moveVec);
        }
    }
    // This is only ccalled when we leave the idle state
    public void setMoveVecToFirstInput(Vector2 firstMoveDiretion)
    {
        this.moveVec = firstMoveDiretion;
    }


}
