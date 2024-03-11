using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable_Pullable : MonoBehaviour, IPushable_Pullable
{
    private Rigidbody2D RB;
    [Space]
    [Header("|-----Is pushable/pullable?-----|")]
    [SerializeField] private bool isPushable;
    [SerializeField] private bool isPullable;
    [SerializeField] private float restingDrag;
    [SerializeField] private float movingDrag;
    private bool enterSwitch = true;
    private bool slowDownEnterSwitch = true;
    private float enterTime;
    private float slowDownEnterTime;
    private bool decelerateSwitch = true;
    [SerializeField] private float ERROR = 0.1f;
    [SerializeField] private float slowDownTime = 1.0f;
    private void Start()
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
    public void OnLatchedTo()
    {
        RB.drag = restingDrag*10.0f;
        Debug.Log("OnLatchedTo() Not Implemented For: " + gameObject.name);
    }

    public void WhileBeingPulled()
    {
        if (enterSwitch)
        {
            RB.drag = movingDrag;
            enterTime = Time.time;
            enterSwitch = false;
            slowDownEnterSwitch = true;
        }
    }

    public void WhileBeingPushed()
    {
        if (enterSwitch)
        {
            RB.drag = movingDrag;
            enterTime = Time.time;
            enterSwitch = false;
            slowDownEnterSwitch = true;
        }
    }

    public void OnRetract()
    {
        throw new System.NotImplementedException();
    }

    public void OnStopBeingPulled()
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

    public void OnStopBeingPushed()
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
    private void decelerate()
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
