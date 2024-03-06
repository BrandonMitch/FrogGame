using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine playerStateMachine;
    protected bool leftMouseDown = false;
    protected bool leftMouseUp = false;
    protected bool rightMouseDown = false;
    protected bool rightMouseUp = false;

    public PlayerState(Player player, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
    }

    public virtual void EnterState()
    {

    }
    public virtual void ExitState()
    {

    }
    public virtual void FrameUpdate()
    {
        // Check movements;
        // set direction to x,y
        // set speedforanim to anim
        // player.Animate();
    }
    public virtual void PhysicsUpdate()
    {

    }

    public virtual void AnimationTriggerEvent(Player.AnimationTriggerType triggerType) 
    {

    }

    // IMPLEMENT
    public virtual float CalculateSpeed()
    {
        return 1.0f;
    }

    protected Vector2 GetCurrentMovementInputs()
    {
        // Check x/y movement, normalize vector
        Vector2 moveVec = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),1.0f);
        return moveVec;
    }
    protected void SetMovementInputs(Vector2 moveVec)
    { 
        player.SetMovementInputs(moveVec,moveVec.magnitude);
    }
    protected void SetMovementInputs(Vector2 moveVec, float speed)
    {
        player.SetMovementInputs(moveVec, speed);
    }

    protected void SetLastMovementDirection(Vector2 LastMovementDirection)
    {
        player.SetLastMoveDirection(LastMovementDirection);
    }
    protected void FindLeftMouseInputs()
    {
        // Attack is started
        if (Input.GetButtonDown("Fire1")) {
            leftMouseDown = true;
        } else {
            leftMouseDown = false;
        }

        // Attack is released
        if (Input.GetButtonUp("Fire1")) {
            leftMouseUp = true;
        } else {
            leftMouseUp = false;
        }
    }
    protected void FindRightMouseInputs()
    {
        // Aiming the tongue
        if (Input.GetButton("Fire2")) {
            rightMouseDown = true;
        } else {
            rightMouseDown = false;
        }
        // Spitting out the tongue on release
        if (Input.GetButtonUp("Fire2")) {
            rightMouseUp = true;
        } else {
            rightMouseUp = false;
        }
    }
}
