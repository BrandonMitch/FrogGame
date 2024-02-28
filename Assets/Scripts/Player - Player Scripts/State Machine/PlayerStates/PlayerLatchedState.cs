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
    }

    public override void ExitState()
    {
        Debug.Log("left latched state");
        _playerInput = Vector2.zero;
    }

    public override void FrameUpdate()
    {
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
