using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour, IFighter, IHealth
{
    public Rigidbody2D RB;

    protected virtual void Start()
    {
        
        RB = GetComponent<Rigidbody2D>();
    }
    #region Fighter Interface
    public virtual IHealth GetHealth()
    {
        return this;
    }

    public virtual bool HasHealth()
    {
        return true;
    }
    #endregion
    #region Health Interface
    public virtual void ForceTakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public virtual int GetCurrentHealth()
    {
        throw new System.NotImplementedException();
    }

    public virtual int GetMaxHealth()
    {
        throw new System.NotImplementedException();
    }

    public virtual float GetPercentHealth()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Heal(float amount)
    {
        throw new System.NotImplementedException();
    }

    public virtual float HealWithOverFlow(float amount)
    {
        throw new System.NotImplementedException();
    }

    public virtual void HealFullHealth()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnHealthChange()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDamaged()
    {
        throw new System.NotImplementedException();
    }
    #endregion


}

