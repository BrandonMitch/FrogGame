using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colliad : MonoBehaviour
{
    [Header("FILTER")]
    public ContactFilter2D filter;
    protected BoxCollider2D boxCollider;
    protected Collider2D[] hits = new Collider2D[10];

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        // Collision work
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            OnCollide(hits[i]);
            //The array is not cleaned up, so we do it ourself
            hits[i] = null;

        }
    }
    protected virtual void OnCollide(Collider2D coll)
    {
            Debug.Log("OnCollide WAS NOT IMPLEMENTED IN " + this.name + "\n");
            Debug.Log("PLEASE implement this method");
    }
    /**
     * attackTypeBehavior is a method to describe specifc attack types like arc attacks, box collider attacks
     * it is meant to be overridden so that we can create specific behaviour for Arc type colliders. 
     */
    public virtual void AttackTypeBehavior(Collider2D coll)
    {
        Debug.Log("AttackTypeBehavior() has not been implemented in " + this.name);
    }
}