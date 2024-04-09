using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
    public void ForceTakeDamage(float damage);
    public bool TakeDamage(float damage);
    public int GetCurrentHealth();
    public int GetMaxHealth();
    public float GetPercentHealth();

    /// <summary>
    /// Heals without overflow
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount);


    // TODO: This method doesn't need to be in the interface
    /// <summary>
    /// Heals With overflow and returns the value of how much it overflowed
    /// </summary>
    /// <param name="amount"></param>
    public float HealWithOverFlow(float amount);
    public void HealFullHealth();
    public void OnHealthChange();
    public void OnDamaged();
}
