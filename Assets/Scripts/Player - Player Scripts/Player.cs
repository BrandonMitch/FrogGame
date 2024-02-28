using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover, IDamageable
{
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;

    [Space]
    [Header("Movement Variables")]
    // Movement Varaibles
    public float playerSpeed = 1.0f;
    public float playerSpeedModifier = 0.0f;
    // TODO: Come up with better way to do this
    public float attackSpeedModiferTime = 2f;
    public bool attackSpeedModiferActive = false;
    public float dashSpeed = 3.0f;
    public float dashRecoverySpeed = 0.5f;
    public float dashCoolDown = 3.0f;
    public float dashDuration = 0.5f;
    private float lastDash;
    private bool dashing;
    [SerializeField] private Vector3 lastMoveDirection = new Vector3(0, 0, 0);

    private Vector3 dashDirection;
    private float xInput { get; set; }
    private float yInput { get; set; }
    private float speedForAnimation;



    [Space]
    [Header("misc")]
    public bool Unkillable = true;

    [Space]
    [Header("Refernces")]
    public Animator animator;
    public GameObject customizableWeaponOjbect;
    private WeaponCustomizable customizableWeapon;
    [SerializeField] private TongueManager tongue;
    [SerializeField] private CrossHairScript crossHair;
    
    public enum ExternalCall
    {
        backSlash,
        none
    }
    public ExternalCall referenceCall; 

    [Space]
    [Header("Attack Varialbes")]
    private float lastAttack;
    public float attackCoolDown = 1.2f;
    [Space]
    [Header("Inventory Related")]
    public Item[] inventory;


    //*** do we need these?//
    public float MaxHealth { get ; set; }
    public float CurrentHealth { get; set; }

    /***********************************Player State Machine************************************/
    /*State Machine Var's**/
    #region State Machine Var's
    public PlayerStateMachine stateMachine { get; set; }

    public PlayerIdleState idleState { get; set; }
    public PlayerMovingState movingState { get; set; }
    public PlayerSlowingState slowingState { get; set; }
    public PlayerAimingTongueState aimingTongueState { get; set; }
    public PlayerThrowingState throwingState { get; set; }
    public PlayerLungingState lungingState { get; set; }
    public PlayerLatchedState latchedState { get; set; }

 

    public TongueStateMachine tongueStateMachine { get; set; }

    [SerializeField] private TongueData tongueData;
    public TongueOffState tongueOffState { get; set; }
    public TongueAimState tongueAimState { get; set; }
    public TongueThrowState tongueThrowState { get; set; }
    public TongueLatchedState tongueLatchedState { get; set; }
    public TongueLungeState tongueLungeState { get; set; }
    #endregion
    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        // Intialize all of the player states
        idleState = new PlayerIdleState(this, stateMachine);
        movingState = new PlayerMovingState(this, stateMachine);
        slowingState = new PlayerSlowingState(this, stateMachine);
        aimingTongueState = new PlayerAimingTongueState(this, stateMachine);
        throwingState = new PlayerThrowingState(this, stateMachine);
        lungingState = new PlayerLungingState(this, stateMachine);
        latchedState = new PlayerLatchedState(this, stateMachine);


        tongueStateMachine = new TongueStateMachine(tongueData);
        // Intialize all of the tongue states
        tongueOffState = new TongueOffState(this, tongueStateMachine);
        tongueAimState = new TongueAimState(this, tongueStateMachine);
        tongueThrowState = new TongueThrowState(this, tongueStateMachine);
        tongueLatchedState = new TongueLatchedState(this, tongueStateMachine);
        tongueLungeState = new TongueLungeState(this, tongueStateMachine);
    }
    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        //TODO: Fill this in once we made player state machine, watch video to fill this part in
    }
    public enum AnimationTriggerType
    {
        PlayerDamaged,
        PlayerIdle,
    }

    // IMPLEMENT
    public void Damage(float amount)
    {
        throw new System.NotImplementedException();
    }
    //IMPLEMENT
    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void SetMovementInputs(Vector2 moveVec)
    {
        speedForAnimation = Mathf.Clamp(moveVec.magnitude, 0.0f, 1.0f);
        xInput = moveVec.x;
        yInput = moveVec.y;
    }
    public void SetMovementInputs(Vector2 moveVec, float speedForAnimation)
    {
        speedForAnimation = Mathf.Clamp(speedForAnimation, 0.0f, 1.0f);
        xInput = moveVec.x;
        yInput = moveVec.y;
    }
    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
    public void SetLastMoveDirection(Vector2 direction)
    {
        lastMoveDirection = direction;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    /***********************************---END---*********************************/
    protected override void Start()
    {
        base.Start();
        stateMachine.Intialize(idleState);
        tongueStateMachine.Intialize(tongueOffState);
        swapSpriteDirection = false;
        customizableWeapon = customizableWeaponOjbect.GetComponent<WeaponCustomizable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        //*******STATE MACHINE******//
        stateMachine.CurrentPlayerState.PhysicsUpdate();
        tongueStateMachine.CurrentTongueState.PhysicsUpdate();
        // Update movement. Every update motor with dashing will check if we are dashing
        if (isAlive)
        {
            UpdateMotor(new Vector3(xInput, yInput, 0), dashing);
        }
    }
    private void Update()
    {
        //*******STATE MACHINE******//
        stateMachine.CurrentPlayerState.FrameUpdate();
        tongueStateMachine.CurrentTongueState.FrameUpdate();

        ProcessInputs();

        Animate();
    }
    private void ProcessInputs()
    {
        // Check x/y movement, normalize vector
        // negative is left, positive is right. CLAMP MOVEMENT SPEED IF CONTROLLERS ARE A PROBLEM
        //Vector2 movVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        speedForAnimation = Mathf.Clamp(new Vector2(xInput,yInput).magnitude,0.0f,1.0f);
        
        // Save the last player Direction, only when moving. 
        if (xInput != 0 || yInput != 0)
        {
            // Save the last direction after going through the checks
            lastMoveDirection = new Vector3(xInput, yInput, 0);
        } 

        // BackSlashTest
        if (Input.GetButtonDown("Fire1"))
        {
            //Debug.Log("pressed t");
            if (Time.time - lastAttack > attackCoolDown)
            {
                //Debug.Log("swing now");
                BackSwordSwing();
            }
            else
            {
                Debug.Log("Attack on Cooldown ..." + (Time.time - lastAttack));
            }
        }
    }
    public void AimTongueCrossHair()
    {
        crossHair.setCrossHairState(1); // 1 corresponds to the tongue cross hair
        crossHair.setCrossHairDistance(1);

        //tongue.AimTongue(crossHair.getCrossHairPosition()); // Intialize Aiming
        tongueAimState.AimTongue(GetCrossHairPosition());
    }
    public Vector3 GetCrossHairPosition()
    {
        return crossHair.getCrossHairPosition();
    }
    public void SpitOutTongueOnRelease()
    {
        if (tongue.TryThrowTongue(crossHair.getCrossHairPosition()))
        {
            Debug.Log("Sucessfully Thrown Tongue");
        };
        crossHair.setCrossHairState(0); // 0 corresponds to the normal attack cross hair
        crossHair.setCrossHairDistance(); // the empty bracket resets it to its default
    }

    public void Animate()
    {
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        animator.SetFloat("Speed",speedForAnimation);
    }
    public void SwapSprite(int skinID)
    {
        // GetComponent<SpriteRenderer>().sprite = GameManager.instance.playerSprites[skinID]; not optimal
        // more optimal 
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinID];
    }
    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive) { return; }

        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitpointChange();
    }

    /*public void OnLevelUp()
    {
        maxHitpoint++;
        hitpoint = maxHitpoint;
    }
    public void SetLevel(int level)
    {
        for(int i = 0; i < level; i++)
        {
            OnLevelUp();
        }
    }*/ // TODO: REMOVE PLAYER LEVEL SYSTEM

    private void Dash()
    {
        dashDirection = Vector3.Scale(lastMoveDirection, new Vector3(playerSpeed * dashSpeed, playerSpeed * dashSpeed, 0));
        
        Debug.Log("DASH");
        // DASH!

    }
    private void BackSwordSwing()
    {
        // TODO: Sort out if we want to slow movement after a swing;
        animator.SetTrigger("QuickBackSlash");
        attackSpeedModiferActive = true;
    }

    private void externalCall() // This method is used to call external events during an animation. If we want to spawn a slash then we set the enum to the correct value and then make a new event in the attack
    {
        switch (referenceCall)
        {
            case ExternalCall.backSlash:
                //Debug.Log("Trying to run method on customizable weapon");
                customizableWeapon.BackSwordSlash();
                break;
            default:
                Debug.Log(referenceCall + " Not implmented into player");
                break;
        }
    } 
    // Overloaded UpdateMotor() to include dashing.
    protected virtual void UpdateMotor(Vector3 input, bool dashing)
    {
        swapSpriteDirection = false; // TODO: Remove this for optimization
        base.UpdateMotor(input*CalculateSpeed()); // Base update motor from mover.

        if (dashing)
        {
            // Update moveDelta to the push direction 
            moveDelta = dashDirection;
            // Reduce dash speed every frame.
            dashDirection = Vector3.Lerp(dashDirection, Vector3.zero, (Time.time-lastDash)/dashDuration);

            // Make sure we can move in this direction by casting a box there first, if the box returns nul, we're free to move
            // 
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor",/*"Enemy",*/ "Blocking"));
            if (hit.collider == null)
            {
                // move this thang
                transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
            }
            else
            {
                // If we hit an object then stop the dash
                dashDirection = Vector3.zero;
            }
            //xs
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor",/*"Enemy",*/"Blocking"));
            if (hit.collider == null)
            {
                // move this thang
                transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
            }
            else
            {
                // If we hit an object then stop the dash
                dashDirection = Vector3.zero;
            }
        }

    }
    float CalculateSpeed()
    {
        attackSpeedModiferActive = false;
        //TODO: Fix this
        float speed = Mathf.Clamp(playerSpeed * Mathf.Clamp(1.0f + playerSpeedModifier, 0, 20), 0, 20);
        return speed;
        
    }

    public void Heal(int healingAmount)
    {
        if (hitpoint == maxHitpoint)
        {
            return;
        }
        hitpoint += healingAmount;
        if (hitpoint > maxHitpoint)
        {
            hitpoint = maxHitpoint;
        }
        GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25,Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.OnHitpointChange();

    }

    protected override void Death()
    {
        if (!Unkillable)
        {
            isAlive = false;
            GameManager.instance.deathMenuAnim.SetTrigger("Show");
        }
    }
    public void Respawn()
    {
        Heal(maxHitpoint);
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
        

    }
    //TODO: Add equip component
    public void EquipComponent(WeaponComponent component)
    {

    }
}
