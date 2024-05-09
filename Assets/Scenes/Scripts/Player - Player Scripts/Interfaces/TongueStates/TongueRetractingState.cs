using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class TongueRetractingState : TongueState
{
    public TongueRetractingState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }
    private Transform endOfTongueTransform;

    private const float TONGUE_SHUTOFF_DISTANCE = 0.0001f;
    private const float TONGUE_RETRACT_SPEED = 4f;
    private const float TONGUE_PUESDO_DRAG = 0.4f;
    public override void EnterState()
    {
        if(tongueStateMachine.TonguePointCount() == 1)
        {
            tongueStateMachine.ChangeState(player.tongueOffState);
            return;
        }
        //Debug.Log("Changed To Retracting State");
        endOfTongueTransform = tongueStateMachine.GetEndOfTongueTransform();
        tongueStateMachine.TurnOffEndOfTongueRB();
        lastMovement = Vector2.zero;


    }

    public override void ExitState()
    {
        player.AnimateRetract_Reset();
        //Debug.Log("leaving Retracting State");
        tongueStateMachine.DestroyEndOfTongue();
    }

    private Vector2 lastMovement;
    private TongueHitData nextPoint;
    public override void FrameUpdate()
    {
        nextPoint = tongueStateMachine.GetPointBeforeEndOfTongue();

        Vector2 NextNode = nextPoint.getPos();

        Vector2 tonguePlayerDifferenceVec = NextNode - (Vector2)endOfTongueTransform.position;
        float dis = tonguePlayerDifferenceVec.magnitude;
        bool shouldShutOff = ShouldTheTongueShutOff_DistanceCheck(dis);

        if (shouldShutOff)
        {
            tongueStateMachine.ChangeState(player.tongueOffState);
            return;
        }
        Vector2 tongeMoveDirection = tonguePlayerDifferenceVec.normalized;
        this.lastMovement = UpdateTongue(tongeMoveDirection, dis, lastMovement);
/*        switch (tongueStateMachine.tongueSwingingMode)
        {
            case TongueSwingingMode.TwoBody:
                UpdateTongueRenderer(lineRenderer, parentTransform, endOfTongueTransform);
                break;
            case TongueSwingingMode.nBody:
                tongueStateMachine.MultiPointTongueRenderer();
                break;
        }*/
    }

    public override void PhysicsUpdate()
    {
        tongueStateMachine.TwoPointTongueRenderer();
    }

    public override bool isRetracting()
    {
        return true;
    }
    private bool ShouldTheTongueShutOff_DistanceCheck(float dis)
    {
        if (nextPoint.type == TonguePointType.baseOfTongue)
        {
            if (dis >= TONGUE_SHUTOFF_DISTANCE)
            {
                return false;
            }
            return true;
        }
        if (nextPoint.type == TonguePointType.tongueHitPoint)
        {
            if (dis <= TONGUE_SHUTOFF_DISTANCE)
            {
                tongueStateMachine.DestroyTongueMidPoint(0);
                return false;
            }
        }
        return false;
    }
    private Vector2 UpdateTongue(Vector2 movVec, float maxDistance, Vector2 lastMovement)
    {
        movVec = (movVec) * TONGUE_RETRACT_SPEED * Time.deltaTime;
        movVec += (TONGUE_PUESDO_DRAG * lastMovement);
        movVec = Vector2.ClampMagnitude(movVec, maxDistance);
        //Debug.Log("movVec = " + movVec);
        Debug.DrawLine(endOfTongueTransform.position,endOfTongueTransform.position + (Vector3)movVec, Color.yellow);
        endOfTongueTransform.position += (Vector3)movVec;
        return movVec;
    }
}
