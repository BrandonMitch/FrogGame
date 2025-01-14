using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour, IFighter
{
    private Vector2 defaultCollisionOffSet;
    private float defaultColliderOffsetMagnitude;

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
    [Header("|-----Tongue Variables-----|")]
    [SerializeField] private GenericStat tongueThrowForceModifier;
    #region Tongue Getters and Setters
    public GenericStat TongueThrowForceModifier { get => tongueThrowForceModifier;    private set => tongueThrowForceModifier = value; }
    #endregion

    private Rigidbody2D playerRB;
    private CircleCollider2D playerCollider;

    // TODO: Come up with better way to do this

    [SerializeField] private Vector3 lastMoveDirection = new Vector3(0, 0, 0);
    private float xInput { get; set; }
    private float yInput { get; set; }
    private float speedForAnimation;

    [Space]
    [Header("|-----Animation-----|")]
    private static readonly int LungeAnimation = Animator.StringToHash("Lunge_Down");
    private static readonly int LungeRetractResetAnimation = Animator.StringToHash("Retract_Reset");
    [Space]
    [Header("Attack Variables")]

    [Space]
    [Header("misc")]
    public bool Unkillable = true;

    [Space]
    [SerializeField] private float maxCrossHairDistance = 0.5f;
    [SerializeField] private float maxCrossHairDistanceDefault = 0.5f;
    [SerializeField] private float maxCrossHairAimTongueDistance = 1f;
    [Space]
    [Header("|-----References-----|")]
    [SerializeField] PlayerReferenceSO playerReferenceSO;
    private PlayerHealth playerHealth;
    public GenericStatDictionary statDictionary;
    public PlayerInventory playerInventory;
    public PlayerInputManager inputManager;
    [SerializeField] private Animator animator;
    public GameObject customizableWeaponOjbect;
    private WeaponCustomizable customizableWeapon;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform characterSpriteTransform;
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

    public PlayerFlyingState flyingState { get; set; }
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
        playerCollider = GetComponent<CircleCollider2D>();
        defaultCollisionOffSet = playerCollider.offset;
        defaultColliderOffsetMagnitude = defaultCollisionOffSet.magnitude;


        // Get all instances of the generic stat
        lateralForceModifer = statDictionary.GetPlayerStatInstance(lateralForceModifer);
        minimumLateralDuration = statDictionary.GetPlayerStatInstance(minimumLateralDuration);
        lateralDragCoefficient = statDictionary.GetPlayerStatInstance(lateralDragCoefficient);
        forwardLungeCoefficient = statDictionary.GetPlayerStatInstance(forwardLungeCoefficient);
        forwardLungeForceModifer = statDictionary.GetPlayerStatInstance(forwardLungeForceModifer);

        lateralLungeEaseInFrames = statDictionary.GetPlayerStatInstance(lateralLungeEaseInFrames);
        lateralLungeEaseOutFrames = statDictionary.GetPlayerStatInstance(lateralLungeEaseOutFrames);
        lateralLungeDesiredVEL = statDictionary.GetPlayerStatInstance(lateralLungeDesiredVEL);

        tongueThrowForceModifier = statDictionary.GetPlayerStatInstance(tongueThrowForceModifier);
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
        flyingState = new PlayerFlyingState(this, stateMachine);

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
        Retract_Reset,
        ChargeTongue,
        ThrowTongue,
        Lunge,
    }

    public void SetMovementInputs(Vector2 moveVec)
    {
        speedForAnimation = Mathf.Clamp(moveVec.magnitude, 0.0f, 1.0f);
        xInput = moveVec.x;
        yInput = moveVec.y;
    }
    public void SetMovementInputs(Vector2 moveVec, float speedForAnimation)
    {
        this.speedForAnimation = Mathf.Clamp(speedForAnimation, 0.0f, 1.0f);
        xInput = moveVec.x;
        yInput = moveVec.y;
    }
    public void SetOffsetColliderDirection(Vector2 direction)
    {
        playerCollider.offset = direction*defaultColliderOffsetMagnitude;
    }
    public void ResetColliderDirection()
    {
        playerCollider.offset = defaultCollisionOffSet;
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
    private void Start()
    {
/*        collider2D = GetComponent<CircleCollider2D>();
        defaultCollisionOffSet = collider2D.offset;*/

        stateMachine.Intialize(idleState);
        tongueStateMachine.Intialize(tongueOffState);

        // This is called for getting the line render componet and certain transforms that only need to be found one time
        //TongueState[] intilizedTongueStates = { };
        //tongueStateMachine.IntializeTongueStates(intilizedTongueStates);

        customizableWeapon = customizableWeaponOjbect.GetComponent<WeaponCustomizable>();

    }
    private void OnEnable()
    {
        playerReferenceSO.RegisterPlayer(this);
    }
    private void OnDisable()
    {
        playerReferenceSO.DeregisterPlayer(this);
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
        inputManager.FrameUpdate(); // basically checks if we want to freeze the player, allows us to freeze the state etc.
        if (!inputManager.isPaused)
        {
            stateMachine.CurrentPlayerState.FrameUpdate();
            tongueStateMachine.CurrentTongueState.FrameUpdate();

            SpellCode(ENABLE_SPELL_CODE_TEST);


            ProcessInputs();
            Animate();
        }
    }
    [SerializeField] bool ENABLE_SPELL_CODE_TEST = true;
    public void SpellCode(bool t)
    {
        if (!t) { return; }

        if (inputManager.LeftMouseUp)
        {
            // try to activate spell;
            IInventoryItem inHand = playerInventory.ItemInHand;
            IActivatable activatable = ((Object) inHand) as IActivatable;
            Debug.Log($"Inhand:{inHand}\n" +
                $"ItemInHand Type: {inHand?.GetType().Name}\n" +
                $"activatable:{activatable}\n" +
                $"canActivate:{activatable?.CanActivate()}\n");
            if (activatable == null) { return; }
            if (activatable.CanActivate()) { activatable.Activate(null, fighter: this); }
        }
    }
    private void ProcessInputs()
    {
        // Save the last player Direction, only when moving. 
        if (xInput != 0 || yInput != 0)
        {
            // Save the last direction after going through the checks
            lastMoveDirection = new Vector3(xInput, yInput, 0);
        } 

    }
    public void AnimateAim(bool active)
    {
        animator.SetBool("ChargeTongue", active);
    }
    public void AnimateThrow(bool active)
    {
        animator.SetBool("ThrowTongue", active);
    }

    public void AnimateLungeActual()
    {
        animator.CrossFade(LungeAnimation, 0, 0);
    }
    /// <summary>
    /// This doesn't actually work, we use the cross fade method to active an animation because this one sucks. All this does is change to a blend tree
    /// </summary>
    /// <param name="active"></param>
    public void AnimateLunge(bool active)
    {
        animator.SetBool("Lunge", active);
    }

    /// <summary>
    /// Can only be called in lunging state or flying state
    /// </summary>
    /// <param name="rotation"></param>
    public void AnimateLunge(Quaternion rotation)
    {
        if (stateMachine.CurrentPlayerState.Equals(lungingState) || stateMachine.CurrentPlayerState.Equals(flyingState))
        {
            headTransform.rotation = rotation;
            characterSpriteTransform.rotation = rotation;
        }
    }
    public void OnInventorySlotChanged()
    {
        // when we change selected inventory slots we need to check if it should update the crosshair
        if(playerInventory.inventory.CrossHairChangeParams != null)
        {
            crossHair.setCrossHairState(playerInventory.inventory.CrossHairChangeParams.GetCrossHairParams());
        }
        else{
            crossHair.setCrossHairState(0);
        }
    }
    public void AnimateRetract_Reset()
    {
        animator.CrossFade(LungeRetractResetAnimation, 0, 0);
        //animator.SetTrigger(LungeRetractResetAnimation);
       // animator.SetTrigger("Retract_Reset");
    }

    public void AimTongueCrossHair()
    {
        crossHair.setCrossHairState(crossHair.tongueCrossHair);
        tongueAimState.AimTongue(GetCrossHairPosition());
    }
    public void SpitOutTongueOnRelease()
    {
        if (playerInventory != null) { 

            var x = playerInventory.inventory.CrossHairChangeParams;
            if (x != null)
            {
                crossHair.setCrossHairState(x.GetCrossHairParams());
            }
            else
            {
                crossHair.setCrossHairState(0);
            }
        }
        else
        {
            crossHair.setCrossHairState(crossHair.swordCrossHair);
        }
    }

    public void Animate()
    {
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        animator.SetFloat("Speed",speedForAnimation);
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

    #region Fighter Interface Implementation
    public bool HasHealth()
    {
        if (playerHealth != null)
        {
            return true;
        }
        else
        {
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public IHealth GetHealth()
    {
        if (HasHealth())
        {
            return playerHealth;
        }
        return null;
    }

    public MonoBehaviour GetMono()
    {
        return this;
    }


    #endregion
}
