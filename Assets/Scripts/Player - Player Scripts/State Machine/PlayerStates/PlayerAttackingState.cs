using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingState : PlayerState
{
    private float attackingStateEntryTime;
    private float lastAttackTime;
    private bool isAttackStarted;
    private float chargeStartedTime;
    private float attackBufferMaxEmptyDuration = 1.0f;

    public PlayerAttackingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        ExecuteAttack();
        attackBufferEmptyTime = Time.time;
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        FindLeftMouseInputs();
        if (leftMouseDown) // once the mouse is pressed down for the first time, record the time
        {
            chargeStartedTime = Time.time;
            Debug.Log("attack charged started");
        }

        if(leftMouseUp) // once the mouse is released, add the attack to the buffer
        {
            AddToAttackBuffer();
        }
        if (Time.time > (lastAttackTime + 3.0f) )
        {
            ExecuteAttack();
        }
    }
    public void AddToAttackBuffer()
    {
        Vector2 attackDirection = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
        Vector2 movementDirection = GetCurrentMovementInputs();
        float attackChargeDuration = Time.time - chargeStartedTime;
        playerStateMachine.AddToAttackBuffer(new AttackInputData(attackDirection, movementDirection, attackChargeDuration));
    }
    public override void PhysicsUpdate()
    {

    }
    private bool attackBufferIsEmptySwitch = false;
    private float attackBufferEmptyTime;
    public void ExecuteAttack()
    {
        if (playerStateMachine.isAttackBufferEmpty())
        { // if the buffer is empty, don't execute attack. Save the first time it's empty, start a count down
            if (attackBufferIsEmptySwitch)
            {
                Debug.Log("attack buffer is empty, starting countdown");
                attackBufferIsEmptySwitch = false;
                attackBufferEmptyTime = Time.time;
            }
            TryClosingAttackingState();
            return;
        }
        else // if the buffer isn't empty, execute attack
        {
            attackBufferIsEmptySwitch = true;
            AttackInputData attackData = playerStateMachine.GetEarliestInAttackBuffer();
            float attackChargeDuration = attackData.getAttackChargeDuration();
            calculateAttackType(attackChargeDuration);
            lastAttackTime = Time.time;
            playerStateMachine.RemoveAttackFromBuffer();
            // use attack
        }
    }

    public void calculateAttackType(float attackChargeDuration)
    {
        if (attackChargeDuration > 3.0f)
        {
            Debug.Log("VERY LONG ATTACK");
        }
        else if (attackChargeDuration > 2.0f)
        {
            Debug.Log("LONG ATTACK");
        }
        else if (attackChargeDuration > 1.0f)
        {
            Debug.Log("MEDIUM ATTACK");
        }
        else if (attackChargeDuration > 0.5f)
        {
            Debug.Log("SHORT ATTACK");
        }
        else
        {
            Debug.Log("Click Attack");
        }
        //playerStateMachine.ChangeState(player.idleState);
    }

    /** Try closing attack buffer tries to close attacking state if the player 
     *  has not given any inputs past the buffer duration
     */
    public void TryClosingAttackingState()
    {
        if (playerStateMachine.isAttackBufferEmpty())
        {
            if( Time.time > (attackBufferEmptyTime + attackBufferMaxEmptyDuration) )
            {
                Debug.Log("Changing to idle state,\nCurrent time: " + Time.time + ", buffer empty time: " + attackBufferEmptyTime);
                playerStateMachine.ChangeState(player.idleState);
            }
        }
    }
}
