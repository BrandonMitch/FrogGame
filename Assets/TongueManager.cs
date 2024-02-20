using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform parentTransform;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private Vector3 aimLocation;
    [SerializeField] private Transform endOfTongueTransform;
    public enum TongueState{
        StartShutOff,
        Off,
        Aiming,
        Throw,
        Thrown,
        Hit,
        Latch,
        LungeForward,
        LungeRight,
        LungeLeft,
        LungeBack,
    };
    public enum LatchMovementType
    {
        None,
        Waiting,
        LungeForward,
        LungeLeft,
        LungeRight,
        LungeBack,
    };
    [SerializeField] private TongueState tongueState;
    [SerializeField] private LatchMovementType movementType;
    [SerializeField] private EndOfTongueScript endOfTongue;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        parentTransform = GetComponentInParent<Transform>();
        tongueState = TongueState.StartShutOff;
        movementType = LatchMovementType.None;
    }

    // Update is called once per frame
    void Update()
    {
        manageTongueState();
    }
    public void moveEndOfTongue(Vector3 location)
    {
        endOfTongueTransform.position = location;
    }

    public TongueState getTongueState()
    {
        return tongueState;
    }
    public void moveTongueTowards(Vector3 location)
    {
        Vector2 direction = new Vector2(location.x, location.y);
        direction = (direction - (Vector2)transform.position);
        direction.Normalize();
        rigidBody2D.AddForce(20*direction);
    }
    public void manageTongueState()
    {
        switch (tongueState)
        {
            case TongueState.StartShutOff: // Start Shut off
                lineRenderer.enabled = false;
                rigidBody2D.velocity = Vector2.zero;
                rigidBody2D.simulated = false;
                changeTongueState(0);
                break;
            case TongueState.Off: // Off
                break;
            case TongueState.Aiming: // Aiming
                rigidBody2D.velocity = Vector2.zero;
                break;
            case TongueState.Throw: // Spitting tongue
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, parentTransform.position);
                lineRenderer.SetPosition(1, endOfTongueTransform.position);
                //endOfTongue.SetPosition(transform.position);
                endOfTongueTransform.position = transform.position;
                rigidBody2D.simulated = true;
                tongueState = TongueState.Thrown;
                break;
            case TongueState.Thrown:
                lineRenderer.SetPosition(0, parentTransform.position);
                //lineRenderer.SetPosition(1, endOfTongueTransform.position);
                lineRenderer.SetPosition(1, endOfTongueTransform.position);
                // NEED TO RAY CAST
                DistanceTest();
                break;
            case TongueState.Hit: // On Tongue Hit
                rigidBody2D.velocity = Vector2.zero;
                tongueState = TongueState.Latch;
                break;
            case TongueState.Latch: // On Tongue Latch
                movementType = readInputs();
                lineRenderer.SetPosition(0, parentTransform.position);
                if ( (movementType != LatchMovementType.Waiting) && (movementType != LatchMovementType.None) )
                {
                    ProcessInputs();
                }
                break;
            default:
                Debug.LogError("Improper State of Tongue Manager");
                break;
        }
    }
    private void changeTongueState(int state)
    {
        switch (state)
        {
            case -1: // Start Shut Off
                tongueState = TongueState.StartShutOff;
                break;
            case 0: // Off
                tongueState = TongueState.Off;
                break;
            case 1: // Aiming tongue
                tongueState = TongueState.Aiming;
                break;
            case 2: // Spitting tongue
                tongueState = TongueState.Throw;
                break;
            case 3: // On Tongue Hit
                tongueState = TongueState.Hit;
                break;
            case 4: // On Tongue Latch
                tongueState = TongueState.Latch;
                break;
            default:
                Debug.LogError("Improper State of Tongue Manager no state: " + state);
                break;
        }
    }
    public void AimTongue(Vector3 location) // Is to be called once
    {
        changeTongueState(1); // is aiming
        aimLocation = location;
    }
    public bool TryThrowTongue(Vector3 location)
    {
        aimLocation = location;
        changeTongueState(2); // is throwing
        moveTongueTowards(location);
        return true; // Successful
        //return false; // Fail
    }
    public bool TryToLatch(Vector3 latch_Location, RaycastHit2D hit)
    {
        bool returnVal = true;
        if (returnVal) { 
           StartCoroutine( OnHit(latch_Location, hit) ); 
        }
        // TODO: Implement false return values, this will let us decided what we don't want to latch onto
        return returnVal;
    }
    public void DistanceTest()
    {
        Vector2 vel = rigidBody2D.velocity;
        Vector2 currentPos = rigidBody2D.position;
        Vector2 nextPosition = currentPos + vel * Time.fixedDeltaTime;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(currentPos, 0.02f, vel,(nextPosition-currentPos).magnitude);
        bool isLatched = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (!isLatched) // if not latched then try to latch
            {
                if ((hit.collider.gameObject.GetComponent("Colliad") as Colliad) != null)
                {
                    // TODO: Special Case for collider objects where we check their mass and if it is less than ours,
                }  // We pull the object towards us, otherwise, go towards it
                else { 
             
                } 
                Vector2 Latch_location = hit.point;
                Debug.Log("try to latch called");
                isLatched = TryToLatch(Latch_location, hit);
                
            }
        }
        Debug.DrawLine(currentPos, nextPosition);
    }

    IEnumerator OnHit(Vector3 latch_location, RaycastHit2D hit)
    {
        tongueState = TongueState.Hit;
        Debug.Log("changed states, hopefully we wait a frame");
        yield return new WaitForFixedUpdate();
        Debug.Log("after the yield");
        moveEndOfTongue(latch_location);
    }
    public LatchMovementType readInputs() // Called on latch
    {
        Vector2 movVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        float xInput = movVec.x;
        float yInput = movVec.y;
        // TODO: make sure this works for analog also it needs to be rotated to the refernce frame relative to the tongue direction
        if( (xInput == 0) && (yInput == 0))
        {
            return LatchMovementType.Waiting;
        } else if( yInput > 0) {
            return LatchMovementType.LungeForward;
        } else if (yInput < 0) {
            return LatchMovementType.LungeBack;
        } else if (xInput > 0) {
            return LatchMovementType.LungeRight;
        } else if (xInput < 0) {
            return LatchMovementType.LungeLeft;
        }
        Debug.LogError("can't detect state");
        return LatchMovementType.Waiting;
    }
    public void ProcessInputs()
    {
        switch (movementType) // execute method for each method type and then set the tongue state
        {
            case LatchMovementType.LungeForward:
                LungeForward();
                tongueState = TongueState.LungeForward;
                break;
            case LatchMovementType.LungeBack:
                LungeBack();
                tongueState = TongueState.LungeBack;
                break;
            case LatchMovementType.LungeLeft:
                LungeLeft();
                tongueState = TongueState.LungeLeft;
                break;
            case LatchMovementType.LungeRight:
                LungeRight();
                tongueState = TongueState.LungeRight;
                break;
            default:
                Debug.LogError("Not implemented yet for " + movementType);
                return;
        }
        
    }
    public void LungeForward()
    {

    }
    public void LungeBack()
    {
        Debug.LogError("Not implemented yet for " + movementType);
    }
    public void LungeLeft()
    {
        Debug.LogError("Not implemented yet for " + movementType);
    }
    public void LungeRight()
    {
        Debug.LogError("Not implemented yet for " + movementType);
    }

}
