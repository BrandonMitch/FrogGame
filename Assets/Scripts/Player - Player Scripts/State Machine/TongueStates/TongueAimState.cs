using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueAimState : TongueState
{
    public TongueAimState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }

    private GameObject endOfTongue;

    public override void EnterState()
    {
        // Instatiate new end of tongue
        endOfTongue = tongueStateMachine.IntializeEndOfTongue();
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

    public void AimTongue(Vector2 location)
    {
        tongueStateMachine.aimLocation = location;
    }
}
