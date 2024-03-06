using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {

    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        // SHOULD BE NO FRAME UPDATE, just exit and entry
    }

    public override void PhysicsUpdate()
    {
        // SHOULD BE NO PHYSICS UPDATE, just exit and entry
    }
}
