using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeBase : MonoBehaviour, IRope
{
    [SerializeField] protected float springCoefficient;
    [SerializeField] protected float maxLength;
    [SerializeField] protected float currentLength;
    [SerializeField] protected float entryLength;
    protected RopeForceManager forceManager;

    public Rigidbody2D parent;
    public Rigidbody2D attachedTo;
    protected Rigidbody2D parentRB;
    protected Rigidbody2D attachedRB;
    protected SpringJoint2D spring;
    [Space]
    [SerializeField] float sFrequency = 0.1f;
    [SerializeField] float sDampening = 0.9f;
    [SerializeField] float minSpringTime = 0.3f;

    public float GetSDampenng()
    {
        return sDampening;
    }
    public float GetSFrequency() 
    {
        return sDampening;
    }

    public virtual float GetMaxLength()
    {
        return maxLength;
    }
    public virtual float GetSpringCoefficient()
    {
        return springCoefficient;
    }

    public virtual void RemoveAttached()
    {
        attachedTo = null;
    }

    public virtual void SetAttched(Rigidbody2D RB)
    {
        attachedTo = RB;
    }

    public virtual void SetMaxLength(float maxLength)
    {
        this.maxLength = maxLength;
    }

    public void SetParent(Rigidbody2D RB)
    {
        parent = RB;
    }

    public virtual void SetSpringCoefficient(float springCoefficient)
    {
        this.springCoefficient = springCoefficient;
    }

    protected virtual void InitializeRope()
    {
        forceManager = new RopeForceManager(this);
    }
    private void CalculateLength()
    {
        var obj1 = parent.transform;
        var obj2 = attachedTo.transform;
        currentLength = (obj1.position - obj2.position).magnitude;

    }
    public virtual float GetCurrentLength()
    {
        CalculateLength();
        return currentLength;
    }

    public void SetEntryLength(float entryLength)
    {
        this.entryLength = entryLength;
    }
    public SpringJoint2D AddSpringJoint()
    {
        Rigidbody2D parentRB = parent;
        Rigidbody2D attachedRB = attachedTo;
        if(parentRB == null || attachedRB == null) { Debug.LogError("NEEDS RB"); return null; }

        GameObject parentObj = parent.gameObject;
        if (parentObj.GetComponent<SpringJoint2D>() == null)
        {
            spring = parentObj.AddComponent<SpringJoint2D>();
            spring.autoConfigureDistance = false;
            spring.enableCollision = true;
            spring.connectedBody = attachedRB;
            spring.enabled = false;
            return spring;
        }
        else
        {
            return spring;
        }
    }
    public void NoHault()
    {
        forceManager.NoHault();
    }
    public void DestroySpringJoint()
    {
        if(spring != null)
        {
            Destroy(spring);
        }
    }
    public float GetMinSpringTime()
    {
        return minSpringTime;
    }

    public void WhenSpringEnabled()
    {
        forceManager.UpdateLogic();
    }

    public void EndRope()
    {
        forceManager.Hault();
        DestroySpringJoint();
        attachedRB = null;
        attachedTo = null;
    }
    protected class RopeForceManager : FlipMethod
    {
        public RopeForceManager(RopeBase rope) : base() {
            this.rope = rope;
            lengthMax = rope.GetMaxLength();
        }
        private RopeBase rope;
        private float lengthMax;
        private float lengthEntry;
        private float springCoefficent;// unused
        private float lengthCurrent;
        private SpringJoint2D spring;
        bool activeCoroutine = false;
        private float entryTime;
        bool hault = false;
        public void Hault()
        {
            hault = true;
            notEnteredYet = true;
        }
        public void NoHault()
        {
            hault = false;
        }
        /**/
        public override bool EntryCondition()
        {
            if (hault) return false;
            lengthCurrent = rope.GetCurrentLength();
            if(lengthCurrent > lengthMax)
            {
                return true;
            }
            return false;
        }
        public override void EntryLogic()
        {
            if (hault) return;
            entryTime = Time.time;
            lengthEntry = rope.GetCurrentLength();
            lengthMax = rope.GetMaxLength();
            springCoefficent = rope.GetSpringCoefficient();
            spring = rope.AddSpringJoint();
            spring.frequency = rope.GetSFrequency();
            spring.dampingRatio = rope.GetSDampenng();
            spring.distance = lengthMax;
            rope.StartCoroutine(WaitForNextPhysicsUpdate());
        }

        public override bool ExitCondition()
        {
            if (hault) return true;
            if (lengthCurrent > lengthMax) return false;
            if (Time.time < rope.GetMinSpringTime() + entryTime) return false;
            return true;
        }

        public override void ExitLogic()
        {
            if (hault) return;
            rope.DestroySpringJoint();
            base.ExitLogic();
        }

        public override void Logic()
        {
            if (hault) return;
            lengthCurrent = rope.GetCurrentLength();
            // Makes the spring stronger the further you are away from the max distnace.
            // The formula is f = frequency * (L/Lmax)^2
            float f = rope.GetSFrequency();
            spring.frequency = Mathf.Clamp(f * Mathf.Pow(lengthCurrent / lengthMax, 2), (0.5f * f), (f * 5));
        }
        IEnumerator WaitForNextPhysicsUpdate()
        {
            if (activeCoroutine == false) {
                activeCoroutine = true;
            } else
            {
                yield return null;
            }
            yield return new WaitForFixedUpdate();

            lengthCurrent = rope.GetCurrentLength();

            if (lengthCurrent >= lengthMax)
            {
                spring.enabled = true;
            }
            activeCoroutine = false;
        }
    }
}
