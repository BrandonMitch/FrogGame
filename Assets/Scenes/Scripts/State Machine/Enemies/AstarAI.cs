using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

[CreateAssetMenu(fileName ="Basic Move State",menuName = "State Machines/States/Basic Move State")]
public class AstarAI : BaseDataState<Enemy, object>, IMove
{
    private Seeker seeker;
    private Path path;
    private Rigidbody2D RB;

    [SerializeField] public float speed = 2f;

    [SerializeField] public float breakingDistance = 0.3f;
    [SerializeField] public float nextWaypointDistance = 0.2f;
    [SerializeField] public float shuttOffDistance = 0.1f;
    [SerializeField] public float velocityClamp = 0.1f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;

    [SerializeField] private float repathRate = 0.2f;
    [SerializeField] private bool allowIdleTimeOut = false;
    private CoolDownBasic repathCooldown; 
    private Request.OnFuffill onComplete = null;

    private int idleCounter;
    private Request idleRequest;

    // Various ways for other states to call the get position.
    private MoveRequest.FrameUpdatedPosition frameUpdatedPosition = null;
    private Transform targetTransform;
    private Vector2 targetLocation;

#if UNITY_EDITOR
    [SerializeField] private bool drawDebugs;
#endif
    // Start a new path to the targetPosition, call the the OnPathComplete function
    // when the path has been calculated (which may take a few frames depending on the complexity)
    //  seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    #region Basic State Functions
    public override void Initialize(IStateMachine<Enemy> stateMachine)
    {
        base.Initialize(stateMachine);
        RB = reference.GetRigidbody();
        seeker = reference.Seeker;
        if(seeker == null)
        {
            Debug.Log("Enemy doesn't contain a seeker component");
        }
        repathCooldown = new CoolDownBasic(repathRate, startOnCreation:false);
    }
    public override void EnterState()
    {
        Debug.Log("Entered AstarAI");
        base.EnterState();
        repathCooldown.StartCooldown();
        repathCooldown.SkipCooldown();
    }

    public override void ExitState()
    {
        base.ExitState();
        idleCounter = 0;
        idleRequest = null;
        frameUpdatedPosition = null;
        targetTransform = null;
    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        //Debug.Log("Cool Down Status:" + repathCooldown.IsCooldownComplete + "," + repathCooldown.CoolDownProgressUnclamped);
        if(repathCooldown.IsCooldownComplete && seeker.IsDone())
        {
            if (Vector2.Distance(RB.position, GetTargetLocation()) > shuttOffDistance)
            {
                repathCooldown.StartCooldown();
                seeker.StartPath(RB.position, GetTargetLocation(), OnPathComplete);
            }
            else
            {
                OnDestinationReached();
                return;
            }
        }

        //  if We have no path to follow don't do anything. But if we have been idling for 2s send a request to the state machine to change to idle
        if (path == null)
        {
            idleCounter++;
            
            if (allowIdleTimeOut && idleCounter > 199)
            {
                // If we reached the threshold for a idle time out, then make a request to the state machine to change to idle mode
                if(idleRequest == null)
                {
                    idleRequest = new Request(locationRequest: Vector2.zero, Request.SimpleRequest.Idle);
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            idleCounter = 0;
        }

        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector2.Distance(RB.position, path.vectorPath[currentWaypoint]);
#if UNITY_EDITOR
            var t = GetTargetLocation();
            Tracer.DrawCircle(t, breakingDistance, 8, Color.red);
            Tracer.DrawCircle(path.vectorPath[currentWaypoint], nextWaypointDistance, 8, Color.magenta);
            Tracer.DrawCircle(t, shuttOffDistance, 8, Color.yellow);
#endif
            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // You can use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        if(distanceToWaypoint > breakingDistance){
            var velMag = RB.velocity.magnitude;
            if (velMag <= velocityClamp)
            {
                // Slow down smoothly upon approaching the end of the path
                // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
                var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

                // Direction to the next waypoint
                // Normalize it so that it has a length of 1 world unit
                Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - RB.position).normalized;

                // Multiply the direction by our desired speed to get a velocity
                Vector2 velocity = speed * speedFactor * dir;

                // Move the agent using the CharacterController component
                // If you are writing a 2D game you should remove the CharacterController code above and instead move the transform directly by uncommenting the next line
                //reference.transform.position += velocity * Time.deltaTime;
                RB.velocity *= speedFactor;
                RB.velocity += velocity * Time.fixedDeltaTime;
            }
        }
        else
        {
            var velMag = RB.velocity.magnitude;
            if (velMag <= velocityClamp)
            {
                var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

                // Direction to the next waypoint
                // Normalize it so that it has a length of 1 world unit
                Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - RB.position).normalized;
                Vector2 velocity = 0.2f * speed * speedFactor * dir;
                RB.velocity *= speedFactor;
                RB.velocity += velocity * Time.fixedDeltaTime;
                RB.velocity = Vector2.ClampMagnitude(RB.velocity, velocityClamp);
            }
            else
            {
                RB.velocity = Vector2.ClampMagnitude(RB.velocity, velocityClamp);
            }
        }

        if (reachedEndOfPath)
        {
            OnDestinationReached();
        }
    }

    public override void RecieveData(object data)
    {
        base.RecieveData(data);
        MoveRequest position = data as MoveRequest;
        if(position != null)
        {
            if(position.frameUpdatedPosition != null)
            {
                this.frameUpdatedPosition = position.frameUpdatedPosition;
            }
            else
            {
                this.targetLocation = position.pos;
            }

        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
        seeker = null;
        RB = null;
        path = null;
        targetTransform = null;
        idleRequest = null;
        onComplete = null;
        repathCooldown = null;
        idleCounter = 0;
    }

    public override object SendData()
    {
        return base.SendData();
    }
    #endregion

    #region Special Functions
    public void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        // Path pooling. To avoid unnecessary allocations paths are reference counted.
        // Calling Claim will increase the reference count by 1 and Release will reduce
        // it by one, when it reaches zero the path will be pooled and then it may be used
        // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
        // take a path from the pool if possible. See also the documentation page about path pooling.
        p.Claim(this);
        if (!p.error)
        {
            if (path != null) { path.Release(this); }
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }

    public void MoveTo(Vector2 location, Request.OnFuffill onComplete = null)
    {
        targetLocation = location;
        targetTransform = null;
        this.onComplete = onComplete;
    }

    public void MoveTo(Transform location, bool keepUpdating = false, Request.OnFuffill onComplete = null)
    {
        if (keepUpdating)
        {
            targetTransform = location.transform;
        }
        else
        {
            targetLocation = location.transform.position;
        }
        this.onComplete = onComplete;
    }

    public void MoveTo(Rigidbody2D location, bool keepUpdating = false, Request.OnFuffill onComplete = null)
    {
        MoveTo(location.transform, keepUpdating, onComplete);
    }

    public void OnDestinationReached()
    {
        Debug.Log("Destination Reached, Oncomplete = " + onComplete);
        if (onComplete != null)
        {
            Debug.Log("Calling On complete delegate: " + onComplete.ToString());
            onComplete.Invoke();
            onComplete = null;
        }
        if (allowIdleTimeOut && Equals(SM.getCurrentState(), this))
        {
            if (idleRequest == null)
            {
                idleRequest = new Request(locationRequest: Vector2.zero, Request.SimpleRequest.Idle);
                SM.TakeRequest(idleRequest);
            }
        }
    }

    /// <summary>
    /// This method gets the target location that was applied.
    /// There are three ways it can get the location:
    /// (1) Giving it a vector2 location
    /// (2) Providing a transform for it to follow
    /// (3) By providing a delegate that returns a vec2 for it to update every frame.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetTargetLocation()
    {
        Debug.Log("Get Target Location(), delegate:" + frameUpdatedPosition + "is delegate null?" + (frameUpdatedPosition == null).ToString());
    
        var pos = Vector2.zero;
        if(frameUpdatedPosition != null)
        {
            return frameUpdatedPosition.Invoke();
        }
        else if(targetTransform != null)
        {
            return targetTransform.position;
        }
        else
        {
            return targetLocation;
        }
    }


    #endregion
}
