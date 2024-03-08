using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected bool isTransitionState = false;
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    
    protected BaseState<EState> CurrentState;

    void Start()
    {
        
        CurrentState.EnterState();
    }
    void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();
        if (!isTransitionState && nextStateKey.Equals (CurrentState.stateKey))
        {
            CurrentState.UpdateState();
        }
        else if (!isTransitionState)
            TransitionToState(nextStateKey);
        CurrentState.UpdateState();
    }

    public void TransitionToState(EState stateKey)
    {
        isTransitionState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        isTransitionState = true;
    }
    
     void OnTriggerEnter2D(Collider other)
    {
        CurrentState.OnTriggerEnter2D(other);
    }
    void OnTriggerStay2D(Collider other)
    {
        CurrentState.OnTriggerStay2D(other);
    }
    void OnTriggerExit2D(Collider other)
    { 
        CurrentState.OnTriggerExit2D(other);
    }
    
}

