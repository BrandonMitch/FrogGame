using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBase<RefType> : IStateMachine<RefType>
    where RefType : class
{
    public RefType reference;
    public BaseState<RefType> CurrentState { get; private set; }
    public BaseState<RefType> PreviousState { get; private set; }
    private List<BaseState<RefType>> states = new();
    private Dictionary<StateTag, List<BaseState<RefType>>> stateDictionary = new();

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
        CurrentState?.ExitState();
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState.EnterState();
    }

    public IState getCurrentState()
    {
        return CurrentState;
    }

    /// <summary>
    /// Add states to the dictionary so that we can create an easy way to search them
    /// </summary>
    /// <param name="state"></param>
    public void AddState(BaseState<RefType> state)
    {
        var stateTags = state.stateTags;
        foreach (StateTag stateTag in stateTags)
        {
            // If the dictionary doesn't contain a states with the tag provided, then create a new entry of the tag and add a list that contains that state with that tag.
            // If the dictionary alreay has a stateTag that of the one provided, then add to the list of that tag type.
            if (!stateDictionary.ContainsKey(stateTag))
            {
                stateDictionary.Add(stateTag, new List<BaseState<RefType>> { state });
            }
            else
            {
                stateDictionary[stateTag].Add(state);
            }
        }
    }
    public void RemoveState(BaseState<RefType> state)
    {
        var stateTags = state.stateTags;
        foreach (StateTag stateTag in stateTags)
        {
            // Check if the dictionary contains the stateTag
            if (stateDictionary.ContainsKey(stateTag))
            {
                // Retrieve the list associated with the stateTag
                var stateList = stateDictionary[stateTag];
                
                // Remove the state from the list
                stateList.Remove(state);

                // If the list becomes empty, remove the tag from the dictionary
                if (stateList.Count == 0)
                {
                    stateDictionary.Remove(stateTag);
                }
            }
        }
    }
    RefType IStateMachine<RefType>.GetRef()
    {
        return reference;
    }


    public void FrameUpdate()
    {
        CurrentState.FrameUpdate();
    }
    public void PhysicsUpdate()
    {
        CurrentState.PhysicsUpdate();
    }

    public void TakeRequest(Request request)
    {
        Debug.Log(this + " Recieved a request: " + request);
        if(request.simpleVectorRequest == Request.SimpleRequest.None)
        {
            /// If the simple request is == none then it is probably a get state. That means we msu
            var stateTag = request.stateTag;
            if(stateTag != null && request.getState != null)
            {
                if (stateDictionary.ContainsKey(stateTag))
                {
                    // TODO: Implement a way of providing an order
                    request.getState.Invoke(stateDictionary[stateTag][0]);
                }
                else
                {
                    Debug.LogWarning("State machine cannot find a reference to the state tag:" + stateTag.name);
                }
            }
            else
            {
                Debug.LogError("I think you called getState and it appears that .getState() wasn't provided, or state tag was null");
            }
        }
        else
        {
            HandleSimpleRequest(request);
        }
    }
    private void HandleSimpleRequest(Request request)
    {
        var requestType = request.simpleVectorRequest;
        switch (requestType)
        {
            case Request.SimpleRequest.MoveTo:
                {
                    if (stateDictionary.TryGetValue(GameManager.moveTag, out List<BaseState<RefType>> moveStates))
                    {
                        var x = moveStates[0];
                        var xmov = x as IMove;
                        if (xmov != null)
                        {
                            ChangeState(x);
                            if (request.onFuffill == null)
                            {
                                xmov.MoveTo(request.locationRequest);
                            }
                            else
                            {
                                xmov.MoveTo(request.locationRequest, onComplete: request.onFuffill);
                            }
                        }
                        else
                        {
                            Debug.LogError(x + " Is contains the move state tag and doesn't implement IMove");
                        }
                    }
                    break;
                }
            case Request.SimpleRequest.RunAwayFrom:
                {
                    if (stateDictionary.TryGetValue(GameManager.moveTag, out List<BaseState<RefType>> moveStates))
                    {
                        var x = moveStates[0];
                        var xmov = x as IMove;
                        if (xmov != null)
                        {
                            ChangeState(x);
                            xmov.MoveTo(request.locationRequest);
                        }
                        else
                        {
                            Debug.LogError(x + " Is contains the move state tag and doesn't implement IMove");
                        }
                    }
                    break;
                }
            case Request.SimpleRequest.Grapple:
                {
                    if (stateDictionary.TryGetValue(GameManager.grappleTag, out List<BaseState<RefType>> grappleStates))
                    {
                        var x = grappleStates[0] as BaseDataState<RefType, Enemy.GrappleData>;
                        if (x != null)
                        {
                            //x.RecieveData(request.dataRequest as Enemy.GrappleData);
                            ChangeState(x); // Make sure this state calls for the information
                        }
                        else
                        {
                            Debug.LogError(x + " Is contains the move grapple tag and doesn't cast to <RefType, object>");
                        }
                    }
                    else
                    {
                        Debug.LogError("No grapple tag found");
                    }
                    break;
                }
            case Request.SimpleRequest.StopCoroutines:
                {

                    break;
                }
            case Request.SimpleRequest.Idle:
                {

                    break;
                }
        }
    }
}

