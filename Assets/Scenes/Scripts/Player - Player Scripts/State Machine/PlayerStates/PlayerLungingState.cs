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
    int lungeDirection = 1;

    #region Lunge Varaibles
    private NonLinearRadialAccelerator nonLinearRadialAccelerator;
    private float lateralForceModifer;
    private float minimumLateralDuration;
    private float lateralDragCoefficient;
    private float dampingCoefficient;
    private float minimumDistanceToSpawnANewPoint;
    private float minimumTimeToSpawnANewPoint;
    private ContactFilter2D tongeContactFilter;
    private float forwardLungeDragCoefficient;
    private float forwardForceModifer;
    private ContactFilter2D playerContactFilter;
    private float lateralLungeEaseInFrames;
    private float lateralLungeEaseOutFrames;
    private float lateralLungeDesiredVEL;
    protected void getLungeVariables()
    {
        ArrayList vars = player.getLungeVaraiables();
        lateralForceModifer =               (float)vars[0];
        minimumLateralDuration =            (float)vars[1];
        lateralDragCoefficient =            (float)vars[2];
        tongeContactFilter =      (ContactFilter2D)vars[3];
        dampingCoefficient =                (float)vars[4];
        minimumDistanceToSpawnANewPoint =   (float)vars[5];
        minimumTimeToSpawnANewPoint =       (float)vars[6];
        forwardLungeDragCoefficient =       (float)vars[7];
        forwardForceModifer =               (float)vars[8];
        playerContactFilter =     (ContactFilter2D)vars[9];
        lateralLungeEaseInFrames =          (float)vars[10];
        lateralLungeEaseOutFrames =         (float)vars[11];
        lateralLungeDesiredVEL =            (float)vars[12];
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
    float r0;
    public override void EnterState()
    {
        lastPos = Vector2.zero; // TODO: REMOVE THIS LINE, just used for tracer
        getLungeVariables(); // TODO: Remove so this is only called once, just used for updating vars
    
        ihat = player.tongueLatchedState.GetIhat(); // right
        jhat = player.tongueLatchedState.GetJhat(); // forward
        tongueSwingingMode = TongueSwingingMode.TwoBody;
        dampingCounter = 0;
        entryTime = Time.time;

        endOfTongueTransform = player.tongueStateMachine.GetEndOfTongueTransform();
        
        
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                playerRB.drag = forwardLungeDragCoefficient;
                playerRB.AddForce(jhat * forwardForceModifer);
                v0 = Time.fixedDeltaTime * forwardForceModifer / playerRB.mass;
                break;
            case LatchMovementType.LungeLeft:
                playerRB.drag = lateralDragCoefficient;
                /*
                playerRB.AddForce(-ihat*lateralForceModifer);
                v0 = /* dT Fmultiplier / m  Time.fixedDeltaTime * lateralForceModifer / playerRB.mass;
                 Debug.Log("V_0 " + v0);
                */
                // Create a new lateral acclerator 
                r0 = (playerRB.position - (Vector2)endOfTongueTransform.position).magnitude;
                nonLinearRadialAccelerator = new NonLinearRadialAccelerator(lateralLungeEaseInFrames, lateralLungeEaseOutFrames, lateralLungeDesiredVEL, playerRB.mass);
                lungeDirection = -1; // negative is -ihat which is left
                hitData = player.tongueStateMachine.GetRotationPoint();
                break;
            case LatchMovementType.LungeRight:
                playerRB.drag = lateralDragCoefficient;
                /*playerRB.AddForce(ihat*lateralForceModifer);
                v0 = /* dT Fmultiplier / m  Time.fixedDeltaTime * lateralForceModifer / playerRB.mass;
                Debug.Log("V_0 = " + v0);
                */
                // Create a new lateral acclerator 
                r0 = (playerRB.position - (Vector2)endOfTongueTransform.position).magnitude;
                nonLinearRadialAccelerator = new NonLinearRadialAccelerator(lateralLungeEaseInFrames, lateralLungeEaseOutFrames, lateralLungeDesiredVEL, playerRB.mass);
                lungeDirection = 1; // positive is +ihat which is right
                hitData = player.tongueStateMachine.GetRotationPoint();
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
        lungeDirection = 1;
    }

    public override void FrameUpdate()
    {
        // Reading inputs differently based on lunge type.
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                {
                    //GetFKeyInputs();
                    if (fKeyDown)
                    {
                        TryShutOffForForwardsLunge();
                    }
                }
                return;
            case LatchMovementType.LungeLeft:
                {
                    if (fKeyDown)
                    {
                        TryShutOffForLateralLunge(); 
                    }
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
                    if (fKeyDown)
                    {
                        TryShutOffForLateralLunge();
                    }
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
    Vector2 rotationPoint = Vector2.zero; // For two body hits this is just the end of the tongue
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
        switch (tongueSwingingMode)
        {
            case TongueSwingingMode.TwoBody:
                rotationPoint = endOfTongueTransform.position;
                break;
            case TongueSwingingMode.nBody:
                rotationPoint = endOfTongueTransform.position; // TODO-> REMOVE THIS
                // TODO: set this to the line render's 1st spot for where this should be where {0,1,2,3} is {tongue base, the last hit point, the previous hit point, the intial end of tongue};
                break;
        }

        switch (tongueSwingingMode)
        {
            case TongueSwingingMode.TwoBody:
                /// NON LINEAR RAIDAL ACCLERATOR WAS ADDED HERE
                TwoBodySwingingLogic();
                Updateihatjhat(lungeDirection);
               /* Debug.Log("PlayerRB:" + playerRB);
                Debug.Log("endOfTongueTransform" + endOfTongueTransform.position);
                Debug.Log("i:" + ihat);
                Debug.Log("j:" + jhat);*/
                nonLinearRadialAccelerator.FixedUpdateCall(ihat, jhat, playerRB, r0);
                //TwoBodySwingingLogic(rotationPoint, playerPos);
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
    /**** Orginal method ******/
    private void TwoBodySwingingLogic(Vector2 rotationPoint, Vector2 playerPos)
    {
        bool TEMP = true;
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        /*int result = */Physics2D.Linecast(rotationPoint, playerPos, tongeContactFilter, collisions);
        foreach (RaycastHit2D col in collisions)
        {
            if (TEMP == false) return;
            Collider2D collision = col.collider;
            if (collision != null) // Check if the collision is not null
            {
                if (!collision.CompareTag("Player"))
                {
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

                    // *** Debugs ***
                    // Yellow is normal
                    // Red is collision point
                    Debug.Log("Collision Name: " + collision.name);
                    DrawCircle(col.point, 0.03f, 10, Color.red);

                    Vector2 normVector = col.normal;
                    Debug.DrawLine(col.point, col.point + normVector, Color.yellow,5f);
                    ///Debug.Break();
                    // -----------------

                    // Try to line cast from the opposite direction
                    rotationPoint = hitData.getPos();
                    bool collidedOnOtherSide = LineCastOppositeDirection(rotationPoint, playerPos, true);// true will apply force
                    if (collidedOnOtherSide) { 
                        return; 
                    }
                    else{
                        float r2 = (rotationPoint - playerPos).magnitude;
                        ApplyCentripetalForce(playerPos, r1, r2);
                    }

                }
            }
        }
        ApplyCentripetalForce(rotationPoint);
    }

    /***** New Method *******/
    TongueHitData hitData;
    private void TwoBodySwingingLogic()
    {
        hitData = player.tongueStateMachine.GetRotationPoint();
        Vector2 rotationPoint = hitData.getPos();
        Vector2 playerPos = playerRB.position;
        bool TEMP = true;
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        /*int result = */
        Physics2D.Linecast(rotationPoint, playerPos, tongeContactFilter, collisions);
        foreach (RaycastHit2D col in collisions)
        {
            if (TEMP == false) return;
            Collider2D collision = col.collider;
            if (collision != null) // Check if the collision is not null
            {
                if (!collision.CompareTag("Player"))
                {
                    // Instantiate tongue collision object, create a new hit data instance to save, then add it to the array list of new collision points
                    GameObject colObject = GameObject.Instantiate(player.tongueStateMachine.tongeHitCollisionPrefab, col.point, Quaternion.identity);
                    hitData = new TongueHitData(col, TonguePointType.tongueHitPoint, colObject);
                    player.tongueStateMachine.AddNewTongueCollisionPoint(hitData);


                    r0 = (rotationPoint - playerPos).magnitude;
                    dampingCounter++;
                    TEMP = false;
                    lastTimePointSpawned = Time.time;

                    // *** Debugs ***
                    // Yellow is normal
                    // Red is collision point
                    ///Debug.Log("Collision Name: " + collision.name);
                    DrawCircle(col.point, 0.03f, 10, Color.red);

                    Vector2 normVector = col.normal;
                    Debug.DrawLine(col.point, col.point + normVector, Color.yellow,5f);
                    ///Debug.Break();
                    // -----------------

                    // Try to line cast from the opposite direction
                    rotationPoint = hitData.getPos();
                    bool collidedOnOtherSide = LineCastOppositeDirection(playerPos);// true will apply force
                }
            }
        }
    }

    /****** New Method*******/
    public bool LineCastOppositeDirection(Vector2 playerPos)
    {
        Vector2 collisionPoint = hitData.getPos();
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        /*int result = */
        Physics2D.Linecast(playerPos, collisionPoint, tongeContactFilter, collisions);
        foreach (RaycastHit2D col in collisions)
        {
            Collider2D collision = col.collider;
            if (collision != null) // Check if the collision is not null
            {
                if (!collision.CompareTag("Player"))
                {
                    // Instantiate tongue collision object, create a new hit data instance to save, then add it to the array list of new collision points
                    GameObject colObject = GameObject.Instantiate(player.tongueStateMachine.tongeHitCollisionPrefab, col.point, Quaternion.identity);
                    TongueHitData hitData = new TongueHitData(col, TonguePointType.tongueHitPoint, colObject);
                    player.tongueStateMachine.AddNewTongueCollisionPoint(hitData);

                    // No need to save r1 since it was already done 
                    // r1 = (rotationPoint - playerPos).magnitude;
                    // vt1 = v0;

                    dampingCounter++;
                    lastTimePointSpawned = Time.time;
                    rotationPoint = hitData.getPos();
                    r0 = (rotationPoint - playerPos).magnitude;

                    // *** Debugs ***
                    // Yellow is normal
                    //Debug.Log("Collision Name: " + collision.name);
                    DrawCircle(col.point, 0.03f, 10, Color.red);
                    Vector2 normVector = col.normal;
                    Debug.DrawLine(col.point, col.point + normVector, Color.yellow,5f);
                    //Debug.Break();
                    return true;
                }
            }
        }
        return false;
    }




    /***** Old Method *******/
    public bool LineCastOppositeDirection(Vector2 collisionPoint, Vector2 playerPos, bool applyForce)
    {
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        /*int result = */Physics2D.Linecast(playerPos, collisionPoint, tongeContactFilter, collisions);
        foreach (RaycastHit2D col in collisions)
        {
            Collider2D collision = col.collider;
            if (collision != null) // Check if the collision is not null
            {
                if (!collision.CompareTag("Player")) 
                {

                    // Instantiate tongue collision object, create a new hit data instance to save, then add it to the array list of new collision points
                    GameObject colObject = GameObject.Instantiate(player.tongueStateMachine.tongeHitCollisionPrefab, col.point, Quaternion.identity);
                    TongueHitData hitData = new TongueHitData(col, TonguePointType.tongueHitPoint, colObject);
                    player.tongueStateMachine.AddNewTongueCollisionPoint(hitData);
                   
                    // No need to save r1 since it was already done 
                    // r1 = (rotationPoint - playerPos).magnitude;
                    // vt1 = v0;

                    dampingCounter++;
                    lastTimePointSpawned = Time.time;
                    rotationPoint = hitData.getPos();
                    float r2 = (rotationPoint - playerPos).magnitude;

                    if (applyForce)
                    {
                        ApplyCentripetalForce(playerPos, r1, r2);
                    }
                    // *** Debugs ***
                    // Yellow is normal
                    Debug.Log("Collision Name: " + collision.name);
                    DrawCircle(col.point, 0.03f, 10, Color.red);


                    Vector2 normVector = col.normal;
                    Debug.DrawLine(col.point, col.point + normVector, Color.yellow,5f);
                    //Debug.Break();
                    return true;
                }
            }
        }
        return false;
    }
    private void NBodySwingingLogic(Vector2 playerPos)
    {
        // we need to cast on the last point, see if we collide, if we do update the last point, and then apply a rotation around the last point we found. *Only allow one new collision per physics frame

        //TongueHitData lastTonguePoint = player.tongueStateMachine.GetPointBeforeEndOfTongue();
        TongueHitData lastTonguePoint = player.tongueStateMachine.GetRotationPoint();
        Vector2 rotationPoint = lastTonguePoint.getPos();
        bool TEMP = true;
        if (Time.time > lastTimePointSpawned + minimumTimeToSpawnANewPoint)
        {
            RaycastHit2D[] collisions = new RaycastHit2D[3];
            Debug.DrawLine(rotationPoint, playerPos, Color.black);
            int result = Physics2D.Linecast(rotationPoint, playerPos, tongeContactFilter, collisions);
            foreach (RaycastHit2D col in collisions)
            {
                if (TEMP == false) break;
                Collider2D collision = col.collider;
                if (collision != null) // Check if the collision is not null
                {
                    if (!collision.CompareTag("Player"))
                    {
                        { // Do a minimum distance check
                            float distanceFromLastPoint = (rotationPoint - col.point).magnitude;
                            if (distanceFromLastPoint < minimumDistanceToSpawnANewPoint) break;
                        }



                        // Instantiate tongue collision object, create a new hit data instance to save, then add it to the array list of new collision points
                        GameObject colObject = GameObject.Instantiate(player.tongueStateMachine.tongeHitCollisionPrefab, col.point, Quaternion.identity);
                        TongueHitData hitData = new TongueHitData(col, TonguePointType.tongueHitPoint, colObject);
                        player.tongueStateMachine.AddNewTongueCollisionPoint(hitData);


                        r1 = (rotationPoint - playerPos).magnitude; // This is the last rotation point magnitude; 
                        //vt1 = playerRB.velocity.magnitude;
                        
                        dampingCounter++;
                        lastTimePointSpawned = Time.time;
                        TEMP = false;

                        LineCastOppositeDirection(col.point, playerPos, false);

                        // ***Debugs***
                        Debug.Log("Collision Name: " + collision.name);
                        DrawCircle(col.point, 0.03f, 10, Color.red);
                        Vector2 normVector = col.normal;
                        Debug.DrawLine(col.point, col.point + normVector, Color.white);
                        DrawCircle(col.point + normVector, 1f, 5, Color.white);
                        Debug.Break();
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
        ApplyCentripetalForce(playerPos, r1, r2);
        //ApplyCentripetalForce(playerPos, savedR1);
    }

    private void ApplyCentripetalForce(Vector2 rotationPoint)
    {
        Vector2 playerPos = playerRB.position;
        Vector2 forceDirection = (rotationPoint - playerPos);
        float mass = playerRB.mass;
        float radius = forceDirection.magnitude;
        DrawCircle(rotationPoint, radius, 50, Color.blue);
        Debug.DrawLine(rotationPoint, playerPos, Color.blue);
        forceDirection.Normalize();
        playerRB.AddForce(forceDirection * (mass * v0 * v0) / (radius)); // Fc = m v0^2 / r

        Tracer(lastPos, playerPos);
        lastPos = playerPos;
    }
    private void ApplyCentripetalForce(Vector2 playerPos, float radius1, float radius2)
    {
        TongueHitData tongueRotationPoint = player.tongueStateMachine.GetRotationPoint();
        Vector2 rotationPoint = tongueRotationPoint.getPos();
        //radius2 = (rotationPoint - playerPos).magnitude;
        if(radius2 < 0.1f)
        {
            return;
        }
        Vector2 forceDirection = (rotationPoint - playerPos);
        float mass = playerRB.mass;

        DrawCircle(rotationPoint, radius2, 50, new Color(0, Mathf.Pow(dampingCoefficient, dampingCounter), 1));
        
        forceDirection.Normalize();

        float vt2 = mass * vt1 * (radius1 / radius2); // based off conservation of angular momentum
        /*{
            float currentVt1 = playerRB.velocity.magnitude;
            playerRB.velocity *= (vt2 / currentVt1);
        }*/
        float forceMagnitude = mass * Mathf.Pow(vt2,2) / radius2;// Fc2 = * m * vt2^2  / r2
        
        Debug.DrawLine(rotationPoint, playerPos, Color.magenta);
        //Debug.Log("ForceMag"+  (forceDirection * forceMagnitude).magnitude + " Force:" + forceDirection );
        //Debug.Break();
        playerRB.AddForce(forceDirection * forceMagnitude);
        vt2 *= Mathf.Pow(dampingCoefficient, dampingCounter);
        playerRB.velocity = Vector2.ClampMagnitude(playerRB.velocity, vt2);

        Tracer(lastPos, playerPos);
        lastPos = playerPos;
    }

    Vector2 lastPos = Vector2.zero;
    private void Tracer(Vector2 lastPosition, Vector2 currentPosition)
    {
        if (lastPos != Vector2.zero)
        {
            Debug.DrawLine(currentPosition, lastPosition, Color.red, 5f);
        }
    }
    private void ForwardLunge()
    {
        // We need to cast the player's collider forward to check if we hit something.
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        int result = Physics2D.CircleCast(playerRB.position, 0.05f,  jhat, playerContactFilter, collisions, (Time.fixedDeltaTime * v0));
        Debug.DrawLine(playerRB.position, playerRB.position + (Time.fixedDeltaTime * v0 * jhat), Color.green);
        DrawCircle(playerRB.position, 0.05f, 8, Color.green);
        DrawCircle(playerRB.position + (Time.fixedDeltaTime * v0 * jhat), 0.05f, 8,Color.cyan);
        //Debug.Break();
        foreach (RaycastHit2D col in collisions) 
        {
            Collider2D collision = col.collider;
            if(collision != null)
            {
                if (collision.tag != "Player")
                {
                    Debug.Log("Collision's name in forward lunge:" + collision.name);
                    DrawCircle(col.point, 0.03f, 8, Color.red);
                    TryShutOffForForwardsLunge();
                    return;
                }
            }
        }
        bool shutOff = DistanceCheckForForwardLunge();
        if (shutOff) { TryShutOffForForwardsLunge(); return; }
        shutOff = VelocityCheckForForwardLunge();
        if (shutOff) { TryShutOffForForwardsLunge(); return; }
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
        playerStateMachine.ChangeState(player.slowingState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    private void TryShutOffFOrBackwardsLunge()
    {
        Debug.Log("Trying to shut off for backwards lunge");
        playerStateMachine.ChangeState(player.slowingState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    private bool DistanceCheckForForwardLunge()
    {
        Vector3 endOfTonguePos = endOfTongueTransform.position;
        Vector3 playerPos = playerRB.position;
        float distance = (endOfTonguePos - playerPos).magnitude;
        if(distance < 0.1f)
        {
            return true;
        }
        return false;
    }
    private bool VelocityCheckForForwardLunge()
    {
        float velocityMagnitude = playerRB.velocity.magnitude;
        if (velocityMagnitude < 0.1f)
        {
            return true;
        }
        return false;
    }

    /******NEW METHOD*********/
    private void Updateihatjhat(int direction)
    {
        // We have make sure this works for analog also it needs to be rotated to the reference frame relative to the tongue direction
        // EOT = j hat
        // right of this vector is i hat, which is EOTx{0,1,0};

        //jhat = endOfTongueTransform.position - (Vector3)playerRB.position;
        jhat = hitData.getPos() - (Vector3)playerRB.position;
        jhat.Normalize();

        Vector3 khat = Vector3.forward;
        ihat = direction*Vector3.Cross(jhat, khat); // gets the vector perpendicular to the tongue direction
    }


    /**OLD METHOD*/
    private void Updateihatjhat()
    {
        // We have make sure this works for analog also it needs to be rotated to the reference frame relative to the tongue direction
        // EOT = j hat
        // right of this vector is i hat, which is EOTx{0,1,0}; 
        TongueHitData rotationPoint = player.tongueStateMachine.GetRotationPoint();

        //jhat = endOfTongueTransform.position - (Vector3)playerRB.position;
        jhat = rotationPoint.getPos() - (Vector3)playerRB.position;
        jhat.Normalize();

        Vector3 khat = Vector3.forward;
        ihat =  Vector3.Cross(jhat, khat); // gets the vector perpendicular to the tongue direction
    }
}
