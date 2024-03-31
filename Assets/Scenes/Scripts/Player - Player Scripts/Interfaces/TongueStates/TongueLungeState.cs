using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class TongueLungeState : TongueState
{
    private Transform endOfTongueTransform;
    private LatchMovementType latchMovementType;

    public TongueLungeState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {

    }

    public override void EnterState()
    {
        endOfTongueTransform = tongueStateMachine.GetEndOfTongueTransform();
    }

    public override void ExitState()
    {
        //Debug.Log("Left lunging tongue state");
    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        tongueStateMachine.TwoPointTongueRenderer();
    }


    public void SetLatchMovementType(LatchMovementType m)
    {
        latchMovementType = m;
    }
    public void HelperMethodForPushingPulling()
    {
        GameObject endOfTongue = tongueStateMachine.endOfTongue;
        endOfTongueTransform = endOfTongue.transform;
    }
    public void UpdateEndOfTongueForPushingPulling(Vector3 distancefromHit, IPushable_Pullable pushPullObject)
    {
        Vector3 objectLocation = pushPullObject.GetPosition();
        endOfTongueTransform.position = objectLocation + distancefromHit;
    }

}
