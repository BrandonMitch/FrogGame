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
    protected bool swapSpriteDirection = false;
  

    // Filter For collider?
    public ContactFilter2D overlayContactFilter;

    // Empty Colliders 
    public bool enableOverlaySprite = false;
    private Collider2D[] overlayhits = new Collider2D[10];
    //private bool stopRunning = false;
    private bool lastState = false;


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
        if (swapSpriteDirection)
        { 
            if (moveDelta.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (moveDelta.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        
        }
        //if (pushDirection != Vector3.zero) { 
        //Debug.Log(pushDirection.ToString()); }

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

    protected virtual bool OverlayCheck()
    {
        if (enableOverlaySprite)
        {
            boxCollider.OverlapCollider(overlayContactFilter, overlayhits /*outputs a list of collisions*/);

            for (int i = 0; i < overlayhits.Length; i++)
            {
                if (overlayhits[i] == null)
                    continue;

                //The array is not cleaned up, so we do it ourself
                overlayhits[i] = null;

                return true; // we are behind something
            }
            return false;
        }
        else
        {
            return lastState;
        }
    }
}
