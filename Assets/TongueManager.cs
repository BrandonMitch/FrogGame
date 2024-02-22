using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform parentTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private Vector3 aimLocation;
    [SerializeField] private Transform endOfTongueTransform;
    [SerializeField] private float lungeTime = 2.0f;
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
        PlayerHit,
        Lunging,
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
    public void movePlayerTowards(Vector3 location)
    {
        Vector2 direction = new Vector2(location.x, location.y);
        direction = (direction - (Vector2)transform.position);
        direction.Normalize();
        playerRB.AddForce(150 * direction);
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
                DistanceTestTongue();
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
            case TongueState.LungeForward:
                movePlayerTowards(endOfTongueTransform.position);
                lineRenderer.SetPosition(0, parentTransform.position);
                tongueState = TongueState.Lunging;
                break;
            case TongueState.Lunging:
                DistanceTestPlayer();
                break;
            case TongueState.PlayerHit:
                playerRB.velocity = Vector2.zero;
                tongueState = TongueState.StartShutOff;
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
    public void DistanceTestTongue()
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
    public void DistanceTestPlayer()
    {
        Vector2 vel = playerRB.velocity;
        Vector2 currentPos = playerRB.position;
        Vector2 nextPosition = currentPos + vel * Time.fixedDeltaTime;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(currentPos, 0.2f, vel, (nextPosition - currentPos).magnitude);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("PLAYER HIT" + hit);
            Vector2 Latch_location = hit.point;
            StartCoroutine(OnHitPlayer(hit));
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
    IEnumerator OnHitPlayer(RaycastHit2D hit)
    {
        Debug.Log("next frame we hit, wait a frame");
        yield return new WaitForFixedUpdate();
        Debug.Log("After waiting, state should change to PlayerHit");
        tongueState = TongueState.PlayerHit;
    }
    public LatchMovementType readInputs() // Called on latch
    {
        Vector2 movVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        float xInput = movVec.x;
        float yInput = movVec.y;
        if (xInput != 0 || yInput != 0) // if we don't detect a movement then run the below code
        {
            // We have make sure this works for analog also it needs to be rotated to the refernce frame relative to the tongue direction
            // EOT = j hat
            // right of this vector is i hat, which is EOTx{0,1,0}; 
            Vector3 jhat = endOfTongueTransform.position - transform.position;
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
            Debug.Log("f=" + f);
            Debug.Log("d=" + d);
            Debug.Log("r=" + r);
            Debug.Log("l=" + l);
            // Find the maximum value of these dot products
            float max = Mathf.Max(f, Mathf.Max(d, Mathf.Max(r, l))); // Now the max value will correspond to the action the player is trying to preform
            if        (max == f) {
                return LatchMovementType.LungeForward;
            } else if (max == d) {
                return LatchMovementType.LungeBack;
            } else if (max == r) {
                return LatchMovementType.LungeRight;
            } else if (max == l) {
                return LatchMovementType.LungeLeft;
            }
            else
            {
                Debug.LogError("Problem");
            }
            
        }
        //Debug.LogError("can't detect state");
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
        Debug.Log("Forward");
    }
    public void LungeBack()
    {
        Debug.Log("Backwards");
    }
    public void LungeLeft()
    {
        Debug.Log("Left");
    }
    public void LungeRight()
    {
        Debug.Log("Right");
    }
}
