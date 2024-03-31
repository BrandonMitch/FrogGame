using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class TongueLatchedState : TongueState
{
    private LatchMovementType movementType;
    private Transform endOfTongueTransform;
    //private Transform parentTransform;
    private Vector2 bufferedInput;
    private IPushable_Pullable push_pullable;
    private LatchLogicType latchLogicType;
    private Rigidbody2D push_pullRB;
    private Vector2 tongueVelocity;
    public TongueLatchedState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }
    public override void EnterState()
    {
        //Debug.Log("made it to latched state");
        movementType = LatchMovementType.Waiting;
        player.stateMachine.ChangeState(player.latchedState);
        endOfTongueTransform = tongueStateMachine.GetEndOfTongueTransform();

        // Check if the object is pushable or pullable that we latched onto
        if (push_pullable != null)
        {

            bool pull = push_pullable.isPullableQ();
            bool push = push_pullable.isPushableQ();
            if (push || pull) push_pullRB = push_pullable.GetRigidBody();

            if (pull)
            {
                latchLogicType = LatchLogicType.pullLogic;
            }
            else if (push)
            {
                latchLogicType = LatchLogicType.pushLogic;
                Vector3 latchlocation = push_pullable.GetLatchLocation();
                push_pullRB = push_pullable.GetRigidBody();
                float tongueForceModifier = 100f;
                push_pullRB.AddForceAtPosition(tongueForceModifier*(tongueVelocity.normalized), latchlocation);
            }
            else { latchLogicType = LatchLogicType.baseLogic; }

        }
        else
        {
            latchLogicType = LatchLogicType.baseLogic;
        }
    }

    public override void ExitState()
    {
        push_pullable = null;
        bufferedInput = Vector2.zero;
        //Debug.Log("left latched state");
    }
    //private bool needToReadLatchInputs;
    
    public override void FrameUpdate()
    {
        if (latchLogicType == LatchLogicType.baseLogic)
        {
            movementType = readInput();
        }
    }

    public override void PhysicsUpdate()
    {
        switch (latchLogicType)
        {
            case (LatchLogicType.baseLogic):
                break;
            case (LatchLogicType.pullLogic):
                break;
            case (LatchLogicType.pushLogic):
                player.stateMachine.ChangeState(player.idleState);
                tongueStateMachine.ChangeState(player.tongueRetractingState);
                break;
        }
        switch (movementType)
        {
            case LatchMovementType.Waiting:
                break;
            case LatchMovementType.LungeForward:
                //Debug.Log("lunge forward");
                ChangeToLungingState(movementType);
                break;
            case LatchMovementType.LungeLeft:
                //Debug.Log("lunge left");
                ChangeToLungingState(movementType);
                break;
            case LatchMovementType.LungeRight:
                //Debug.Log("lunge right");
                ChangeToLungingState(movementType);
                break;
            case LatchMovementType.LungeBack:
                //Debug.Log("Lunge back");
                ChangeToLungingState(movementType);
                break;
            default:
                Debug.LogError("invalid state of movement type in TongueLatchedState");
                break;
        }
    }

    private Vector2 ihat;
    private Vector2 jhat;
    public LatchMovementType readInput()
    {

        Vector2 movVec = player.latchedState.getPlayerInput();
        float xInput = movVec.x;
        float yInput = movVec.y;
        if (xInput != 0 || yInput != 0) // if we don't detect a movement then run the below code
        {
            // We have make sure this works for analog also it needs to be rotated to the reference frame relative to the tongue direction
            // EOT = j hat
            // right of this vector is i hat, which is EOTx{0,1,0}; 
            ///Vector3 jhat = endOfTongueTransform.position - parentTransform.position;
            Vector3 jhat = endOfTongueTransform.position - tongueStateMachine.GetParentTransformPosition();
            jhat.z = 0;
            jhat.Normalize();

            Vector3 khat = Vector3.forward;
            Vector3 ihat = Vector3.Cross(jhat, khat); // gets the vector perpendicular to the tongue direction
            //Debug.Log("ihat = " + ihat.x + "," + ihat.y + ",");
            //Debug.Log("jhat = " + jhat.x + ","  + jhat.y + ",");;

            // Now we compute 4 dot products on the vectors {j,0} forward, {-j,0} back, {0,i} right, {0,-i} left. This will give a number between -1 and 1 of how much the vector falls onto a certain direction
            float f, d, r, l;
            f = Vector3.Dot(movVec, jhat);
            d = Vector3.Dot(movVec, -jhat);
            r = Vector3.Dot(movVec, ihat);
            l = Vector3.Dot(movVec, -ihat);
            //Debug.Log("f = " + f);
            //Debug.Log("d = " + d);
            //Debug.Log("r = " + r);
            //Debug.Log("l = " + l);

            // Find the maximum value of these dot products
            float max = Mathf.Max(f, Mathf.Max(d, Mathf.Max(r, l))); // Now the max value will correspond to the action the player is trying to preform
            if (max == f)
            {
                this.ihat = ihat; this.jhat = jhat;
                return LatchMovementType.LungeForward;
            }
            else if (max == d)
            {
                this.ihat = ihat; this.jhat = jhat;
                return LatchMovementType.LungeBack;
            }
            else if (max == r)
            {
                this.ihat = ihat; this.jhat = jhat;
                return LatchMovementType.LungeRight;
            }
            else if (max == l)
            {
                this.ihat = ihat; this.jhat = jhat;
                return LatchMovementType.LungeLeft;
            }
            else
            {
                Debug.LogError("Problem in readInput() in TongueLatchState");
            }

        }
        //Debug.LogError("can't detect state");
        return LatchMovementType.Waiting;
    }


    private void ChangeToLungingState(LatchMovementType m)
    {
        player.lungingState.SetLatchMovementType(m);
        player.tongueLungeState.SetLatchMovementType(m);
        
        player.stateMachine.ChangeState(player.lungingState);
        player.tongueStateMachine.ChangeState(player.tongueLungeState);
    }
    public Vector2 GetIhat()
    {
        return ihat;
    }
    public Vector2 GetJhat()
    {
        return jhat;
    }
    public void SetPushPullable(IPushable_Pullable pushable_Pullable)
    {
        this.push_pullable = pushable_Pullable;
    }
    public void SetTongueVelocityDirection(Vector2 vel)
    {
        tongueVelocity = vel;
    }
}
