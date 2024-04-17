using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;
    [Space]
    [Header("|-----Movement Variables-----|")]
    // Movement Varaibles
    [SerializeField] private float playerSpeed = 1.0f;
    [SerializeField] private float playerMaxSpeed = 2.0f;
    [SerializeField] private float playerRestingDrag = 5.0f;
    [SerializeField] private float playerRunningDrag = 1.0f;
    [SerializeField] private float playerDragSlowDownTime = 0.2f;
    [SerializeField] private float playerRunForceModifier = 10f;
    [Space]
    [Header("|-----Lunge Variables-----|")]
    [SerializeField] private float minimumDistanceToSpawnANewPoint = 0.05f;
    [SerializeField] private float minimumTimeToSpawnANewPoint = 0.1f;
    [Space]
    [SerializeField] public ContactFilter2D tongueContactFilter;
    [SerializeField] private ContactFilter2D playerContactFilter;

    [SerializeField] private GenericStat lateralForceModifer;
    [SerializeField] private GenericStat minimumLateralDuration;
    [SerializeField] private GenericStat lateralDragCoefficient;
    [SerializeField] private GenericStat forwardLungeCoefficient;
    [SerializeField] private GenericStat forwardLungeForceModifer;
    [SerializeField] private GenericStat lateralLungeEaseInFrames;
    [SerializeField] private GenericStat lateralLungeEaseOutFrames;
    [SerializeField] private GenericStat lateralLungeDesiredVEL;
    #region Lateral Getters and setters
    public GenericStat LateralForceModifer { get => lateralForceModifer;              private set => lateralForceModifer = value; }
    public GenericStat MinimumLateralDuration { get => minimumLateralDuration;        private set => minimumLateralDuration = value; }
    public GenericStat LateralDragCoefficient { get => lateralDragCoefficient;        private set => lateralDragCoefficient = value; }
    public GenericStat ForwardLungeDragCoefficient { get => forwardLungeCoefficient;  private set => forwardLungeCoefficient = value; }
    public GenericStat ForwardLungeForceModifer { get => forwardLungeForceModifer;    private set => forwardLungeForceModifer = value; }
    public GenericStat LateralLungeEaseInFrames { get => lateralLungeEaseInFrames;    private set => lateralLungeEaseInFrames = value; }
    public GenericStat LateralLungeEaseOutFrames { get => lateralLungeEaseOutFrames;  private set => lateralLungeEaseOutFrames = value; }
    public GenericStat LateralLungeDesiredVEL { get => lateralLungeDesiredVEL;        private set => lateralLungeDesiredVEL = value; }
    #endregion

    private Rigidbody2D playerRB;
    private Collider2D playerCollider;

    // TODO: Come up with better way to do this

    [SerializeField] private Vector3 lastMoveDirection = new Vector3(0, 0, 0);
    private float xInput { get; set; }
    private float yInput { get; set; }
    private float speedForAnimation;



    [Space]
    [Header("Attack Variables")]

    [Space]
    [Header("misc")]
    public bool Unkillable = true;

    [Space]
    [Header("|-----References-----|")]
    public GenericStatDictionary statDictionary;
    public PlayerInputManager inputManager;
    public Animator animator;
    public GameObject customizableWeaponOjbect;
    private WeaponCustomizable customizableWeapon;
    [SerializeField] private CrossHairScript crossHair;
    
    public enum ExternalCall
    {
        backSlash,
        none
    }
    public ExternalCall referenceCall; 

    [Space]
    [Header("Attack Varialbes")]

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
    public PlayerAttackChargingState attackChargingState { get; set; }
    public PlayerAttackingState attackingState { get; set; }
    public PlayerComboState comboState { get; set; }
    public PlayerDeadState deadState { get; set; }

    public TongueStateMachine tongueStateMachine { get; set; }

    [SerializeField] private TongueData tongueData;
    public TongueOffState tongueOffState { get; set; }
    public TongueRetractingState tongueRetractingState { get; set; }
    public TongueAimState tongueAimState { get; set; }
    public TongueThrowState tongueThrowState { get; set; }
    public TongueLatchedState tongueLatchedState { get; set; }
    public TongueLungeState tongueLungeState { get; set; }
    public float PlayerSpeed { get => playerSpeed; set => playerSpeed = value; }
    #endregion
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // Get all instances of the generic stat
        lateralForceModifer = statDictionary.GetPlayerStatInstance(lateralForceModifer);
        minimumLateralDuration = statDictionary.GetPlayerStatInstance(minimumLateralDuration);
        lateralDragCoefficient = statDictionary.GetPlayerStatInstance(lateralDragCoefficient);
        forwardLungeCoefficient = statDictionary.GetPlayerStatInstance(forwardLungeCoefficient);
        forwardLungeForceModifer = statDictionary.GetPlayerStatInstance(forwardLungeForceModifer);

        lateralLungeEaseInFrames = statDictionary.GetPlayerStatInstance(lateralLungeEaseInFrames);
        lateralLungeEaseOutFrames = statDictionary.GetPlayerStatInstance(lateralLungeEaseOutFrames);
        lateralLungeDesiredVEL = statDictionary.GetPlayerStatInstance(lateralLungeDesiredVEL);

        stateMachine = new PlayerStateMachine();
        // Intialize all of the player states
        idleState = new PlayerIdleState(this, stateMachine);
        movingState = new PlayerMovingState(this, stateMachine);
        slowingState = new PlayerSlowingState(this, stateMachine);
        aimingTongueState = new PlayerAimingTongueState(this, stateMachine);
        throwingState = new PlayerThrowingState(this, stateMachine);
        lungingState = new PlayerLungingState(this, stateMachine);
        latchedState = new PlayerLatchedState(this, stateMachine);
        attackChargingState = new PlayerAttackChargingState(this, stateMachine);
        attackingState = new PlayerAttackingState(this, stateMachine);
        comboState = new PlayerComboState(this, stateMachine);

        tongueStateMachine = new TongueStateMachine(tongueData);
        // Intialize all of the tongue states
        tongueOffState = new TongueOffState(this, tongueStateMachine);
        tongueRetractingState = new TongueRetractingState(this, tongueStateMachine);
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
    #region ---Getter's---
    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
    public Rigidbody2D GetPlayerRigidBody()
    {
        return playerRB;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public Collider2D GetCollider()
    {
        return playerCollider;
    }
    public Vector3 GetCrossHairPosition()
    {
        return crossHair.getCrossHairPosition();
    }
    public float[] getMovementVaraibles()
    {
        float[] returnVals = {
            playerSpeed,                // 0
            playerMaxSpeed,             // 1
            playerRestingDrag,          // 2
            playerRunningDrag,          // 3
            playerDragSlowDownTime,     // 4
            playerRunForceModifier };   // 5

        return returnVals;
    }
    public ArrayList getLungeVaraiables()
    {
        ArrayList returnVals = new()
        {
            tongueContactFilter,                // 0
            minimumDistanceToSpawnANewPoint,    // 1
            minimumTimeToSpawnANewPoint,        // 2
            playerContactFilter,                // 3
        };
        return returnVals;
    }
    #endregion
    public void SetLastMoveDirection(Vector2 direction)
    {
        lastMoveDirection = direction;
    }
    public bool isTongueOff()
    {
        return tongueStateMachine.isTongueOff();
    }

    /***********************************---END---*********************************/
    protected override void Start()
    {
        base.Start();

        stateMachine.Intialize(idleState);
        tongueStateMachine.Intialize(tongueOffState);

        // This is called for getting the line render componet and certain transforms that only need to be found one time
        //TongueState[] intilizedTongueStates = { };
        //tongueStateMachine.IntializeTongueStates(intilizedTongueStates);

        customizableWeapon = customizableWeaponOjbect.GetComponent<WeaponCustomizable>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    private void FixedUpdate()
    {
        //*******STATE MACHINE******//
        stateMachine.CurrentPlayerState.PhysicsUpdate();
        tongueStateMachine.CurrentTongueState.PhysicsUpdate();
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

    }
    public void AimTongueCrossHair()
    {
        crossHair.setCrossHairState(1); // 1 corresponds to the tongue cross hair
        crossHair.setCrossHairDistance(1);

        //tongue.AimTongue(crossHair.getCrossHairPosition()); // Intialize Aiming
        tongueAimState.AimTongue(GetCrossHairPosition());
    }
    public void SpitOutTongueOnRelease()
    {
       /* if (tongue.TryThrowTongue(crossHair.getCrossHairPosition()))
        {
            //Debug.Log("Sucessfully Thrown Tongue");
        };*/
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

    private void BackSwordSwing()
    {
        // TODO: Sort out if we want to slow movement after a swing;
        animator.SetTrigger("QuickBackSlash");
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
    protected override void Death()
    {
        if (!Unkillable)
        {
            deadState = new PlayerDeadState(this, stateMachine);
            stateMachine.ChangeState(deadState);
            deadState = null;
        }
        /*
        if (!Unkillable)
        {
            isAlive = false;
            GameManager.instance.deathMenuAnim.SetTrigger("Show");
        }
        */
    }
    public void Respawn()
    {
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
        

    }
    //TODO: Add equip component
    public void EquipComponent(WeaponComponent component)
    {

    }
}
