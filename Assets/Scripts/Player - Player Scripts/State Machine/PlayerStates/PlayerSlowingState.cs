using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowingState : PlayerState
{

    public PlayerSlowingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        Debug.Log("Entered Slowing State");
    }

    public override void ExitState()
    {
        Debug.Log("Left Slowing State");
    }

    public override void FrameUpdate()
    {
        playerStateMachine.ChangeState(player.idleState);
    }

    public override void PhysicsUpdate()
    {
    }
}
