using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A state that contains a StateMachine within itself.
/// </summary>
/// <typeparam name="RefType">The parent class holding the stateMachine I.E Player, or Enemy</typeparam>
/// <typeparam name="DataType">The type of data to be sent inbetween other states</typeparam>
public abstract class ChainBaseState<RefType, DataType> : BaseState<RefType>, IStateMachine<RefType>
    where RefType : class
{
    public IState CurrentState { get; private set; }

    // Can chain states together that require data to be sent inbetween them
    protected ChainOffState<RefType, DataType> offState;
    protected List<BaseState<RefType>> subStates;

    [Header("--- Reference Off State --- ")]
    [SerializeField] protected ChainOffState<RefType, DataType> referenceOFFSTATE; // TODO: This will create a null reference exception. Super fucking annoying
    [Tooltip("--- Substates --- * Do not include offState in the substates")]
    [SerializeField] protected List<BaseState<RefType>> referenceSubStates;


    #region Basic State Code

    public override void EnterState()
    {
        Debug.Log("[c0] Entering Chainbase state");
        NextState();
    }
    public override void FrameUpdate()
    {
        CurrentState.FrameUpdate();
    }
    public override void PhysicsUpdate()
    {
        CurrentState.PhysicsUpdate();
    }
    public override void ExitState()
    {
        ChangeState(offState);
    }
    public override void Initialize(IStateMachine<RefType> stateMachine)
    {
        // Intialize the state, (Remeber a chain state is like a sub state machine, it contains a reference to the base state machine)
        base.Initialize(stateMachine);

        /// After we intialize the chain state like normal, we have to treat the chain state as if it where a state machine, so that means calling intializeStateMachine
        InitializeStateMachine(null);
    }
    #endregion

    #region Sub-State Machine Code
    /// <summary>
    /// Will intialize the chain state machine by putting it into the off state.
    /// </summary>
    /// <param name="reference">Parent reference object, usually a mono behaviour like Player or Enemey </param>
    public virtual void InitializeStateMachine(RefType reference)
    {
        // reference in the method parameters is unused because we get this info from the base state machine, chain state is only acting as a state with a state machine it in 
        if(SM == null) { Debug.LogError("Call Intialize(Base state machine) before calling IntializeStateMachine(reference)"); return; }
        if (referenceOFFSTATE == null) { Debug.LogError("Chain state is missing a reference to an off state"); return; }
        this.reference = SM.GetRef();

        // Now we complete the cloning process of every substate 
        int n = referenceSubStates.Count;
        subStates = new(); // create an empty list

        //  First construct a copy of the off state and place it in the first slot of substates
        //Debug.Log("for[0]:" + referenceOFFSTATE);
        offState = Instantiate<ChainOffState<RefType, DataType>>(referenceOFFSTATE);
        offState.Initialize(this);
        subStates.Add(offState);

        // Now clone all of the substates
        for (int i = 0; i < n; i++)
        {
            var refState = referenceSubStates[i];
            //Debug.Log("for[" + (i + 1) + "]:" + refState);
            if (refState != null)
            {
                subStates.Add(Instantiate<BaseState<RefType>>(refState));
                subStates[i + 1].Initialize(this);
            }
        }

        // Normal State machine Code, start in the off state. Don't run until change state is called
        CurrentState = offState;
        offState.EnterState();
    }

    public void ChangeState(BaseState<RefType> newState)
    {
        //Debug.Log("Chain State CHANGE TO:" + newState);

        if (newState == null) { Debug.LogError("State is null, something went wrong!"); return; }
        if (!subStates.Contains(newState)) { Debug.LogError("Chain state doesn't contain:" + newState +", Or cast failed"); return; }

        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }
    public override void ResetValues()
    {
        base.ResetValues();
        subStates = null;
    }

    /// <summary>
    /// Move to the next state in a Chain State Machine.
    /// </summary>
    /// <param name="loop"> if loop is set true then it will reset to the start of the substate list if there is not a next substate</param>
    public void NextState(bool loop = false)
    {
        int n = subStates.IndexOf(CurrentState as BaseState<RefType>);
        if (n + 1 >= subStates.Count)
        {
            ChangeState(offState);
            if (loop)
            {
                offState.Loop();
            }
        }
        else
        {
            ChangeState(subStates[n + 1]);
        }
    }
    public void ExitSubStateMachine()
    {
        ChangeState(offState);
    }

    #region Getters
    public IState getCurrentState()
    {
        return CurrentState;
    }
    public RefType GetRef()
    {
        return reference;
    }
    public override string ToString()
    {
        string s = base.ToString() + "\n";
        int i = 0;
        foreach (var substate in subStates)
        {
            if (substate != null)
            {
                string current = (substate.Equals(CurrentState)) ? " >>>" : "    ";
                s += string.Format("{0}[{1}]:{2}\n", current, i, substate.ToString());
            }
            else
            {
                s += "    [" + i + "]: NULL\n";
            }
            i++;
        }
        return s;
    }
    #endregion

    #endregion
}

#region Off Chain State
/// <summary>
/// Empty state used for chaining
/// </summary>
/// <typeparam name="RefType"> Unused</typeparam>
public class ChainOffState<RefType, DataType> : BaseState<RefType>
    where RefType : class
{
    new ChainBaseState<RefType, DataType> SM;
    public override void EnterState()
    {
        Debug.Log("Entered off state");
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }
    public void Loop()
    {
        // Loop false is called so that we don't have inifinite recursion if there are no states present besisdes the off state
        SM.NextState(loop: false);
    }
}
#endregion