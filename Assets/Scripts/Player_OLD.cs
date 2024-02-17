using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_OLD : Mover
{
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;

    [Space]
    [Header("Movement Variables")]
    // Movement Varaibles
    public float globalSpeed = 1.0f;
    public float dashSpeed = 3.0f;
    public float dashRecoverySpeed = 0.5f;
    public float dashCoolDown = 3.0f;
    public float dashDuration = 0.5f;
    private float lastDash;
    private bool dashing;
    private Vector3 lastDirection = new Vector3(0, 0, 0);
    private Vector3 dashDirection;
    [Space]
    [Header("misc")]
    public bool Unkillable = true;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate()
    {
        // negative is left, positive is right
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        // Save the last player Direction, only when moving
        if (x != 0 || y != 0)
        {
            lastDirection = new Vector3(x, y, 0);
        }

        // Update movement. Every update motor with dashing will check if we are dashing
        if (isAlive)
        {
            UpdateMotor(new Vector3(x, y, 0), dashing);

            WhenBehindSprite(OverlayCheck());
        }
    }
    private void Update()
    {

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



    }
    private void ProcessInputs()
    {
        // negative is left, positive is right
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        // Save the last player Direction, only when moving
        if (x != 0 || y != 0)
        {
            lastDirection = new Vector3(x, y, 0);
        }



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
        for (int i = 0; i < level; i++)
        {
            OnLevelUp();
        }
    }

    protected void Dash()
    {
        dashDirection = Vector3.Scale(lastDirection, new Vector3(globalSpeed * dashSpeed, globalSpeed * dashSpeed, 0));

        Debug.Log("DASH");
        // DASH!

    }

    // Overloaded UpdateMotor()
    protected virtual void UpdateMotor(Vector3 input, bool dashing)
    {
        UpdateMotor(input); // Base update motor from mover.



        if (dashing)
        {

            // Update moveDelta to the push direction 
            moveDelta = dashDirection;
            // Reduce dash speed every frame.
            dashDirection = Vector3.Lerp(dashDirection, Vector3.zero, (Time.time - lastDash) / dashDuration);

            // Make sure we can move in this direction by casting a box there first, if the box returns nul, we're free to move
            //y
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Enemy", "Blocking"));
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
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Enemy", "Blocking"));
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
        GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
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
}