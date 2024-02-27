using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLatchedState : PlayerState
{
    [SerializeField] private Vector2 _bufferedMovementInput;
    [SerializeField] private Vector2 _playerInput;

    public PlayerLatchedState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        _playerInput = GetCurrentMovementInputs();
        // First check if the buffered movement input isn't equal to zero;
        if (_bufferedMovementInput != Vector2.zero)
        {
            // Next check if the currented movement input is not zero so that you can override the buffer at anytime
            if(_playerInput != Vector2.zero)
            {
                
            }
        }
    }

    public override void ExitState()
    {
        _playerInput = Vector2.zero;
    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }

    public void RecieveBufferedMovementFromThrowingState(Vector2 bufferedMovement)
    {
        _bufferedMovementInput = bufferedMovement;
    }

    public Vector2 getPlayerInput()
    {
        return _playerInput;
    }
}
