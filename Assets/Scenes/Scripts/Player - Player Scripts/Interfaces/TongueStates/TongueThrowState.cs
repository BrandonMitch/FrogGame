using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueThrowState : TongueState
{
    private Transform parentTransform = null;

    private Transform endOfTongueTransform;
    private Rigidbody2D endOfTongueRB;
    private bool needToCalculateVel = true;
    private Vector2 TongueVelocity;
    private bool willNotHitNextFrame = true;
    private Vector3 latchLocation;
    private RaycastHit2D collisionHit;
    private IPushable_Pullable pushPullInterface;
    private IModifyTongueBehavior collisionObjectBehaviour;
    private GenericStat tongueThrowForceModifier;
    public TongueThrowState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
        player.StartCoroutine(GetParentTransform());
        tongueThrowForceModifier = player.TongueThrowForceModifier;
    }
    private IEnumerator GetParentTransform()
    {
        bool keepTrying = true;
        while (keepTrying)
        {
            // try to get parent transform
            parentTransform = tongueStateMachine.GetParentTransform();
            if (parentTransform == null) // if it failed, wait a frame trying again
            {
                yield return null;
            }
            else // if it worked, exit loop
            {
                break;
            }
        }
    }

    public override void EnterState()
    {
        if(parentTransform == null)
        {
            Debug.LogError("This really should never happen. The parentTransform is null while trying to enter tongue throw state");
        }
        pushPullInterface = null;
        collisionObjectBehaviour = null;

        endOfTongueTransform = tongueStateMachine.GetEndOfTongueTransform();
        endOfTongueRB = tongueStateMachine.endOfTongueRB;

        endOfTongueTransform.position = parentTransform.position;
        endOfTongueRB.simulated = true; // Enable physics for tongue
        TongueVelocity = Vector2.zero;
        MoveTongueTowards(tongueStateMachine.aimLocation);

        // turn on rendering for tongue, call it for one frame
        tongueStateMachine.StartTongueRenderer();
        tongueStateMachine.TwoPointTongueRenderer();
    }

    public override void ExitState()
    {
        needToCalculateVel = true;
        willNotHitNextFrame = true;
    }
    public override void FrameUpdate()
    {
        tongueStateMachine.TwoPointTongueRenderer();
    }

    public override void PhysicsUpdate()
    {
        if (willNotHitNextFrame) // if we wont hit next frame run distnace test
        {
            DistanceTestTongue();
        }
        else if (!willNotHitNextFrame) // if we will hit next frame
        {
            OnHit(latchLocation, collisionHit);
        }
    }
    private void MoveTongueTowards(Vector3 location)
    {
        Vector2 direction = new Vector2(location.x, location.y);
        direction = (direction - (Vector2)parentTransform.position);
        direction.Normalize();
        endOfTongueRB.AddForce(tongueThrowForceModifier.Value * direction);
        //Debug.Log("velocity found: " + endOfTongueRB.velocity);
    }

    private void DistanceTestTongue()
    {
        if (needToCalculateVel == true) // since velocity is constant for the tongue, check the velocity only once for preformance issues
        {
            TongueVelocity = endOfTongueRB.velocity;
            if(TongueVelocity != Vector2.zero)
            {
                needToCalculateVel = false;
            }
        }
        if (needToCalculateVel == false)
        {   // TODO: I dont think raycast hit2d is working in this
            Vector2 currentPos = endOfTongueRB.position;
            Vector2 nextPos = currentPos + TongueVelocity * Time.fixedDeltaTime;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(currentPos, 0.02f, TongueVelocity, (nextPos - currentPos).magnitude);
            Debug.DrawLine(currentPos, nextPos);
            bool isLatched = false;
            foreach (RaycastHit2D hit in hits)
            {
                if (!isLatched)
                {
                    if (hit.collider.tag != "Player")
                    {
                        Vector2 latchLocation = hit.point;
                        Debug.Log("Try to latch is called");
                        isLatched = TryToLatch(latchLocation, hit);
                    }
                }
            }
        }
    }

    private bool TryToLatch(Vector3 latchLocation, RaycastHit2D hit)
    {
        bool ableToLatch = true;
        collisionObjectBehaviour = hit.collider.GetComponent<IModifyTongueBehavior>();
        if (collisionObjectBehaviour != null)
        {
            pushPullInterface = collisionObjectBehaviour as IPushable_Pullable;
        }

        if(collisionObjectBehaviour != null)
        {
            ableToLatch = collisionObjectBehaviour.isLatchable(player);
        }
        if (ableToLatch)
        {
            willNotHitNextFrame = false; // will hit next frame
            this.latchLocation = latchLocation;
            this.collisionHit = hit;
        }
        else // if unable to latch, should we retract?
        {
            if (collisionObjectBehaviour.iCauseRetractOnUnLatchable(player))
            {
                player.stateMachine.CurrentPlayerState.RetractTongue();
            }
        }
        return ableToLatch;
    }

    private void OnHit(Vector3 latchlocation, RaycastHit2D hit)
    {
        endOfTongueRB.simulated = false;

        moveEndOfTongue(latchLocation);

        // If we hit a moving object, send the information to the end of the tongue so it can "stick to the object" every frame
        if (collisionObjectBehaviour != null)
        {
            ICanMoveTheTongue movingObject = collisionObjectBehaviour as ICanMoveTheTongue;
            if (movingObject != null)
            {
                tongueStateMachine.endOfTongueScript.SetInfo(
                    movingObject: movingObject
                    );
            }
        }

        if (pushPullInterface != null)
        {
            pushPullInterface.OnLatchedTo(latchlocation);
            SendInfoToLatchState();

        }
        tongueStateMachine.ChangeState(player.tongueLatchedState);
    }

    private void SendInfoToLatchState()
    {
        player.tongueLatchedState.SetPushPullable(pushPullInterface);
        player.tongueLatchedState.SetTongueVelocityDirection(TongueVelocity);
        player.latchedState.SetPushPullable(pushPullInterface);
    }
    private void moveEndOfTongue(Vector3 location)
    {
        endOfTongueTransform.position = location;
    }
}
