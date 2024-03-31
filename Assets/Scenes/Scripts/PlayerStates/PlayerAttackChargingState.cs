using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackChargingState : PlayerState
{
    private float attackEntryTime;
    
    public PlayerAttackChargingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {

    }

    public override void EnterState()
    {
        Debug.Log("entered attack charging state");
        SetAttackEntryTime();
    }

    public override void ExitState()
    {
        float attackChargeDuration = Time.time - attackEntryTime;
        Vector2 attackDirection = (player.GetCrossHairPosition() - player.GetPosition()).normalized;
        Vector2 movementDirection = GetCurrentMovementInputs();
        AttackInputData newAttack = new AttackInputData(attackDirection,movementDirection,attackChargeDuration);
        playerStateMachine.AddToAttackBuffer(newAttack);
    }

    public override void FrameUpdate()
    {
       // FindLeftMouseInputs();
       // FindRightMouseInputs();
        if (leftMouseUp)
        {
            //playerStateMachine.ChangeState(playerAttackState);
            playerStateMachine.ChangeState(player.attackingState);
            return;
        }
        if (rightMouseButton)
        {
            playerStateMachine.ChangeState(player.aimingTongueState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public void SetAttackEntryTime()
    {
        string[] previousStateData = playerStateMachine.GetPreviousStateData();
        foreach (string s in previousStateData)
        {
            Debug.Log("contents:" + s);
        }
        int i = ParsePreviousStateDataFor(previousStateData, "startChargingTime");
        if (i != -1)
        {
            attackEntryTime = parseDataForFloat(previousStateData[i]);
        }
        else
        {
            Debug.Log("FAILED TO READ STARTCHARGING TIME");
            attackEntryTime = Time.time;
        }
    }
}
