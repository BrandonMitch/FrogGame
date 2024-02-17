using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Swing_Attack : Enemy
{
    [SerializeField]
    private float attackDistanceThreshold = 0.2f;

    [SerializeField]
    private float attackDelay = 1f;
    private float passedTime = 1f;

    public SpinScript weaponNode;
    public Weapon weapon;

    protected override void Start()
    {
        base.Start();
    }
    private void FixedUpdate()
    {
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        if (distance < chaseLength)
        {
            // Point weapon at playerTransform.positon
            if (weaponNode != null) {
                if (weaponNode.lookAtDefault)
                {
                    weaponNode.lookAtDefault = false;
                }
                weaponNode.lookat = playerTransform.position;
                if (distance <= attackDistanceThreshold)
                {
                    // Attack Behaviour
                    if(passedTime >= attackDelay)
                    {
                        passedTime = 0;
                        weapon.Swing();
                    }
                }
                else if (!collidingWithPlayer)
                {
                    // Chasing The Player, if not Colliding.
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }

             }


        }
        else
        {
            if(weaponNode != null)
            {
                if (!weaponNode.lookAtDefault)
                {
                    weaponNode.lookAtDefault = true;
                }
            }
        }

        // Check For Overlaps
        CollidingWithPlayerCheck();
    }
    protected override void Death()
    { 
        base.Death();
    }
}
