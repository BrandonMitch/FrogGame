using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBase<RefType> : IStateMachine<RefType>
    where RefType : class
{
    public RefType reference;
    public BaseState<RefType> CurrentState { get; private set; }

    /// <summary>
    /// Intialize State Machine by passing in a reference
    /// </summary>
    /// <param name="reference"></param>
    public void InitializeStateMachine(RefType reference)
    {
        this.reference = reference;
    }

    /// <summary>
    /// Puts the state machine in the first state
    /// </summary>
    /// <param name="startingState"></param>
    public virtual void StartStateMachine(BaseState<RefType> startingState)
    {
        //Debug.Log("[0] Start State Machine, setting state ");
        CurrentState = startingState;
        //Debug.Log("[2] currentstate.enterstate()");
        CurrentState.EnterState();
        //Debug.Log("[3] start state machine passed");
    }

    public void ChangeState(BaseState<RefType> newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }

    public IState getCurrentState()
    {
        return CurrentState;
    }

    RefType IStateMachine<RefType>.GetRef()
    {
        return reference;
    }


}

