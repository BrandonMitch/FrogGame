using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;
public class PlayerStateMachine
{

    public PositionStatus positionStatus;
    public PlayerState CurrentPlayerState {  get; private set; }
    private PlayerState PreviousPlayerState = null;

    public PlayerInputManager playerInputManager;
    private List<AttackInputData> attackBuffer;

    public void Intialize(PlayerState startingState)
    {
        attackBuffer = new List<AttackInputData>();
        positionStatus = PositionStatus.OnTheGround;

        CurrentPlayerState = startingState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        PreviousPlayerState = CurrentPlayerState;
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState(); 
    }
    public AttackInputData GetEarliestInAttackBuffer()
    {
        return attackBuffer[0];

    }
    public void AddToAttackBuffer(AttackInputData attack)
    {
        attackBuffer.Add(attack);
    }

    public bool isAttackBufferEmpty()
    {
        if (attackBuffer.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
    public void RemoveAttackFromBuffer()
    {
        if (attackBuffer.Count > 0)
        {
            attackBuffer.RemoveAt(0);
        }
        else
        {
            Debug.LogError("FAILED TO REMOVE ATTACK FROM BUFFER");
        }
    }

    public string[] GetPreviousStateData()
    {
        return PreviousPlayerState.PreviousStateData();
    }
    public void EnterOffStateImmediately(Player player)
    {
        player.stateMachine.ChangeState(player.idleState);
    }
}
