using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : Colliad
{
    // Damage
    public int damage = 1;
    public float pushForce = 3;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            // Create a new damage object, before sending it to the player
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
        // Push back people in the way
        if (coll.tag == "Fighter" && coll.name !="Player") {
            Damage dmg = new Damage
            {
                damageAmount = 0,
                origin = transform.position,
                pushForce = pushForce
            };
        }
    }
}
