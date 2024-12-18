using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;
public class TongueState : IState
{
    protected Player player;
    protected TongueStateMachine tongueStateMachine;
    public TongueState(Player player, TongueStateMachine tongueStateMachine)
    {
        this.player = player;
        this.tongueStateMachine = tongueStateMachine;
    }
    public Player GetPlayer()
    {
        return player;
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
    public virtual bool isOff()
    {
        return false;
    }
    public virtual bool isAiming()
    {
        return false;
    }
    public virtual void Intialize()
    {

    }
    public virtual void ResetValues()
    {

    }
}
