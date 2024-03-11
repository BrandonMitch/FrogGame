using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueState 
{
    protected Player player;
    protected TongueStateMachine tongueStateMachine;
    //protected GameObject endOfTongue;
    public TongueState(Player player, TongueStateMachine tongueStateMachine)
    {
        this.player = player;
        this.tongueStateMachine = tongueStateMachine;
    }

    public virtual void EnterState()
    {

    }
    public virtual void ExitState()
    {

    }
    public virtual void FrameUpdate()
    {

    }
    public virtual void PhysicsUpdate()
    {

    }
    public virtual bool isRetracting()
    {
        return false;
    }
    public virtual void Intialize()
    {

    }

    static void UpdateBaseOfTongueRenderer(LineRenderer LR, Transform parentTransform)
    {
        LR.SetPosition(0, parentTransform.position);
    }
    static void UpdateEndOfTongueRenderer(LineRenderer LR, Transform endOfTongueTransform)
    {
        LR.SetPosition(1, endOfTongueTransform.position);
    }
    static protected void UpdateTongueRenderer(LineRenderer LR, Transform parentTransform, Transform endOfTongueTransform)
    {
        UpdateBaseOfTongueRenderer(LR, parentTransform);
        UpdateEndOfTongueRenderer(LR, endOfTongueTransform);
    }
    public void changeModeToCollision()
    {

    }
}
