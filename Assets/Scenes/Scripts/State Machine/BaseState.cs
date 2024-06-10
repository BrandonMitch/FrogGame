using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A base for all other state to derive from. Base states do not hold data avaliable for other states to retreive.
/// </summary>
/// <typeparam name="RefType">The parent class holding the stateMachine I.E Player, or Enemy</typeparam>
abstract public class BaseState<RefType> : ScriptableObject, IState 
    where RefType : class
{
    protected IStateMachine<RefType> SM;
    protected RefType reference;
    [SerializeField] public List<StateTag> stateTags;

    /// <summary>
    /// Intializes a state with a reference to a statemachine
    /// </summary>
    /// <param name="stateMachine"></param>
    public virtual void Initialize(IStateMachine<RefType> stateMachine)
    {
        SM = stateMachine;
        reference = SM.GetRef();
    }
    public virtual void EnterState()
    {

    }
    public virtual void FrameUpdate()
    {

    }
    public virtual void PhysicsUpdate()
    {

    }
    public virtual void ExitState()
    {

    }

    /// <summary>
    /// This is called before anything else to reset the initalization to make sure everything goes smoothly
    /// </summary>
    public virtual void ResetValues()
    {
        reference = null;
        SM = null;
    }
}
