using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueLungeState : TongueState
{
    public TongueLungeState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }

    public override void EnterState()
    {
        tongueStateMachine.ChangeState(player.tongueRetractingState);
        player.stateMachine.ChangeState(player.idleState);
    }

    public override void ExitState()
    {
        Debug.Log("Left lunging state");
    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }


}
