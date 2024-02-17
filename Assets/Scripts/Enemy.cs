using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // Exerience
    public int xpValue = 4;

    // Logic
    public float triggerLength = 1;
    protected float chaseLength = 1.2f;
    private bool chasing;
    public bool collidingWithPlayer;
    protected Transform playerTransform;
    private Vector3 startingPosition;
    
    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        // enemies start off with no i-frames
        lastImmune = Time.time - immuneTime;
        playerTransform = GameObject.Find("Player").transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();

    }

    private void FixedUpdate()
    {
        // Is the player in range?
        if(Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength){
                chasing = true;
            }
            if (chasing)
            {
                if(!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        // Check For Overlaps
        CollidingWithPlayerCheck();
    }
    /*
     * CollidingWithPlayerCheck Is Used To Check for Player collisons. U
     */
    public void CollidingWithPlayerCheck() 
    {
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
            {
                continue;
            }

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }
            //The array is not cleaned up, so we do it ourself
            hits[i] = null;

        }
    }

    protected override void Death()
    {
        
        //Debug.Log(gameObject.name + "DEAD");
 
        
        GameManager.instance.GrantXp(xpValue);

        // Create random displacement vector for the +xp text
        Vector2 rv = Random.insideUnitCircle * 0.3f;
        GameManager.instance.ShowText("+" + xpValue.ToString() + "xp", 25, Color.magenta, transform.position + new Vector3(rv.x,rv.y), Vector3.up * 4.0f, 1f);

        Destroy(gameObject);
    }

}
