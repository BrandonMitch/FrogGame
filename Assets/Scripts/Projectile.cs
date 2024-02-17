using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Colliad
{
    // Damage Struct
    public int damagePoint = 1;
    public int pushForce = 1;

    // Projectile Characteristics
    public float projectileDuration = 30;
    public float projectileAccelation;
    public ContactFilter2D projectileContactFilter;
    public Vector3 projectileVelocity;
    public Sprite sprite;
    public bool hitsPlayer = true;

    // Needed for Movements
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    protected float spawnTime;

    protected override void Start()
    {
        base.Start();
        spawnTime = Time.time;
    }

    protected void FixedUpdate()
    {
        if (Time.time - spawnTime < projectileDuration)
        {
            UpdateProjectileMotor(projectileVelocity);
        }
        else
        {
            Debug.Log("Projectile Destroyed");
            // Destroy Projectile
        }
    }

    // CHECK THIS BEFORE RUNNING CODE
    protected override void Update()
    {
        base.Update();

    }

    // CHECK THIS BEFORE RUNNING CODE
    protected override void OnCollide(Collider2D coll)
    {

        if (coll.tag == "Fighter")
        {
            
            if (hitsPlayer)
            {
                if (coll.name == "Player")
                {
                    Debug.Log("I tink we hit da player BETTER DAMAGE");
                }

            }
            else
            {
                if (coll.name == "Player")
                {
                    Debug.Log("I tink we hit da player. Shouldn't do dmg tho");
                    return;
                }
            }
            // Create a damage object, then we'll send it to the fighter we've hit
            Damage dmg = new Damage
            {
                damageAmount = damagePoint,
                origin = transform.position,
                pushForce = pushForce,
            };

            coll.SendMessage("ReceiveDamage", dmg);
            Debug.Log(coll.name);
            // After sending damage name, Play any hit animations

            // After playing  hit animations, destroy the game object


            //Debug.Log(coll.name);

        }

    }

    protected virtual void UpdateProjectileMotor(Vector3 input)
    {
        // Reset MoveDelta
        moveDelta = projectileVelocity;

        // Make sure we can move in this direction by casting a box there first, if the box returns nul, we're free to move
        // y
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // move this thang
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
        }
        // x
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // move this thang
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
        }

    }
}