using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueThrowState : TongueState
{
    private LineRenderer lineRenderer;
    private Transform parentTransform;

    private Transform endOfTongueTransform;
    private Rigidbody2D endOfTongueRB;
    private bool needToCalculateVel = true;
    private Vector2 TongueVelocity;
    private bool willNotHitNextFrame = true;
    private Vector3 latchLocation;
    private RaycastHit2D collisionHit;
    private IPushable_Pullable pushPullInterface;
    public TongueThrowState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }
    public override void Intialize()
    {
        lineRenderer = tongueStateMachine.lineRenderer;
        parentTransform = tongueStateMachine.parentTransform;
    }

    public override void EnterState()
    {
        pushPullInterface = null;
        // Declare variables (just for readability so  I don't have to write tongueStatemachine.linerenderer
        GameObject endOfTongue = tongueStateMachine.endOfTongue;
        endOfTongueTransform = endOfTongue.transform;
        endOfTongueRB = tongueStateMachine.endOfTongueRB;

        endOfTongueTransform.position = parentTransform.position;
        endOfTongueRB.simulated = true; // Enable physics for tongue
        TongueVelocity = Vector2.zero;
        moveTongueTowards(tongueStateMachine.aimLocation);

        // turn on rendering for tongue
        lineRenderer.enabled = true; 
        // Set start of tongue a the parent transform position
        lineRenderer.SetPosition(0, parentTransform.position);
        // Set end of tongue at the end of the tongue game object that we instiated 
        lineRenderer.SetPosition(1, endOfTongueTransform.position);
    }

    public override void ExitState()
    {
        needToCalculateVel = true;
        willNotHitNextFrame = true;
    }
    public override void FrameUpdate()
    {
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

        lineRenderer.SetPosition(1, endOfTongueTransform.position); // maybe should be in frame update
    }

    private void moveTongueTowards(Vector3 location)
    {
        Vector2 direction = new Vector2(location.x, location.y);
        direction = (direction - (Vector2)parentTransform.position);
        direction.Normalize();
        endOfTongueRB.AddForce(20 * direction);
        Debug.Log("velocity found: " + endOfTongueRB.velocity);
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
        bool returnVal = true;
        pushPullInterface = hit.rigidbody.GetComponent<IPushable_Pullable>();
        if(pushPullInterface != null)
        {
            Debug.Log("THIS IS PUSHABLE/PULLABLE");
            Debug.Log("Is this pulllable?" + pushPullInterface.isPullableQ());

        }
        if (returnVal)
        {
            // We will hit next frame;
            willNotHitNextFrame = false;
            this.latchLocation = latchLocation;
            this.collisionHit = hit;
        }
        // TODO: Implement false return values, this will let us decided what we don't want to latch onto
        return returnVal;
    }

    private void OnHit(Vector3 latchlocation, RaycastHit2D hit)
    {
        endOfTongueRB.simulated = false;
        moveEndOfTongue(latchLocation);
        if (pushPullInterface != null)
        {
            pushPullInterface.OnLatchedTo();
            SendInfoToLatchState();

        }
        tongueStateMachine.ChangeState(player.tongueLatchedState);
    }

    private void SendInfoToLatchState()
    {
        player.tongueLatchedState.SetPushPullable(pushPullInterface);
        player.latchedState.SetPushPullable(pushPullInterface);
    }
    private void moveEndOfTongue(Vector3 location)
    {
        endOfTongueTransform.position = location;
    }
}
