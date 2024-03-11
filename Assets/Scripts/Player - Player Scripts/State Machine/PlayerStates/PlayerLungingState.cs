using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class PlayerLungingState : PlayerState
{
    /* Lunge variables */
    private LatchMovementType latchMovementType = LatchMovementType.Waiting;
    private TongueSwingingMode tongueSwingingMode;
    private Vector2 jhat;
    private Vector2 ihat;
    private Vector2 inputs;

    private float entryTime;

    #region Lunge Varaibles
    private float lateralForceModifer;
    private float minimumLateralDuration;
    private float lateralDragCoefficient;
    private float dampingCoefficient;
    private float minimumDistanceToSpawnANewPoint;
    private float minimumTimeToSpawnANewPoint;
    private ContactFilter2D tongeContactFilter;

    protected void getLungeVariables()
    {
        ArrayList vars = player.getLungeVaraiables();
        lateralForceModifer = (float)vars[0];
        minimumLateralDuration = (float)vars[1];
        lateralDragCoefficient = (float)vars[2];
        tongeContactFilter = (ContactFilter2D)vars[3];
        dampingCoefficient = (float)vars[4];
        minimumDistanceToSpawnANewPoint = (float)vars[5];
        minimumTimeToSpawnANewPoint = (float)vars[6];
    }
    private float v0;
    #endregion

    /*End of Tongue Variables*/
    private Transform endOfTongueTransform;

    /*Player Values*/
    private Rigidbody2D playerRB;
    private Collider2D playerCollider;
    
    public PlayerLungingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        playerRB = player.GetPlayerRigidBody();
        playerCollider = player.GetCollider();
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        playerCollider = player.GetComponent<Collider2D>();
    }

    public override void EnterState()
    {
        getLungeVariables(); // TODO: Remove so this is only called once, just used for updating vars
    
        ihat = player.tongueLatchedState.GetIhat();
        jhat = player.tongueLatchedState.GetJhat();
        tongueSwingingMode = TongueSwingingMode.TwoBody;
        dampingCounter = 0;
        entryTime = Time.time;

        endOfTongueTransform = player.tongueStateMachine.GetEndOfTongueTransform();
        
        
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                break;
            case LatchMovementType.LungeLeft:
                playerRB.drag = lateralDragCoefficient;
                playerRB.AddForce(-ihat*lateralForceModifer);
                v0 = /* dT Fmultiplier / m */ Time.fixedDeltaTime * lateralForceModifer / playerRB.mass;

                Debug.Log("V_0 " + v0);
                break;
            case LatchMovementType.LungeRight:
                playerRB.drag = lateralDragCoefficient;
                playerRB.AddForce(ihat*lateralForceModifer);
                v0 = /* dT Fmultiplier / m */ Time.fixedDeltaTime * lateralForceModifer / playerRB.mass;
                Debug.Log("V_0 = " + v0);
                break;
            case LatchMovementType.LungeBack:
                break;
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR IN player lunging state on entry, should not be waiting");
                break;
        }
    }

    public override void ExitState()
    {
        SetLatchMovementType(LatchMovementType.Waiting);
    }

    public override void FrameUpdate()
    {
        // Reading inputs differently based on lunge type.
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                return; 
            case LatchMovementType.LungeLeft:
                {
                    Vector2 input = GetCurrentMovementInputs();
                    if (input != Vector2.zero)
                    {
                        return;
                    }
                    TryShutOffForLateralLunge();
                }
                return; 
            case LatchMovementType.LungeRight:
                {
                    Vector2 input = GetCurrentMovementInputs();
                    if (input != Vector2.zero)
                    {
                        return;
                    }
                    TryShutOffForLateralLunge();
                }
                return; 
            case LatchMovementType.LungeBack:
                return; 
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR in player lunging state, state should not be waiting");
                return; 
            default:
                Debug.LogError("ERROR in player lunging state, invalid state");
                return;
        }
    }

    public override void PhysicsUpdate()
    {
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                ForwardLunge();
                break;
            case LatchMovementType.LungeLeft:
                LateralLunge(latchMovementType);
                break;
            case LatchMovementType.LungeRight:
                LateralLunge(latchMovementType);
                break;
            case LatchMovementType.LungeBack:
                BackwardsLunge();
                break;
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR in player lunging state, state should not be waiting");
                return;
            default:
                Debug.LogError("ERROR in player lunging state, invalid state");
                return;
        }
    }
    public void SetLatchMovementType(LatchMovementType m)
    {
        latchMovementType = m;
    }
    private void LateralLunge(LatchMovementType direction)
    {
        
        switch (direction)
        {
            case LatchMovementType.LungeLeft:
                break;
            case LatchMovementType.LungeRight:
                break;
            default:
                Debug.LogError("Error in player lateral lunge state, should not be waiting or invalid state");
                return;
        } // log error if we are in the wrong mode

        Vector2 playerPos = playerRB.position;
        Vector2 rotationPoint = Vector2.zero; // For two body this is just the end of the tongue;
        switch (tongueSwingingMode)
        {
            case TongueSwingingMode.TwoBody:
                rotationPoint = endOfTongueTransform.position;
                break;
            case TongueSwingingMode.nBody:
                rotationPoint = endOfTongueTransform.position;
                // TODO: set this to the line render's 1st spot for where this should be where {0,1,2,3} is {tongue base, the last hit point, the previous hit point, the intial end of tongue};
                break;
        }

        switch (tongueSwingingMode)
        {
            case TongueSwingingMode.TwoBody:
                TwoBodySwingingLogic(rotationPoint, playerPos);
                break;
            case TongueSwingingMode.nBody:
                NBodySwingingLogic(playerPos);
                break;
        }
    }
    private float r1; // intial radius
    private float vt1; // intial tangental velocity
    private float dampingCounter; // how many times to apply the damping coefficient
    private float lastTimePointSpawned;
    private void TwoBodySwingingLogic(Vector2 rotationPoint, Vector2 playerPos)
    {
        bool TEMP = true;
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        int result = Physics2D.Linecast(rotationPoint, playerPos, tongeContactFilter, collisions);
        foreach (RaycastHit2D col in collisions)
        {
            if (TEMP == false) return;
            Collider2D collision = col.collider;
            if (collision != null) // Check if the collision is not null
            {
                if (collision.tag != "Player")
                {
                    Debug.Log("Collision Name: " + collision.name);
                    DrawCircle(col.point, 0.03f, 10, Color.red);

                    // Instantiate tongue collision object, create a new hit data instance to save, then add it to the array list of new collision points
                    GameObject colObject = GameObject.Instantiate(player.tongueStateMachine.tongeHitCollisionPrefab, col.point, Quaternion.identity);
                    TongueHitData hitData = new TongueHitData(col, TonguePointType.tongueHitPoint, colObject);
                    player.tongueStateMachine.AddNewTongueCollisionPoint(hitData);

                    tongueSwingingMode = TongueSwingingMode.nBody;

                    r1 = (rotationPoint - playerPos).magnitude;
                    vt1 = v0;
                    dampingCounter++;
                    TEMP = false;
                    lastTimePointSpawned = Time.time;

                    rotationPoint = hitData.getPos();
                    float r2 = (rotationPoint - playerPos).magnitude;
                    ApplyCentripetalForce(rotationPoint, playerPos, r1, r2);
                }
            }
        }
        ApplyCentripetalForce(rotationPoint);
    }
    private void NBodySwingingLogic(Vector2 playerPos)
    {
        // we need to cast on the last point, see if we collide, if we do update the last point, and then apply a rotation around the last point we found. *Only allow one new collision per physics frame

        TongueHitData lastTonguePoint = player.tongueStateMachine.GetPointBeforeEndOfTongue();
        Vector2 rotationPoint = lastTonguePoint.getPos();
        bool TEMP = true;
        if (Time.time > lastTimePointSpawned + minimumTimeToSpawnANewPoint)
        {
            RaycastHit2D[] collisions = new RaycastHit2D[3];
            int result = Physics2D.Linecast(rotationPoint, playerPos, tongeContactFilter, collisions);
            foreach (RaycastHit2D col in collisions)
            {
                if (TEMP == false) break;
                Collider2D collision = col.collider;
                if (collision != null) // Check if the collision is not null
                {
                    if (collision.tag != "Player")
                    {
                        { // Do a minimum distance check
                            float distanceFromLastPoint = (rotationPoint - col.point).magnitude;
                            if (distanceFromLastPoint < minimumDistanceToSpawnANewPoint) break;
                        }

                        Debug.Log("Collision Name: " + collision.name);
                        DrawCircle(col.point, 0.03f, 10, Color.red);
                        // Instantiate tongue collision object, create a new hit data instance to save, then add it to the array list of new collision points
                        GameObject colObject = GameObject.Instantiate(player.tongueStateMachine.tongeHitCollisionPrefab, col.point, Quaternion.identity);
                        TongueHitData hitData = new TongueHitData(col, TonguePointType.tongueHitPoint, colObject);
                        player.tongueStateMachine.AddNewTongueCollisionPoint(hitData);


                        r1 = (rotationPoint - playerPos).magnitude; // This is the last rotation point magnitude; 
                        vt1 = playerRB.velocity.magnitude;

                        dampingCounter++;
                        lastTimePointSpawned = Time.time;
                        TEMP = false;
                    }
                }
            }
        }
        float r2;
        if(TEMP == false) // if there was a new collision
        { // find the new last tongue position
            lastTonguePoint = player.tongueStateMachine.GetPointBeforeEndOfTongue();
            rotationPoint = lastTonguePoint.getPos();
            r2 = (rotationPoint - playerPos).magnitude;

        }
        else
        {
            r2 = r1;
        }
        ApplyCentripetalForce(rotationPoint, playerPos, r1, r2);

    }

    private void ApplyCentripetalForce(Vector2 rotationPoint)
    {
        Vector2 playerPos = playerRB.position;
        Vector2 forceDirection = (rotationPoint - playerPos);
        float mass = playerRB.mass;
        float radius = forceDirection.magnitude;
        DrawCircle(rotationPoint, radius, 50, Color.blue);
        forceDirection.Normalize();
        playerRB.AddForce(forceDirection * (mass * v0 * v0) / (radius)); // Fc = m v0^2 / r
    }
    private void ApplyCentripetalForce(Vector2 rotationPoint, Vector2 playerPos, float radius1, float radius2)
    {
        Vector2 forceDirection = (rotationPoint - playerPos);
        float mass = playerRB.mass;

        DrawCircle(rotationPoint, radius2, 50, new Color(0,dampingCoefficient*5,255));
        
        forceDirection.Normalize();

        float vt2 = mass * vt1 * (radius1 / radius2); // based off conservation of angular momentum
 
        float forceMagnitude = mass * Mathf.Pow(vt2,2) / radius2;// Fc2 = * m * vt2^2  / r2
        
        Debug.DrawLine(rotationPoint, playerPos, Color.magenta);
        //Debug.Log("ForceMag"+  (forceDirection * forceMagnitude).magnitude + " Force:" + forceDirection );
        //Debug.Break();
        playerRB.AddForce(forceDirection * forceMagnitude);
        vt2 *= Mathf.Pow(dampingCoefficient, dampingCounter);
        playerRB.velocity = Vector2.ClampMagnitude(playerRB.velocity, vt2);
    }

    private void ForwardLunge()
    {
        TryShutOffForForwardsLunge();
    }
    private void BackwardsLunge()
    {
        TryShutOffFOrBackwardsLunge();
    }
    private void TryShutOffForLateralLunge()
    {
        //Debug.Log("trying to shut off lateral lunge");
        if (Time.time > minimumLateralDuration + entryTime)
        {
            //Debug.Log("sucess!");
            ShutOffTongueForLateral();
        }
    }
    private void ShutOffTongueForLateral()
    {
        playerStateMachine.ChangeState(player.slowingState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    private void TryShutOffForForwardsLunge()
    {
        Debug.Log("Trying to shut off forward lunge");
        playerStateMachine.ChangeState(player.idleState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    private void TryShutOffFOrBackwardsLunge()
    {
        Debug.Log("Trying to shut off for backwards lunge");
        playerStateMachine.ChangeState(player.idleState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
}
