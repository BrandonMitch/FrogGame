using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningScript : MonoBehaviour
{
    private CircleCollider2D coll;
    
    public LayerMask enemyLayer;
    public int damagePoint;
    public GameObject chainLightningEffect;
    public GameObject beenStruck;

    public int amountToChain;

    private GameObject startObject;
    public GameObject endObject;

    private Animator ani;

    public ParticleSystem parti;

    private int singleSpawns;
    void Start()
    {
        if (amountToChain == 0) 
        {
            Debug.Log("Nothing to chain  to");
            Destroy(gameObject); 
        }
        coll = GetComponent<CircleCollider2D>();

        ani = GetComponent<Animator>();

        parti = GetComponent<ParticleSystem>();

        startObject = gameObject;

        singleSpawns = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See if the child of the prefab of the collision has that script
        // Make sure every thing is on the enemy layer

        Debug.Log("Collision Check. Here is the Collider2D Found:" + collision.name);
        if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)) && !collision.GetComponentInChildren<EnemyStruck>())
        {
            if (singleSpawns != 0)
            {
                endObject = collision.gameObject;

                amountToChain--;
                Instantiate(chainLightningEffect, collision.gameObject.transform.position, Quaternion.identity);
                Instantiate(beenStruck, collision.gameObject.transform);

                // Send Damage to the Collision Object
                Damage dmg = new Damage
                {
                    damageAmount = damagePoint,
                    origin = transform.position,
                    pushForce = 1
                };
                Debug.Log("sending damage");
                collision.SendMessage("ReceiveDamage", dmg);
                Debug.Log("damage sent");
                collision.SendMessage("ReceiveChainLightning", chainLightningEffect);
                // Stop animation
                ani.StopPlayback();
                // Disable collider
                coll.enabled = false;
                singleSpawns--;
                // Play particle effect
                parti.Play();
                
                //EMIT FIRST PARTICLE
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = startObject.transform.position;
                parti.Emit(emitParams, 1);

                // EMIT SECOND
                emitParams.position = endObject.transform.position;
                parti.Emit(emitParams, 1);

                emitParams.position = (startObject.transform.position + endObject.transform.position)/2;
                parti.Emit(emitParams, 1);

                Destroy(gameObject, 1f);
            }
        }
    }
}
