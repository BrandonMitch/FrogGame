using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueAimState : TongueState
{
    public TongueAimState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }

    private GameObject endOfTongue;
    bool needToKeepTryingToIntializeEndOfTongue;
    public override void EnterState()
    {
        needToKeepTryingToIntializeEndOfTongue = false;
        // Instatiate new end of tongue
        endOfTongue = tongueStateMachine.IntializeEndOfTongue();
        if(endOfTongue == null)
        {
            needToKeepTryingToIntializeEndOfTongue = true;
        }
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        if (needToKeepTryingToIntializeEndOfTongue)
        {
            endOfTongue = tongueStateMachine.IntializeEndOfTongue();
            if (endOfTongue != null)
            {
                needToKeepTryingToIntializeEndOfTongue = false;
            }
        }
    }

    public void AimTongue(Vector2 location)
    {
        tongueStateMachine.aimLocation = location;
    }
}
