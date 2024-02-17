using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }

    // Damage Struct
    private int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7 };
    private float[] pushForce = { 2.0f, 2.2f, 2.5f, 2.6f, 2.7f, 2.8f, 3.0f };

    // Weapon Definition
    private Vector3 shootDirection;

    // Upgrade 
    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    // Shoots
    private Animator anim;
    /*
    private float cooldown = 0.1f;
    private float lastShot;*/

    protected void Start ()
    {
        anim = GetComponent<Animator>();
    }

}
