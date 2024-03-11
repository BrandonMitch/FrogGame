using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class TongueLungeState : TongueState
{
    private LineRenderer lineRenderer;
    private Transform parentTransform;
    private Transform endOfTongueTransform;
    private LatchMovementType latchMovementType;

    public TongueLungeState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }
    public override void Intialize()
    {
        lineRenderer = tongueStateMachine.lineRenderer;
        parentTransform = tongueStateMachine.parentTransform;
    }

    public override void EnterState()
    {
        GameObject endOfTongue = tongueStateMachine.endOfTongue;
        endOfTongueTransform = endOfTongue.transform;
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
        switch(tongueStateMachine.tongueSwingingMode){
            case TongueSwingingMode.TwoBody:
                UpdateTongueRenderer();
                break;
            case TongueSwingingMode.nBody:
                tongueStateMachine.MultiPointTongueRenderer();
                break;
        }
       

    }
    public void UpdateTongueRenderer()
    {
        UpdateTongueRenderer(lineRenderer, parentTransform, endOfTongueTransform);
    }
    public void SetLatchMovementType(LatchMovementType m)
    {
        latchMovementType = m;
    }

}
