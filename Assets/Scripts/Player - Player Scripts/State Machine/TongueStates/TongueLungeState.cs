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
        tongueStateMachine.ChangeState(player.tongueOffState);
        player.stateMachine.ChangeState(player.idleState);
    }

    public override void ExitState()
    {
        GameObject.Destroy(tongueStateMachine.endOfTongue);
        Debug.Log("destroyed end of tongue");
    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }


}
