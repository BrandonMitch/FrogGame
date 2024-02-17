using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Colliad
{
    public enum AttacksWho{
        enemys,
        players,
        all
    }
    public AttacksWho attacksWho = AttacksWho.enemys; // Defaults to attacking enemy
    // Damage Struct
    public int[] damagePoint = { 4, 4, 5, 6, 7, 8, 9 };
    public float[] pushForce = { 3.0f, 2.2f, 2.5f, 2.6f, 2.7f, 2.8f, 3.0f };

    // Upgrade 
    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    // Swing
    protected Animator anim;
    private float cooldown = 0.1f;
    private float lastSwing;
    

    [Space]
    [Header("Chainlightning")]
    // Chainlightning
    public GameObject chainLightningEffect;
    public bool ChainLightningEnabled;
    protected override void Start()
    {
        base.Start(); // make box collider variable
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
            else
            {
                //Debug.Log("swing on cooldown ..." + (Time.time - lastSwing));
            }
            
        }
    }
    protected override void OnCollide(Collider2D coll)
    {
        switch (attacksWho)
        {
            case AttacksWho.enemys: // Attacks Enemys
                if (coll.tag == "Fighter")
                {
                    if (coll.name == "Player")
                    {
                        return;
                    }
                    // Create a damage object, then we'll send it to the fighter we've hit
                    Damage dmg = new Damage
                    {
                        damageAmount = damagePoint[weaponLevel],
                        origin = transform.position,
                        pushForce = pushForce[weaponLevel]
                    };

                    coll.SendMessage("ReceiveDamage", dmg);

                    //Debug.Log(coll.name);

                }
                break;
            case AttacksWho.players: // Attacks Players
                if (coll.name == "Player")
                {
                    // Create a damage object, then we'll send it to the fighter we've hit
                    Damage dmg = new Damage
                    {
                        damageAmount = damagePoint[weaponLevel],
                        origin = transform.position,
                        pushForce = pushForce[weaponLevel]
                    };

                    coll.SendMessage("ReceiveDamage", dmg);

                    //Debug.Log(coll.name);

                }
                break;
            case AttacksWho.all:
                {
                    // Create a damage object, then we'll send it to the fighter we've hit
                    Damage dmg = new Damage
                    {
                        damageAmount = damagePoint[weaponLevel],
                        origin = transform.position,
                        pushForce = pushForce[weaponLevel]
                    };

                    coll.SendMessage("ReceiveDamage", dmg);
                    //Debug.Log(coll.name);
                }
                break;
            default: 
                break;
        }

        /**
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
            {
                return;
            }
            // Create a damage object, then we'll send it to the fighter we've hit
            Damage dmg = new Damage
            {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            coll.SendMessage("ReceiveDamage", dmg);

            //Debug.Log(coll.name);

        }**/

    }
    public void Swing()
    {
        anim.SetTrigger("Swing"); // Or .setint or bool etc.

        if (ChainLightningEnabled)
        {
           Instantiate(chainLightningEffect, gameObject.transform.position, Quaternion.identity);
        }
    }
    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
