namespace MovementNameSpace
{
    // 
    public enum LatchMovementType
    {
        Waiting,
        LungeForward,
        LungeLeft,
        LungeRight,
        LungeBack,
    };
    public enum TongueSwingingMode
    {
        TwoBody,
        nBody,
    }
    public enum TonguePointType
    {
        baseOfTongue,
        tongueHitPoint,
        endOfTongue,
    }
    public enum LatchLogicType
    {
        baseLogic,
        pushLogic,
        pullLogic,
    }

    public enum PositionStatus
    {
        OnTheGround,
        Flying,
        Lunging
    }
}