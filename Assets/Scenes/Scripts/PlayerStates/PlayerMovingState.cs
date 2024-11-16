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
        setRunningDrag();
        SetMovementInputs(moveVec);
        SetLastMovementDirection(moveVec);
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        if (RightMouseButton)
        {
            playerStateMachine.ChangeState(player.aimingTongueState);
            SetMovementInputs(Vector2.zero);
            return;
        }
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
        movementCode4(moveVec);
    }
    public void movementCode(Vector2 moveVec)
    {
        Vector2 velocity = playerRB.velocity;

        float velocityMagnitude = velocity.magnitude;

        if (velocityMagnitude > playerMaxSpeed)
        {
            playerRB.velocity = Vector2.ClampMagnitude(velocity, playerMaxSpeed);
        }
        else
        {
            playerRB.AddForce(playerRunForceModifier * moveVec);
        }
    }
    public void movementCode2(Vector2 moveVec)
    {
        Vector2 oldvelocity = playerRB.velocity;
        float velocityMagnitude = oldvelocity.magnitude;
        Vector2 newVelocity = oldvelocity + (playerRunForceModifier * moveVec)/playerRB.mass ; // add force to the old velocity
                newVelocity = Vector2.ClampMagnitude(newVelocity, playerMaxSpeed) * playerRB.mass;
        playerRB.velocity = newVelocity;
    }

    float turnFactorScaling = 0.3f; // controls how fast you can turn around, if = 0.1, turning around instantly would only give you 1/10 acceleration compared to walking in a direction close to the one you were already running in

    public void movementCode3(Vector2 movVec) {
        float turnFactor; // number ranging from 1/2 to 1 based on the dot product of your move direction and your last direction, stops quick turn arounds
        Vector2 oldvelocity = playerRB.velocity;
        turnFactor = Vector2.Dot(oldvelocity.normalized, movVec);
        turnFactor = 0.5f * (1 + turnFactor + turnFactorScaling - turnFactor*turnFactorScaling); // this is some dot product mapping, it is a perfect circle when turnFactorScaling = 1, and looks like a mini heart when it = 0. (It's called a cardiod curve)
        playerRB.AddForce(playerRunForceModifier * moveVec * turnFactor);
        playerRB.velocity = Vector2.ClampMagnitude(playerRB.velocity, playerMaxSpeed);
    }

    public void movementCode4(Vector2 movVec)
    {
        Vector2 oldvelocity = playerRB.velocity;
        float oldvelocityMag = oldvelocity.magnitude;
  
        if (oldvelocityMag < playerMaxSpeed) {
            float turnFactor; // number ranging from 1/2 to 1 based on the dot product of your move direction and your last direction, stops quick turn arounds
            turnFactor = Vector2.Dot(oldvelocity.normalized, movVec);
            turnFactor = 0.5f * (1 + turnFactor + turnFactorScaling - turnFactor * turnFactorScaling); // this is some dot product mapping, it is a perfect circle when turnFactorScaling = 1, and looks like a mini heart when it = 0. (It's called a cardiod curve)
            /*            float newVelMag = (oldvelocity + movVec * (playerRunForceModifier * turnFactor * playerRB.mass / (Time.fixedDeltaTime))).magnitude; // find the magnitude if we were to add this force,
                        if (newVelMag > playerMaxSpeed)
                        {
                            playerRB.velocity = 0.9f * playerRB.velocity;
                        }*/
            playerRB.velocity *= 0.9f;
            playerRB.AddForce(playerRunForceModifier * moveVec * turnFactor);
        }
        else
        {
            playerRB.velocity = Vector2.ClampMagnitude(playerRB.velocity, playerMaxSpeed);
        }
    }
    // This is only ccalled when we leave the idle state
    public void setMoveVecToFirstInput(Vector2 firstMoveDiretion)
    {
        this.moveVec = firstMoveDiretion;
    }

    public void setRunningDrag()
    {
        playerRB.drag = playerRunningDrag;
    }
}
