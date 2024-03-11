using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable_Pullable : MonoBehaviour, IPushable_Pullable
{
    protected Rigidbody2D RB;
    [Space]
    [Header("|-----Is pushable/pullable?-----|")]
    [SerializeField] protected bool isPushable;
    [SerializeField] protected bool isPullable;
    [SerializeField] protected float restingDrag;
    [SerializeField] protected float movingDrag;
    protected bool enterSwitch = true;
    protected bool slowDownEnterSwitch = true;
    protected float enterTime;
    protected float slowDownEnterTime;
    protected bool decelerateSwitch = true;
    //[SerializeField] protected float ERROR = 0.1f;
    [SerializeField] protected float slowDownTime = 1.0f;
    protected virtual void Start()
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
    }
    #region Getters
    public Vector2 GetPosition()
    {
        return RB.position;
    }

    public Rigidbody2D GetRigidBody()
    {
        return RB;
    }
    public Transform GetTransform()
    {
        return transform;
    }
    
    public bool isPullableQ()
    {
        return isPullable;
    }

    public bool isPushableQ()
    {
        return isPushable;
    }
    #endregion
    public virtual void OnLatchedTo()
    {
        RB.drag = restingDrag*10.0f;
        Debug.Log("OnLatchedTo() Not Implemented For: " + gameObject.name);
    }

    public virtual void WhileBeingPulled()
    {
        if (enterSwitch)
        {
            RB.drag = movingDrag;
            enterTime = Time.time;
            enterSwitch = false;
            slowDownEnterSwitch = true;
        }
    }

    public virtual void WhileBeingPushed()
    {
        if (enterSwitch)
        {
            RB.drag = movingDrag;
            enterTime = Time.time;
            enterSwitch = false;
            slowDownEnterSwitch = true;
        }
    }

    public virtual void OnRetract()
    {
        Debug.Log("OnRetract() Not implemented yet for " + gameObject.name);
        //throw new System.NotImplementedException();
    }

    public virtual void OnStopBeingPulled()
    {
        if (slowDownEnterSwitch)
        {
            slowDownEnterTime = Time.time;
            slowDownEnterSwitch = false;
            enterSwitch = true;
            decelerateSwitch = true;
        }
        decelerate();
    }

    public virtual void OnStopBeingPushed()
    {
        if (slowDownEnterSwitch)
        {
            slowDownEnterTime = Time.time;
            slowDownEnterSwitch = false;
            enterSwitch = true;
            decelerateSwitch = true;
        }
        decelerate();
    }
    protected virtual void decelerate()
    {
        if (!decelerateSwitch) return;
        float dT = (Time.time - slowDownEnterTime) / (slowDownTime);
        if(dT < 1)
        {
            RB.drag = Mathf.Lerp(movingDrag, restingDrag, dT);
        }
        else
        {
            RB.drag = restingDrag;
            decelerateSwitch = false;
        }
    }
}
