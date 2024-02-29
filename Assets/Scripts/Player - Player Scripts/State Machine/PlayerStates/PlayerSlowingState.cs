using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowingState : PlayerState
{
    private Rigidbody2D playerRB;
    private float enterTime;
    private float dT;
    private float error = 0.1f;
    public PlayerSlowingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        playerRB = player.GetPlayerRigidBody();
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        getMovementVaraibles();
        enterTime = Time.time;
        //Debug.Log("Entered Slowing State");
    }

    public override void ExitState()
    {
        //Debug.Log("Left Slowing State");
        dT = 0;
    }

    public override void FrameUpdate()
    {
        Vector2 input = GetCurrentMovementInputs();
        if (input != Vector2.zero)
        {
            playerStateMachine.ChangeState(player.movingState);
        }
    }

    public override void PhysicsUpdate()
    {
        float velocityMagnitude = playerRB.velocity.magnitude;
        if ( velocityMagnitude < error )
        {
            playerRB.velocity = Vector2.zero;
            playerStateMachine.ChangeState(player.idleState);
        }
        else
        {
            decelerate();
        }
    }
    public void decelerate()
    {
        dT = (Time.time - enterTime) / (playerDragSlowDownTime);
        playerRB.drag = Mathf.Lerp(playerRunningDrag, playerRestingDrag, dT);
    }
}
