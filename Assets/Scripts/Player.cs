using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;

    [Space]
    [Header("Movement Variables")]
    // Movement Varaibles
    public float playerSpeed = 1.0f;
    public float playerSpeedModifier = 0.0f;
    public float attackSpeedModifer = -0.8f;
    // TODO: Come up with better way to do this
    public float attackSpeedModiferTime = 2f;
    public bool attackSpeedModiferActive = false;
    public float dashSpeed = 3.0f;
    public float dashRecoverySpeed = 0.5f;
    public float dashCoolDown = 3.0f;
    public float dashDuration = 0.5f;
    private float lastDash;
    private bool dashing;
    [HideInInspector] public Vector3 lastDirection = new Vector3(0,0,0);
    private Vector3 dashDirection;
    private float xInput;
    private float yInput;
    private float speedForAnimation;
    public float quikRunMultiplier=1.2f;
    public float quikRunToActivateTime=3f;

    float runT0 = 0;
    public bool quikRunActive = false;
    bool RunningInSameDirection = false;
    [Space]
    [Header("misc")]
    public bool Unkillable = true;

    [Space]
    [Header("Refernces")]
    public Animator animator;
    public GameObject customizableWeaponOjbect;
    private WeaponCustomizable customizableWeapon;
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

    protected override void Start()
    {
        base.Start();
        swapSpriteDirection = false;
        customizableWeapon = customizableWeaponOjbect.GetComponent<WeaponCustomizable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        // Update movement. Every update motor with dashing will check if we are dashing
        if (isAlive)
        {
            UpdateMotor(new Vector3(xInput, yInput, 0), dashing);

            WhenBehindSprite(OverlayCheck());
        }
    }
    private void Update()
    {
        ProcessInputs();

        Animate();
    }
    private void ProcessInputs()
    {
        // Check x/y movement, normalize vector
        // negative is left, positive is right. CLAMP MOVEMENT SPEED IF CONTROLLERS ARE A PROBLEM
        Vector2 movVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        speedForAnimation = Mathf.Clamp(movVec.magnitude,0.0f,1.0f);
        xInput = movVec.x;
        yInput = movVec.y;

        
        // Save the last player Direction, only when moving. 
        if (xInput != 0 || yInput != 0)
        {
            // Check if we are running in the same direction 
            
            if (lastDirection.x == xInput && lastDirection.y == yInput)
            {
                if (!RunningInSameDirection)  // If it is the first time, then record the time.
                {
                    runT0 = Time.time;
                }
                RunningInSameDirection = true;
            }
            else
            {
                RunningInSameDirection = false;
                quikRunActive = false;
            }
            // if the time to activate has passsed and you are still running in the same direction, then increasse player speed.
            if (RunningInSameDirection && (Time.time - runT0 > quikRunToActivateTime))
            {
                speedForAnimation = 2.0f; // increasing animation speed will automatically play the quik run animation
                quikRunActive = true;
            }

            // Save the last direction after going through the checks
            lastDirection = new Vector3(xInput, yInput, 0);
        } 
        else if (quikRunActive) // if the input vector is zero, disable quik running
        {
            RunningInSameDirection = false;
            quikRunActive = false;
        }

        // Check for dash, then dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time - lastDash > dashCoolDown)
            {
                lastDash = Time.time;
                dashing = true;
                Dash();
            }
            else
            {
                Debug.Log("DASH on cooldown ..." + (Time.time - lastDash));
            }
        }
        if (dashing && Time.time - lastDash >= dashDuration)
        {
            dashing = false; // Set Dashing to false when the dash duration has elapsed.
        }
        // BackSlashTest
        if (Input.GetKeyDown(KeyCode.T))
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
    private void Animate()
    {
        animator.SetFloat("Horizontal", lastDirection.x);
        animator.SetFloat("Vertical", lastDirection.y);
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

    public void OnLevelUp()
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
    }

    private void Dash()
    {
        dashDirection = Vector3.Scale(lastDirection, new Vector3(playerSpeed * dashSpeed, playerSpeed * dashSpeed, 0));
        
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
                Debug.Log("Trying to run method on customizable weapon");
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
        UpdateMotor(input*CalculateSpeed()); // Base update motor from mover.

        
        
        if (dashing)
        {

            // Update moveDelta to the push direction 
            moveDelta = dashDirection;
            // Reduce dash speed every frame.
            dashDirection = Vector3.Lerp(dashDirection, Vector3.zero, (Time.time-lastDash)/dashDuration);

            // Make sure we can move in this direction by casting a box there first, if the box returns nul, we're free to move
            //y
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
        // TODO: Fixx this, very inefficient
        if(Time.time - lastAttack < 2.0f)
        {
            attackSpeedModiferActive = true;
        }
        else
        {
            attackSpeedModiferActive = false;
        }
        //TODO: Fix this
        float speed = Mathf.Clamp(playerSpeed * Mathf.Clamp(1.0f + playerSpeedModifier,0,20),0,20)*(1 + attackSpeedModifer*(attackSpeedModiferActive ? 1 : 0));
        if (quikRunActive)
        {
            speed *= quikRunMultiplier;
        }
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