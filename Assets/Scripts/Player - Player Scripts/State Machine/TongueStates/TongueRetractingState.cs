using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueRetractingState : TongueState
{
    public TongueRetractingState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }
    public override void Intialize()
    {
        parentTransform = tongueStateMachine.parentTransform;
        lineRenderer = tongueStateMachine.lineRenderer;
    }
    private LineRenderer lineRenderer;
    private Transform endOfTongueTransform;
    private Transform parentTransform;

    private const float TONGUE_SHUTOFF_DISTANCE = 0.0001f;
    private const float TONGUE_RETRACT_SPEED = 4f;
    private const float TONGUE_PUESDO_DRAG = 0.4f;
    public override void EnterState()
    {
        //Debug.Log("Changed To Retracting State");
        endOfTongueTransform = tongueStateMachine.GetEndOfTongueTransform();
        tongueStateMachine.TurnOffEndOfTongueRB();
        lastMovement = Vector2.zero;
    }

    public override void ExitState()
    {
        //Debug.Log("leaving Retracting State");
        tongueStateMachine.DestroyEndOfTongue();
    }

    private Vector2 lastMovement;
    public override void FrameUpdate()
    {
        Vector2 tonguePlayerDifferenceVec = parentTransform.position - endOfTongueTransform.position;
        float dis = tonguePlayerDifferenceVec.magnitude;
        bool shouldShutOff = ShouldTheTongueShutOff_DistanceCheck(dis);

        if (shouldShutOff)
        {
            tongueStateMachine.ChangeState(player.tongueOffState);
            return;
        }
        Vector2 tongeMoveDirection = tonguePlayerDifferenceVec.normalized;
        this.lastMovement = UpdateTongue(tongeMoveDirection, dis, lastMovement);
        UpdateTongueRenderer(lineRenderer, parentTransform, endOfTongueTransform);
    }

    public override void PhysicsUpdate()
    {

    }

    public override bool isRetracting()
    {
        return true;
    }
    private bool ShouldTheTongueShutOff_DistanceCheck(float dis)
    {
        if(dis >= TONGUE_SHUTOFF_DISTANCE)
        {
            return false;
        }
        return true;
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
