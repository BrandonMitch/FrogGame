using UnityEngine;

public class PlayerIdleState : PlayerState
{
    #region Movement Variables
    protected float playerSpeed;
    protected float playerMaxSpeed;
    protected float playerRestingDrag;
    protected float playerRunningDrag;
    protected float playerDragSlowDownTime;
    protected float playerRunForceModifier;

    protected void getMovementVaraibles()
    {
        float[] vars = player.getMovementVaraibles();
        playerSpeed = vars[0];
        playerMaxSpeed = vars[1];
        playerRestingDrag = vars[2];
        playerRunningDrag = vars[3];
        playerDragSlowDownTime = vars[4];
        playerRunForceModifier = vars[5];
    }
    #endregion
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType); // UHHHH IMPLEMENT
    }

    public override void EnterState()
    {
        SetMovementInputs(Vector2.zero, 0.0f);
        ClearStateData();
    }

    public override void ExitState()
    {

    }
    private float startChargingTime = 0;
    public override void FrameUpdate()
    {
        Vector2 moveVec = GetCurrentMovementInputs();
        // Check tongue first
        //FindRightMouseInputs();
        //FindLeftMouseInputs();
        //Debug.Log("Right mouse inputs: up " + rightMouseUp + " down " + rightMouseDown);
        
        
        if (rightMouseButton)
        {
            if (TryChangingToTongueAimingState()) return;
        }
        // Save the time when the player starts charging
        if (leftMouseDown) {
            SaveStateData(0,"startChargingTime = " + Time.time); ;
        }

        if (leftMouseButton) // while the button is being held down, we want to save the time
        {
            if (TryChangingToAttackChargingState()) return;
        }

        // Then check movement
        if(moveVec != Vector2.zero)
        {
            player.movingState.setMoveVecToFirstInput(moveVec);
            player.stateMachine.ChangeState(player.movingState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {

    }

    private bool TryChangingToTongueAimingState()
    {
        bool tongueIsNotRetracting = !player.tongueStateMachine.isTongueRetracting();
        if (tongueIsNotRetracting)
        {
            playerStateMachine.ChangeState(player.aimingTongueState);
            return true;
        }
        return false;
    }

    private bool TryChangingToAttackChargingState()
    {
        bool tongueIsNotRetracting = !player.tongueStateMachine.isTongueRetracting();
        if (tongueIsNotRetracting)
        {
            playerStateMachine.ChangeState(player.attackChargingState);
            return true;
        }
        return false;
    }

    private string[] stateData = new string[1];
    public void SaveStateData(int index,string s)
    {
        stateData[index] = s; 
    }
    public void ClearStateData()
    {
        for (int i = 0; i < stateData.Length; i++)
        {
            stateData[i] = "";
        }
    }
    public override string[] PreviousStateData()
    {
        return stateData;
    }
}
