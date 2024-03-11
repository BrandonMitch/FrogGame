using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;
public class PlayerLatchedState : PlayerState
{
    private Vector2 _bufferedMovementInput;
    private Vector2 _playerInput;
    private IPushable_Pullable push_pullable;
    private LatchLogicType latchLogicType;
    private Rigidbody2D push_pullRB;

    public PlayerLatchedState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        Debug.Log("entered latched state");
        _playerInput = GetCurrentMovementInputs();
        // First check if the buffered movement input isn't equal to zero;
        if (_bufferedMovementInput != Vector2.zero)
        {
            // Next check we check if there are no inputs at this time, if there are, then override them with the buffered input
            if (_playerInput == Vector2.zero)
            {
                Debug.Log("we are saving the buffered input on entry");
                _playerInput = _bufferedMovementInput;
            }
        }

        // Check if the object is pushable or pullable that we latched onto
        if (push_pullable != null)
        {

            bool pull = push_pullable.isPullableQ();
            bool push = push_pullable.isPushableQ();
            if (push || pull) push_pullRB = push_pullable.GetRigidBody();

            if (pull)
            {
                latchLogicType = LatchLogicType.pullLogic;
            }
            else if (push)
            {
                latchLogicType = LatchLogicType.pushLogic;
            }
            else { latchLogicType = LatchLogicType.baseLogic; }

        }
        else
        {
            latchLogicType = LatchLogicType.baseLogic;
        }
    }
    public override void ExitState()
    {
        Debug.Log("left latched state");
        _playerInput = Vector2.zero;
        push_pullable = null;
    }
    #region Frame Update Logic
    public override void FrameUpdate()
    {
        switch (latchLogicType)
        {
            case LatchLogicType.baseLogic:
                DefaultFrameUpdateLogc();
                break;
            case LatchLogicType.pushLogic:
                PushUpdateLogic();
                break;
            case LatchLogicType.pullLogic:
                PullUpdateLogc();
                break;
        }
    }
    private void DefaultFrameUpdateLogc()
    {
        if (CheckIfPlayerWantsToRetractTongue())
        {
            return;
        }
        _playerInput = GetCurrentMovementInputs();
        // First check if the buffered movement input isn't equal to zero;
        if (_bufferedMovementInput != Vector2.zero)
        {
            //Debug.Log("buffered movement input is not equal to the zero v");
            // Next check we check if there are no inputs at this time, if there are, then override them with the buffered input
            if (_playerInput == Vector2.zero)
            {
                //Debug.Log("we are saving the buffered input on frame update");
                _playerInput = _bufferedMovementInput;
            }
        }
    }
    private void PullUpdateLogc()
    {
        if (CheckIfPlayerWantsToRetractTongue())
        {
            push_pullable.OnRetract();
            return;
        }
        _playerInput = GetCurrentMovementInputs();
    }
    private void PushUpdateLogic()
    {
        if (CheckIfPlayerWantsToRetractTongue())
        {
            push_pullable.OnRetract();
            return;
        }
        _playerInput = GetCurrentMovementInputs();
    }
    #endregion
    #region Physics Update Logic
    public override void PhysicsUpdate()
    {
        switch (latchLogicType)
        {
            case LatchLogicType.baseLogic:
                DefaultPhyiscsUpdateLogic();
                break;
            case LatchLogicType.pushLogic:
                PushPhysicsUpdateLogic();
                break;
            case LatchLogicType.pullLogic:
                PullPhysicsUpdateLogic();
                break;
        }
    }

    public void DefaultPhyiscsUpdateLogic()
    {

    }
    public void PushPhysicsUpdateLogic()
    {
        if (_playerInput == Vector2.zero)
        {
            push_pullable.OnStopBeingPushed();
        }
        else
        {
            push_pullable.WhileBeingPushed();
        }
    }
    public void PullPhysicsUpdateLogic()
    {
        if (_playerInput == Vector2.zero)
        {
            push_pullable.OnStopBeingPulled();
            player.slowingState.setRestingDrag();
        }
        else
        {
            push_pullable.WhileBeingPulled();
            Transform endOfTongueTransform = player.tongueStateMachine.GetEndOfTongueTransform();
            Vector2 forceDirection = player.GetPosition() - endOfTongueTransform.position;
            forceDirection.Normalize();
            push_pullRB.AddForce(forceDirection * 5.0f);

            player.tongueLungeState.UpdateTongueRenderer();
            player.movingState.setRunningDrag();
            player.movingState.movementCode(_playerInput);
        }

    }
    #endregion
    public void RecieveBufferedMovementFromThrowingState(Vector2 bufferedMovement)
    {
        _bufferedMovementInput = bufferedMovement;
    }
    public bool CheckIfPlayerWantsToRetractTongue()
    {
        GetFKeyInputs();
        if (fKeyDown)
        {
            playerStateMachine.ChangeState(player.slowingState);
            player.tongueStateMachine.ChangeState(player.tongueRetractingState);
            return true;
        }
        return false;
    }
    public Vector2 getPlayerInput()
    {
        return _playerInput;
    }
    public void SetPushPullable(IPushable_Pullable pushable_Pullable)
    {
        this.push_pullable = pushable_Pullable;
    }
}

