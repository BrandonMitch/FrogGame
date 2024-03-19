using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    // Public Fields
    public int hitpoint = 10;
    public int maxHitpoint = 10;
    public float pushRecoverySpeed = 0.2f;

    // Immunity
    protected float immuneTime = 1.0f;
    protected float lastImmune;


    // Push
    protected Vector3 pushDirection;

    // All fighters can recieve damage / die
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if(Time.time - lastImmune > immuneTime)
        {
            //Debug.Log("HIT RECIEVED");
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.zero, 1f);

            if(hitpoint <= 0)
            {
                hitpoint = 0;
                Death();
            }

        }
    }
    protected virtual void ReceiveChainLightning(GameObject chainLightningEffect)
    {
        //Debug.Log("Chain Lightning Recieved By" + gameObject.name);
        //Debug.Log("Creating New chain Lightning");
        //Instantiate(chainLightningEffect, gameObject.transform.position, Quaternion.identity);
    }
    protected virtual void Death()
    {

    }
}
