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

}
