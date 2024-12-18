using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementNameSpace;

// create teching by making no ease in frames and shutting off the speed and distance checks for lunging
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

        getLungeVariables(); // TODO: Remove so this is only called once, just used for updating vars
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
        ReachedSpeedThresholdForFlying = false;
    
        ihat = player.tongueLatchedState.GetIhat(); // right
        jhat = player.tongueLatchedState.GetJhat(); // forward
        dampingCounter = 0;
        entryTime = Time.time;
        endOfTongueTransform = player.tongueStateMachine.GetEndOfTongueTransform();

        // Sets Trigger For Forward Lunge, and sets the rotation
        player.StartCoroutine(startLungeAnimation());

        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                /*  playerRB.drag = forwardLungeDragCoefficient.Value;
                playerRB.AddForce(jhat * forwardForceModifer.Value);
                v0 = Time.fixedDeltaTime * forwardForceModifer.Value / playerRB.mass;
                hitData = player.tongueStateMachine.GetRotationPoint();*/
                ForwardLungeInialization();
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
    IEnumerator startLungeAnimation()
    {
        yield return new WaitForFixedUpdate();
        player.AnimateLunge(true);
    }
    private void ForwardLungeInialization()
    {
        playerRB.drag = forwardLungeDragCoefficient.Value;

        if (nonLinearRadialAccelerator != null)
        {
            nonLinearRadialAccelerator.intitalize(lateralLungeEaseInFrames.Value / 1.5f, lateralLungeEaseOutFrames.Value*2.0f, forwardForceModifer.Value * Time.fixedDeltaTime / playerRB.mass, playerRB.mass);
        }
        else
        {
            nonLinearRadialAccelerator = new NonLinearRadialAccelerator(lateralLungeEaseInFrames.Value / 1.5f, lateralLungeEaseOutFrames.Value / 2.0f, forwardForceModifer.Value * Time.fixedDeltaTime / playerRB.mass, playerRB.mass);
        }
        hitData = player.tongueStateMachine.GetRotationPoint();
    }
    private void LateralLungeIntialization(int direction)
    {
        playerRB.drag = lateralDragCoefficient.Value;
        // Create a new lateral acclerator 
        r0 = (playerRB.position - (Vector2)endOfTongueTransform.position).magnitude;
        if (nonLinearRadialAccelerator != null)
        {
            // TODO: Make use of intialize() that doesn't use all of the params, this is just used to allow play time editing (we just need to update the start time)
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
        // reset value



        PositionStatus posStatus = playerStateMachine.positionStatus;
        // If we are not flying, reset rotation
        if (posStatus != PositionStatus.Flying)
        {
            ReachedSpeedThresholdForFlying = false;
            ResetRotation();
            player.ResetColliderDirection();        
            // Shut off animation
            player.AnimateLunge(false);
        }

        SetLatchMovementType(LatchMovementType.Waiting);
        lungeDirection = 1;
        pauseCasting = false;
        if (stopSwingingCoroutine != null)
        {
            player.StopCoroutine(stopSwingingCoroutine);
            activeStopSwingCoroutine = false;
        }
    }
    
    /// <summary>
    /// This method will reset the player's rotation so that it is looking down
    /// </summary>
    public void ResetRotation()
    {
        player.AnimateLunge(Quaternion.Euler(0, 0, 0));
    }

    bool SUPER_CAPE = true;
    public override void FrameUpdate()
    {
        // Reading inputs differently based on lunge type.
        switch (latchMovementType)
        {
            case LatchMovementType.LungeForward:
                {
                    // allows you to cancel out of the lunge by stoping movement inputs, or by pressing f key
                    if (FKeyDown)
                    {
                        if (SUPER_CAPE && PercentMaxSpeed >= SPEED_THRESHOLD_FOR_FLYING && ReachedSpeedThresholdForFlying)
                        {
                            // if we want to fly,
                            // we must change states to flying state
                            Debug.Log("ForwardsLungeToFlying()");
                            ForwardsLungeToFlying();
                            return;
                        }
                        else
                        {
                            TryShutOffForForwardsLunge();
                        }
                    }
                    Vector2 input = GetCurrentMovementInputs();
                    if (input != Vector2.zero)
                    {
                        return;
                    }
                    TryShutOffForForwardsLunge();
                }
                return;
            case LatchMovementType.LungeLeft:
                {
                    // allows you to cancel out of the lunge by stoping movement inputs, or by pressing f key
                    if (FKeyDown)
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
                    if (FKeyDown)
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
    static readonly float SPEED_THRESHOLD_FOR_FLYING = 0.4f;
    bool ReachedSpeedThresholdForFlying = false;
    float PercentMaxSpeed { get => playerRB.velocity.magnitude / lateralLungeDesiredVEL.Value;  }
    
    public override void PhysicsUpdate()
    {
        // First apply proper lunging forces, linecasts, and update ihat jhat vectors.
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


        // Lunging speed
        float percentMaxSpeed = PercentMaxSpeed;
        
        // Animates the direction of the spitting animation, and also plays the lunge animation if we are going fast enough
        player.SetMovementInputs(jhat, percentMaxSpeed);

        // Make sure the orientation is not rotated unless we are lunging and have not already started lunging
        if(percentMaxSpeed < SPEED_THRESHOLD_FOR_FLYING && !ReachedSpeedThresholdForFlying)
        {
            ResetRotation();
        }
        else
        {
            // Apply rotations to the character if in the lunge animation
            player.AnimateLungeActual();
            player.AnimateLunge(GetCharacterRotation());
            ReachedSpeedThresholdForFlying = true;
        }
    }
    public void SetLatchMovementType(LatchMovementType m)
    {
        latchMovementType = m;
    }
    Vector2 rotationPoint = Vector2.zero; // For two body hits this is just the end of the tongue
    private void LateralLunge()
    {
        // The lines below first cast a line from the character to the end of the tongue, if there is a collision it spawns a new rotation point. 
        // Next we update new position vector after the rotation has happened last frame.
        // Finally, we apply forces to keep the character rotating with the proper amount of rotational energy. 
        TwoBodySwingingLogic();
        Updateihatjhat(lungeDirection);
        player.SetOffsetColliderDirection(-jhat);
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
    /// <summary>
    /// Two body swinging logic for the frog around a rotation point. Each frame there is a line cast and if a new collision is detected, it spawns a new rotation point. 
    /// </summary>
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

                    // Check if we can swing on the object
                    IModifyTongueBehavior tongueBehavior = collision.GetComponent<IModifyTongueBehavior>();
                    if (tongueBehavior != null && !tongueBehavior.isSwingableOn(player))
                    {
                        // OnSwungOn() if implements interface
                        ICantBeSwungOn cantBeSwungInterface = col.collider.GetComponent<ICantBeSwungOn>();
                        if (cantBeSwungInterface != null)
                        {
                            cantBeSwungInterface.OnSwungOn(col);
                        }

                        // pause casting if we cause a pause in the casting
                        if (tongueBehavior.iCauseRetractOnUnSwingable(player))
                        {
                            pauseCasting = true;
                            // if there isn't a retract on swing coroutine, start a new one
                            if (!activeStopSwingCoroutine)
                            {
                                stopSwingingCoroutine = player.StartCoroutine(DelayedStopSwing());
                            }
                        }
                        return;
                    }

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

                    // Check if we can swing on the object
                    IModifyTongueBehavior tongueBehavior = collision.GetComponent<IModifyTongueBehavior>();
                    if (tongueBehavior != null && !tongueBehavior.isSwingableOn(player))
                    {
                        // OnSwungOn() if implements interface
                        ICantBeSwungOn cantBeSwungInterface = col.collider.GetComponent<ICantBeSwungOn>();
                        if (cantBeSwungInterface != null)
                        {
                            cantBeSwungInterface.OnSwungOn(col);
                        }

                        // pause casting if we cause a pause in the casting
                        if (tongueBehavior.iCauseRetractOnUnSwingable(player))
                        {
                            pauseCasting = true;
                            // if there isn't a retract on swing coroutine, start a new one
                            if (!activeStopSwingCoroutine)
                            {
                                stopSwingingCoroutine = player.StartCoroutine(DelayedStopSwing());
                            }
                        }
                    }

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

    #region |----Forward Lunge-----|
    private void ForwardLunge()
    {
        Updateihatjhat(1);
        player.SetOffsetColliderDirection(-jhat);
        // We need to cast the player's collider forward to check if we hit something.
        RaycastHit2D[] collisions = new RaycastHit2D[3];
        int result = Physics2D.CircleCast(playerRB.position, 0.05f, jhat, playerContactFilter, collisions, (Time.fixedDeltaTime * v0));
        Debug.DrawLine(playerRB.position, playerRB.position + (Time.fixedDeltaTime * v0 * jhat), Color.green);
        Tracer.DrawCircle(playerRB.position, 0.05f, 8, Color.green);
        Tracer.DrawCircle(playerRB.position + (Time.fixedDeltaTime * v0 * jhat), 0.05f, 8, Color.cyan);
        foreach (RaycastHit2D col in collisions)
        {
            Collider2D collision = col.collider;
            if (collision != null)
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
        // Check if we should stop forward lunging
        bool shutOff = DistanceCheckForForwardLunge();
        if (shutOff) { TryShutOffForForwardsLunge(); return; }
        /*        shutOff = VelocityCheckForForwardLunge();
                if (shutOff) { TryShutOffForForwardsLunge(); return; }*/

        // Apply force
        nonLinearRadialAccelerator.FixedUpdateCall(ihat, jhat, playerRB, forcingType: NonLinearRadialAccelerator.ForcingType.Linear);

    }
    private void TryShutOffForForwardsLunge()
    {
        //Debug.Log("Trying to shut off forward lunge");
        if (latchMovementType == LatchMovementType.LungeForward)
        {
            ShutOffForForwardsLunge();
        }
    }
    private void ShutOffForForwardsLunge()
    {
        player.AnimateRetract_Reset();
        ResetRotation();
        playerStateMachine.ChangeState(player.slowingState);
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }

    private void ForwardsLungeToFlying()
    {

        // this has to be called first so we don't end up reseting rotations or animations
        playerStateMachine.positionStatus = PositionStatus.Flying;
        // Give the flying state a position so we can do distance checks to the end of our journey.
        player.flyingState.SetLastPositionOfTongue(endOfTongueTransform.position);
        // Change to flying state
        playerStateMachine.ChangeState(player.flyingState);
        
        // Retract the tounge
        player.tongueStateMachine.ChangeState(player.tongueRetractingState);
    }
    #endregion

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
        return DistanceCheckForForwardLunge(endOfTongueTransform.position);
    }
    public bool DistanceCheckForForwardLunge(Vector3 position)
    {
        Vector3 endOfTonguePos = position;
        Vector3 playerPos = playerRB.position;
        float distance = (endOfTonguePos - playerPos).magnitude;
        if (distance < 0.1f)
        {
            Debug.Log("Distance Check Failed");
            return true;
        }
        return false;
    }
    private bool VelocityCheckForForwardLunge()
    {
        float velocityMagnitude = playerRB.velocity.magnitude;
        if (velocityMagnitude < 0.1f)
        {
            Debug.Log("Velocity Check Failed");
            return true;
        }
        return false;
    }
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

    /// <summary>
    /// Gets the character rotation for lunging in a certain direction. Used for animation
    /// </summary>
    /// <returns>The rotation needed so that the character is facing towards the end of the tongue during the lunge animation</returns>
    private Quaternion GetCharacterRotation()
    {
        // jhat is a vector that is updated every frame that points from the character towards the end of the tongue.
        float angle = Mathf.Rad2Deg * Mathf.Atan2(jhat.y, jhat.x) + 90; // This calculates the angle from the downwards vector (0,-1).
                                                   // if jhat is (0,-1) -> angle = 0
                                                   // if jhat is (-1, 0) -> angle = 90
                                                   // if jhat is (0, 1) -> angle = 180
        //Debug.Log("Angle:" + angle);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log("GetCharacterRotation():" + rotation);
        return rotation;
    }
}
