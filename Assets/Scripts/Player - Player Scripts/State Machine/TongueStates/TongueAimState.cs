using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueAimState : TongueState
{
    public TongueAimState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }

    private GameObject endOfTongue;

    public override void EnterState()
    {
        // Instatiate new end of tongue
        tongueStateMachine.endOfTongue = GameObject.Instantiate(tongueStateMachine.tongueEndPrefab, tongueStateMachine.parentTransform.position, Quaternion.identity);
        endOfTongue = tongueStateMachine.endOfTongue;

        // get the rigid body of the tongue end and shut it off
        Rigidbody2D endOfTongueRB = endOfTongue.GetComponent<Rigidbody2D>();
        tongueStateMachine.endOfTongueRB = endOfTongueRB;
        endOfTongueRB.simulated = false;
        
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }

    public void AimTongue(Vector2 location)
    {
        tongueStateMachine.aimLocation = location;
    }
}
