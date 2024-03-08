using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class PlayerLungingState : PlayerState
{
    /* Lunge variables */
    private LatchMovementType latchMovementType = LatchMovementType.Waiting;
    private Vector2 jhat;
    private Vector2 ihat;
    private Vector2 inputs;

    private float entryTime;

    #region Lunge Varaibles
    private float lateralForceModifer;
    private float minimumLateralDuration;
    private float lateralDragCoefficient;
    protected void getLungeVariables()
    {
        float[] vars = player.getLungeVaraiables();
        lateralForceModifer = vars[0];
        minimumLateralDuration = vars[1];
        lateralDragCoefficient = vars[2];
    }
    #endregion

    /*End of Tongue Variables*/
    private Transform endOfTongueTransform;

    /*Player Values*/
    private Rigidbody2D playerRB;
    private Collider2D playerCollider;
    
    public PlayerLungingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        playerRB = player.GetPlayerRigidBody();
        playerCollider = player.GetCollider();
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        playerCollider = player.GetComponent<Collider2D>();
    }

    public override void EnterState()
    {
        getLungeVariables(); // TODO: Remove so this is only called once, just used for updating vars
    
        ihat = player.tongueLatchedState.GetIhat();
        jhat = player.tongueLatchedState.GetJhat();

        entryTime = Time.time;

        endOfTongueTransform = player.tongueStateMachine.GetEndOfTongueTransform();
        
        
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                break;
            case LatchMovementType.LungeLeft:
                playerRB.drag = lateralDragCoefficient;
                playerRB.AddForce(-ihat*lateralForceModifer);
                break;
            case LatchMovementType.LungeRight:
                playerRB.drag = lateralDragCoefficient;
                playerRB.AddForce(ihat*lateralForceModifer);
                break;
            case LatchMovementType.LungeBack:
                break;
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR IN player lunging state on entry, should not be waiting");
                break;
        }
    }

    public override void ExitState()
    {
        SetLatchMovementType(LatchMovementType.Waiting);
    }

    public override void FrameUpdate()
    {
        // Reading inputs differently based on lunge type.
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                return; 
            case LatchMovementType.LungeLeft:
                {
                    Vector2 input = GetCurrentMovementInputs();
                    if (input != Vector2.zero)
                    {
                        return;
                    }
                    TryShutOffForLateralLunge();
                }
                return; 
            case LatchMovementType.LungeRight:
                {
                    Vector2 input = GetCurrentMovementInputs();
                    if (input != Vector2.zero)
                    {
                        return;
                    }
                    TryShutOffForLateralLunge();
                }
                return; 
            case LatchMovementType.LungeBack:
                return; 
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR in player lunging state, state should not be waiting");
                return; 
            default:
                Debug.LogError("ERROR in player lunging state, invalid state");
                return;
        }
    }

    public override void PhysicsUpdate()
    {
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                ForwardLunge();
                break;
            case LatchMovementType.LungeLeft:
                LateralLunge(latchMovementType);
                break;
            case LatchMovementType.LungeRight:
                LateralLunge(latchMovementType);
                break;
            case LatchMovementType.LungeBack:
                BackwardsLunge();
                break;
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR in player lunging state, state should not be waiting");
                return;
            default:
                Debug.LogError("ERROR in player lunging state, invalid state");
                return;
        }
    }
    public void SetLatchMovementType(LatchMovementType m)
    {
        latchMovementType = m;
    }

    private void LateralLunge(LatchMovementType direction)
    {

        switch (direction)
        {
            case LatchMovementType.LungeLeft:
                break;
            case LatchMovementType.LungeRight:
                break;
            default:
                Debug.LogError("Error in player lateral lunge state, should not be waiting or invalid state");
                return;
        }
    }
    private void ForwardLunge()
    {
        TryShutOffForForwardsLunge();
    }
    private void BackwardsLunge()
    {
        TryShutOffFOrBackwardsLunge();
    }
    private void TryShutOffForLateralLunge()
    {
        Debug.Log("trying to shut off lateral lunge");
        if (Time.time > minimumLateralDuration + entryTime)
        {
            Debug.Log("sucess!");
            playerStateMachine.ChangeState(player.slowingState);
            player.tongueStateMachine.ChangeState(player.tongueRetractingState);
        }
    }
    private void TryShutOffForForwardsLunge()
    {
        Debug.Log("Trying to shut off forward lunge");
        playerStateMachine.ChangeState(player.idleState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    private void TryShutOffFOrBackwardsLunge()
    {
        Debug.Log("Trying to shut off for backwards lunge");
        playerStateMachine.ChangeState(player.idleState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
}
