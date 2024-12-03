using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : Colliad, IDamage
{
    public bool CanDamage(IHealth thing)
    {
        return true;
    }

    public Damage Damage()
    {
        Damage d = new Damage();
        d.damageAmount = 1;
        return d;
    }

    protected override void OnCollide(Collider2D coll)
    {
        //Debug.Log("I hit this thing :" + coll.name);

        IHealth health = coll.GetComponent<IHealth>();
        if(health == null) { return; }
        if (CanDamage(health))
        {
            health.ForceTakeDamage(Damage().damageAmount);
            Debug.Log($"health ({health}) took damage");
        }
    }
}
