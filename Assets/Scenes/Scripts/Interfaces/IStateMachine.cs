public interface IStateMachine<RefType>
    where RefType : class
{
    public void InitializeStateMachine(RefType reference);
    /// <summary>
    /// Get's the parent reference object
    /// </summary>
    /// <returns>the reference of the parent object</returns>
    public RefType GetRef();
    public IState getCurrentState();
    public void ChangeState(BaseState<RefType> newState);

    public void FrameUpdate();
    public void PhysicsUpdate();

    public void TakeRequest(Request request);
}
