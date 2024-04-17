using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

public class PlayerLungingState : PlayerState
{
    /* Lunge variables */
    private LatchMovementType latchMovementType = LatchMovementType.Waiting;
    private Vector2 jhat;
    private Vector2 ihat;

    private float entryTime;
    int lungeDirection = 1;

    #region Lunge Varaibles
    private NonLinearRadialAccelerator nonLinearRadialAccelerator = null;

    //private float dampingCoefficient;
    private float minimumDistanceToSpawnANewPoint;
    private float minimumTimeToSpawnANewPoint;
    private ContactFilter2D tongeContactFilter;
    private ContactFilter2D playerContactFilter;
    private GenericStat minimumLateralDuration;
    private GenericStat lateralDragCoefficient;
    private GenericStat forwardLungeDragCoefficient;
    private GenericStat forwardForceModifer;
    private GenericStat lateralLungeEaseInFrames;
    private GenericStat lateralLungeEaseOutFrames;
    private GenericStat lateralLungeDesiredVEL;
    protected void getLungeVariables()
    {
        ArrayList vars = player.getLungeVaraiables();
        tongeContactFilter =      (ContactFilter2D)vars[0];

        minimumDistanceToSpawnANewPoint =   (float)vars[1];
        minimumTimeToSpawnANewPoint =       (float)vars[2];

        playerContactFilter =     (ContactFilter2D)vars[3];

    }
    private float v0;
    #endregion

    /*End of Tongue Variables*/
    private Transform endOfTongueTransform;

    /*Player Values*/
    private Rigidbody2D playerRB;
    
    public PlayerLungingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        playerRB = player.GetPlayerRigidBody();
        minimumLateralDuration = player.MinimumLateralDuration;
        lateralDragCoefficient = player.LateralDragCoefficient;
        forwardLungeDragCoefficient = player.ForwardLungeDragCoefficient;
        forwardForceModifer = player.ForwardLungeForceModifer;
        lateralLungeEaseInFrames = player.LateralLungeEaseInFrames;
        lateralLungeEaseOutFrames = player.LateralLungeEaseOutFrames;
        lateralLungeDesiredVEL = player.LateralLungeDesiredVEL;
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }
    /// <summary>
    /// intial radius of the swing, useful for creating stable orbits in lateral lunges. If we updated this every frame floating point errors make orbiting very unstable
    /// Only is supposed to be updated when ever we change our rotation point 
    /// </summary>
    float r0; 
    public override void EnterState()
    {
        pauseCasting = false;
        getLungeVariables(); // TODO: Remove so this is only called once, just used for updating vars
    
        ihat = player.tongueLatchedState.GetIhat(); // right
        jhat = player.tongueLatchedState.GetJhat(); // forward
        dampingCounter = 0;
        entryTime = Time.time;
        endOfTongueTransform = player.tongueStateMachine.GetEndOfTongueTransform();
        
        
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                playerRB.drag = forwardLungeDragCoefficient.Value;
                playerRB.AddForce(jhat * forwardForceModifer.Value);
                v0 = Time.fixedDeltaTime * forwardForceModifer.Value / playerRB.mass;
                break;
            case LatchMovementType.LungeLeft:
                LateralLungeIntialization(-1); // negative (-1) is -ihat which is left
                break;
            case LatchMovementType.LungeRight:
                // Create a new lateral acclerator 
                LateralLungeIntialization(1); // positive (1) is ihat which is right
                break;
            case LatchMovementType.LungeBack:
                break;
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR IN player lunging state on entry, should not be waiting");
                break;
        }
    }
    private void LateralLungeIntialization(int direction)
    {
        playerRB.drag = lateralDragCoefficient.Value;
        // Create a new lateral acclerator 
        r0 = (playerRB.position - (Vector2)endOfTongueTransform.position).magnitude;
        if (nonLinearRadialAccelerator != null)
        {
            // TODO: Make use of intialize() that doesn't use all of the params, this is just used to allow play time editing
            nonLinearRadialAccelerator.intitalize(lateralLungeEaseInFrames.Value, lateralLungeEaseOutFrames.Value, lateralLungeDesiredVEL.Value, playerRB.mass);
        }
        else
        {
            nonLinearRadialAccelerator = new NonLinearRadialAccelerator(lateralLungeEaseInFrames.Value, lateralLungeEaseOutFrames.Value, lateralLungeDesiredVEL.Value, playerRB.mass);
        }
        lungeDirection = direction; // negative is -ihat which is left, right is +1 which is +ihat which is right
        hitData = player.tongueStateMachine.GetRotationPoint();
    }
    public override void ExitState()
    {
        SetLatchMovementType(LatchMovementType.Waiting);
        lungeDirection = 1;
        pauseCasting = false;
        if (stopSwingingCoroutine != null)
        {
            player.StopCoroutine(stopSwingingCoroutine);
            activeStopSwingCoroutine = false;
        }
    }

    public override void FrameUpdate()
    {
        if (Input.GetKey(KeyCode.P))
        {
            player.tongueStateMachine.EnterOffStateImmediately(player);
            playerStateMachine.EnterOffStateImmediately(player);
            return;
        }
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
                    // allows you to cancel out of the lunge by stoping movement inputs, or by pressing f key
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
                    // allows you to cancel out of the lunge by stoping movement inputs, or by pressing f key
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
                // Check if something collides with tongue that's not supposed to
                bool hit = LineCastEndOfTongueToRotationPoint();
                if (hit)
                {
                    return;
                }
                ForwardLunge();
                break;
            case LatchMovementType.LungeLeft:
                LateralLunge();
                break;
            case LatchMovementType.LungeRight:
                LateralLunge();
                break;
            case LatchMovementType.LungeBack:
                BackwardsLunge();
                break;
            case LatchMovementType.Waiting:
                Debug.LogError("ERROR in player lunging state, state should not be waiting!");
                return;
            default:
                Debug.LogError("ERROR in player lunging state, invalid state:" + latchMovementType);
                return;
        }
    }
    public void SetLatchMovementType(LatchMovementType m)
    {
        latchMovementType = m;
    }
    Vector2 rotationPoint = Vector2.zero; // For two body hits this is just the end of the tongue
    private void LateralLunge()
    {
        /// NON LINEAR RAIDAL ACCLERATOR WAS ADDED HERE
        TwoBodySwingingLogic();
        Updateihatjhat(lungeDirection);
        nonLinearRadialAccelerator.FixedUpdateCall(ihat, jhat, playerRB, r0);

    }
    private float r1; // intial radius
    private float vt1; // intial tangental velocity
    private float dampingCounter; // how many times to apply the damping coefficient
    private float lastTimePointSpawned;

    /// <summary>
    /// Hit data tracks the most recent hit of the tongue. This only needs to be updated when a new collision occurs with the tognue.
    /// </summary>
    TongueHitData hitData;
    bool pauseCasting = false;
    private void TwoBodySwingingLogic()
    {
        if (pauseCasting) return;
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
                    Tracer.DrawCircle(col.point, 0.03f, 10, Color.red);

                    Vector2 normVector = col.normal;
                    Debug.DrawLine(col.point, col.point + normVector, Color.yellow,5f);
                    ///Debug.Break();
                    // -----------------
                    rotationPoint = hitData.getPos();
                    // Check if the collision can't be swung on
                    CantBeSwungOnCheck(col);
                    // Try to line cast from the opposite direction
                    bool collidedOnOtherSide = LineCastOppositeDirection(playerPos);// true will apply force
                }
            }
        }
    }
    public bool LineCastOppositeDirection(Vector2 playerPos)
    {
        if (pauseCasting) { return false; }
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
                    Tracer.DrawCircle(col.point, 0.03f, 10, Color.red);
                    Vector2 normVector = col.normal;
                    Debug.DrawLine(col.point, col.point + normVector, Color.yellow,5f);
                    //Debug.Break();
                    return true;
                }
            }
        }
        return false;
    }
    public override bool CantBeSwungOnCheck(RaycastHit2D col)
    {
        ICantBeSwungOn cantBeSwungInterface = col.collider.GetComponent<ICantBeSwungOn>();
        if (cantBeSwungInterface == null)
        {
            return false;
        }
        else
        {
            pauseCasting = true;
            cantBeSwungInterface.OnSwungOn(col);

            if (!activeStopSwingCoroutine)
            {
                stopSwingingCoroutine = player.StartCoroutine(DelayedStopSwing());
            }
            return true;
        }
    }
    public override void OnTongueCollisionIntersection()
    {
        playerStateMachine.ChangeState(player.slowingState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }


    bool activeStopSwingCoroutine = false;
    Coroutine stopSwingingCoroutine = null;
    private IEnumerator DelayedStopSwing()
    {
        activeStopSwingCoroutine = true;
        yield return new WaitForSeconds(0.2f);
        activeStopSwingCoroutine = false;
        TryShutOffForLateralLunge();
    }
    private void ForwardLunge()
    {
        // We need to cast the player's collider forward to check if we hit something.
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        int result = Physics2D.CircleCast(playerRB.position, 0.05f,  jhat, playerContactFilter, collisions, (Time.fixedDeltaTime * v0));
        Debug.DrawLine(playerRB.position, playerRB.position + (Time.fixedDeltaTime * v0 * jhat), Color.green);
        Tracer.DrawCircle(playerRB.position, 0.05f, 8, Color.green);
        Tracer.DrawCircle(playerRB.position + (Time.fixedDeltaTime * v0 * jhat), 0.05f, 8,Color.cyan);
        //Debug.Break();
        foreach (RaycastHit2D col in collisions) 
        {
            Collider2D collision = col.collider;
            if(collision != null)
            {
                if (collision.tag != "Player")
                {
                    Debug.Log("Collision's name in forward lunge:" + collision.name);
                    Tracer.DrawCircle(col.point, 0.03f, 8, Color.red);
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
        if (latchMovementType == LatchMovementType.LungeLeft || latchMovementType == LatchMovementType.LungeRight)
        {
            //Debug.Log("trying to shut off lateral lunge");
            if (Time.time > minimumLateralDuration.Value + entryTime)
            {
                //Debug.Log("sucess!");
                ShutOffTongueForLateral();
            }
        }
    }
    private void ShutOffTongueForLateral()
    {
        playerStateMachine.ChangeState(player.slowingState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    private void TryShutOffForForwardsLunge()
    {
        if (latchMovementType == LatchMovementType.LungeForward)
        {
            //Debug.Log("Trying to shut off forward lunge");
            playerStateMachine.ChangeState(player.slowingState);
            player.tongueStateMachine.ChangeState(player.tongueRetractingState);
        }
    }
    private void TryShutOffFOrBackwardsLunge()
    {
        if (latchMovementType == LatchMovementType.LungeBack)
        {
            //Debug.Log("Trying to shut off for backwards lunge");
            playerStateMachine.ChangeState(player.slowingState);
            player.tongueStateMachine.ChangeState(player.tongueRetractingState);
        }
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
    /// <summary>
    ///  Updates ihat and jhat. 
    ///  <br>jhat is the is direction towards the tongue. </br>
    ///  <br>ihat is the tangental direction to the last collision point of the tongue.</br>
    /// </summary>
    /// <param name="direction"> Describes which direction we are lunging. -1 for left (-ihat) +1 for right(+ihat)</param>
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

    // OLD LOGIC
    /* old linecast opposite direction
        public bool LineCastOppositeDirection(Vector2 collisionPoint, Vector2 playerPos, bool applyForce)
        {
            RaycastHit2D[] collisions = new RaycastHit2D[3];
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
    */
    /* nbody   
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
    */
    /* centripetal around one point
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
    */
    /* centripetal around new point
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
    */
    /* updateihatjhat old
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
    */
    /* TwobodyswingingLogic
    private void TwoBodySwingingLogic(Vector2 rotationPoint, Vector2 playerPos)
    {
        bool TEMP = true;
        RaycastHit2D[] collisions = new RaycastHit2D[3];
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
*/
}
