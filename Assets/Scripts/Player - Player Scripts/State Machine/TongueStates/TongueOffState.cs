using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueOffState : TongueState
{
    public TongueOffState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {

    }

    public override void EnterState()
    {
        tongueStateMachine.TurnOffLineRenderer();
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
