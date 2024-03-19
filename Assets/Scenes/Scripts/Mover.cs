using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// making a class abstract makes it inheritable only
public abstract class Mover : Fighter 
{
    protected BoxCollider2D boxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    public float ySpeed = 0.75f;
    public float xSpeed = 1.0f;


    //private bool stopRunning = false;


    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Non-Player UpdateMotor
    protected virtual void UpdateMotor(Vector3 input)
    {
        // Reset MoveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0);

        // Swap Sprite Direction, Wether you're going right or left

        if (moveDelta.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (moveDelta.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        

        // Add Push Vetor, if any
        moveDelta += pushDirection;

        // Reduce push force every frame, based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        // Make sure we can move in this direction by casting a box there first, if the box returns nul, we're free to move
        // y
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking", "Enemy"));
        if (hit.collider == null)
        {
            // move this thang
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
        }
        // x
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking","Enemy"));
        if (hit.collider == null)
        {
            // move this thang
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
        }
        
    }

}
