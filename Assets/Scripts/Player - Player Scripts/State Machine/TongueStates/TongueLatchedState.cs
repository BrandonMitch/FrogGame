using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueLatchedState : TongueState
{
    public enum LatchMovementType
    {
        None,
        Waiting,
        LungeForward,
        LungeLeft,
        LungeRight,
        LungeBack,
    };
    private LatchMovementType movementType;
    private Transform endOfTongueTransform;
    private Transform parentTransform;
    private Vector2 bufferedInput;
    public TongueLatchedState(Player player, TongueStateMachine tongueStateMachine) : base(player, tongueStateMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log("made it to latched state");
        movementType = LatchMovementType.Waiting;
        player.stateMachine.ChangeState(player.latchedState);
        endOfTongueTransform = tongueStateMachine.endOfTongue.transform;
        parentTransform = tongueStateMachine.parentTransform;
    }

    public override void ExitState()
    {
        bufferedInput = Vector2.zero;
        Debug.Log("left latched state");
    }

    public override void FrameUpdate()
    {
        movementType = readInput();
    }

    public override void PhysicsUpdate()
    {
        switch (movementType)
        {
            case LatchMovementType.None:
                break;
            case LatchMovementType.Waiting:
                break;
            case LatchMovementType.LungeForward:
                Debug.Log("LUNGE FORWARD");
                player.stateMachine.ChangeState(player.lungingState);
                player.tongueStateMachine.ChangeState(player.tongueLungeState);
                break;
            case LatchMovementType.LungeLeft:
                Debug.Log("lunge left");
                player.stateMachine.ChangeState(player.lungingState);
                player.tongueStateMachine.ChangeState(player.tongueLungeState);
                break;
            case LatchMovementType.LungeRight:
                Debug.Log("lunge right");
                player.stateMachine.ChangeState(player.lungingState);
                player.tongueStateMachine.ChangeState(player.tongueLungeState);
                break;
            case LatchMovementType.LungeBack:
                Debug.Log("Lunge back");
                player.stateMachine.ChangeState(player.lungingState);
                player.tongueStateMachine.ChangeState(player.tongueLungeState);
                break;
            default:
                Debug.LogError("invalid state of movement type in TongueLatchedState");
                break;
        }
    }

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
            Vector3 jhat = endOfTongueTransform.position - parentTransform.position;
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
                return LatchMovementType.LungeForward;
            }
            else if (max == d)
            {
                return LatchMovementType.LungeBack;
            }
            else if (max == r)
            {
                return LatchMovementType.LungeRight;
            }
            else if (max == l)
            {
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
    /** public void RecieveBufferedMovementFromThrowingState(Vector2 bufferedMovementInput)
    {
        this.bufferedInput = bufferedMovementInput;
    }**/
}
