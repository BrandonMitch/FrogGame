using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chain State Machine", menuName = "State Machines/States/Basic Chain State")]
public class BasicChainState : ChainBaseState<Enemy, object>
{
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void InitializeStateMachine(Enemy reference)
    {
        base.InitializeStateMachine(reference);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
