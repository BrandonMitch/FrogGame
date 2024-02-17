using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcAttack : Colliad
{
    protected CircleCollider2D circleCollider;
    // TODO: Should this be a weapon or weaponCustomizable
    private WeaponCustomizable parentWeapon;

    // Start is called before the first frame update
    protected override void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();  // make circle collider variable
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (circleCollider.enabled)
        {
            circleCollider.OverlapCollider(filter, hits);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                    continue;

                OnCollide(hits[i]);
                //The array is not cleaned up, so we do it ourself
                hits[i] = null;
            }
        }
    }
    public void setWeapon(WeaponCustomizable parentWeapon)
    {
        this.parentWeapon = parentWeapon;
    }
    
    protected override void OnCollide(Collider2D coll)
    {
        // USE PARENT's BASE ATTACK BEHAVIOR
        parentWeapon.filterAttacks(coll, this);
    }
    public override void AttackTypeBehavior(Collider2D coll)
    {
        /** Basically we draw two vectors basis 1 and two. 
        *   Then we rotate them towards the rotation of the slash
        *   Then we find the vector which  is inbetween both of them (reference vector)
        *   Then calculate the angle from the ref vector to a basis
        *   Then we check the angle from the collided to the reference vector
        *   if this angle is smaller than the basis, then it must lie in the boundary
        *   if this is true, we send an attack, otherwise the attack missed.
        */  
        // Angled Normalized Vectors
        Vector3 basis = new Vector3(0.984808f, 0.173648f);
        Vector3 basis2 = new Vector3(-1f, 1f);
        float rotAngle = transform.localEulerAngles.z;
        Debug.Log(rotAngle);
        basis = Quaternion.Euler(0, 0, rotAngle) * basis;
        Debug.Log(Quaternion.Euler(0, 0, rotAngle));
        basis2 = Quaternion.Euler(0, 0, rotAngle) * basis2;

        Vector3 referenceVector = (basis+basis2).normalized;
        
        Vector3 directionToColl = coll.transform.position - transform.position;
        float angleRange = Vector3.Angle(referenceVector, basis);

        float angleToOther = Vector3.Angle(referenceVector, directionToColl);
        Debug.Log("angle to other is:" + angleToOther);

        if (angleToOther <= angleRange)
        {
            Debug.Log("object within range" + coll.name);

            // Send Damage Packet
            Damage dmg = new Damage
            {
                damageAmount = parentWeapon.damagePoint[parentWeapon.weaponLevel],
                origin = transform.position,
                pushForce = parentWeapon.pushForce[parentWeapon.weaponLevel]
            };
            parentWeapon.SendDamage(coll, dmg);
        }

        Debug.DrawLine(transform.position, transform.position + 10 * referenceVector, Color.green, 5f);
        Debug.DrawLine(transform.position, transform.position + 10 * basis, Color.blue, 5f);
        Debug.DrawLine(transform.position, transform.position + 10 * basis2, Color.red, 5f);
    }
}
