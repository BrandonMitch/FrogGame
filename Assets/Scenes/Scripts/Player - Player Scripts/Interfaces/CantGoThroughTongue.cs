using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantGoThroughTongue : MonoBehaviour, ICantGoThroughTongue
{
    bool debug = true;
    Vector2 lastPos;

    public bool iCauseRetractOnUnLatchable(Player player)
    {
        return false;
    }

    public bool iCauseRetractOnUnSwingable(Player player)
    {
        throw new System.NotImplementedException();
    }

    public bool isLatchable(Player player)
    {
        return false;
    }

    public bool isPullable(Player player)
    {
        return false;
    }

    public bool isPushable(Player player)
    {
        return false;
    }

    public bool isSwingableOn(Player player)
    {
        return false;
    }

    //[SerializeField] float forceMultiplier = 10;
    public void OnSwungOn(RaycastHit2D hit)
    {
        OnTongueCollide(hit);
    }

    public void OnTongueCollide(RaycastHit2D hit)
    {
        Vector2 normVec = hit.normal;


        if (debug)
        {
            Debug.Log("TONGUE COLLIDED WITH BALL");
            lastPos = transform.position;
            StartCoroutine(Trace(2));
            Debug.DrawLine(hit.point, hit.point - normVec, new Color(1,165f/255f,0),1);
        }
        Rigidbody2D RB = hit.collider.attachedRigidbody;
        Vector2 vel = RB.velocity;
        float mag = vel.magnitude;
        RB.velocity = -0.5f * mag * normVec;

        //RB.AddForceAtPosition(-normVec * forceMultiplier, hit.point);
    }
    IEnumerator Trace(float duration)
    {
        float entryTime = Time.time;
        while (Time.time - entryTime < duration)
        {
            Tracer.Trace(lastPos, transform.position);
            lastPos = transform.position;
            yield return null;
        }
    }
}
