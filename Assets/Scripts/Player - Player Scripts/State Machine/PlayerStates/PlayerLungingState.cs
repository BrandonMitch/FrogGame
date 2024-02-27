using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLungingState : PlayerState
{
    public PlayerLungingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
    }
}
