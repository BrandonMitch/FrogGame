using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state is used after the tongue is thrown and we are waiting for the tongue to hit an object
///  <para>-Once the tongue is thrown we read buffer inputs after 0.5 seconds </para>
/// </summary>
public class PlayerThrowingState : PlayerState
{
    
    [SerializeField] private float _timeWhenEnteringThrowingState;
    [SerializeField] private float _bufferTimeToStartReading = 0.1f; // how long it takes after the tongue was thrown to read buffer inputs
    [SerializeField] public Vector2 bufferedMovementInput;
    public PlayerThrowingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        Debug.Log("entered throwing state");
        player.tongueStateMachine.ChangeState(player.tongueThrowState);
        // Save the current time when we enter the state
        _timeWhenEnteringThrowingState = Time.time;
        // Reset the buffered input to zero vector
        bufferedMovementInput = Vector2.zero;
    }

    public override void ExitState()
    {
        // throw the buffered input to the next state if it's not equal to zero,
        // after we change states to latched,
        // latched state will throw it to  tongue manager if the player is not giving inputs
        Debug.Log("Buffered Inputs: (" + bufferedMovementInput.x + "," + bufferedMovementInput.y + ")");
        SendBufferedMovementToLatchedState();
    }

    public override void FrameUpdate()
    {
        // only thing we want avalible is to be able to cancel out of the throw with a sword attack
        
        // read inputs and buffer them
        if(Time.time > (_timeWhenEnteringThrowingState + _bufferTimeToStartReading))
        {
            //Debug.Log("reading buffered inputs");
            Vector2 currentMovmentInputs = GetCurrentMovementInputs();
            if (currentMovmentInputs != Vector2.zero)
            {
                bufferedMovementInput = GetCurrentMovementInputs();
                Debug.Log("we should be saving this input:" + bufferedMovementInput);
            }
        }
        else
        {
            //Debug.Log("not reading buffered inputs");
        }

    }

    public override void PhysicsUpdate()
    {

    }

    public void SendBufferedMovementToLatchedState()
    {
        player.latchedState.RecieveBufferedMovementFromThrowingState(bufferedMovementInput);
    }

}
