using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public float fireballSpeed = 2.5f;
    public float distance = 0.25f;
    public Transform fireball;
    

    private void Update()
    {
        fireball.position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed) * distance, Mathf.Sin(Time.time * fireballSpeed) * distance, 0);
    }
}
