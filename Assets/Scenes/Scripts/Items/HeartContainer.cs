using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Heart Container", menuName ="Item/Heart Container/Heart Container")]
public class HeartContainer : Item
{
    [Space]
    [Header("Health Related")]
    [SerializeField] private float maxHealthValue;
    [HideInInspector] public float currentHealth;
    /// <summary>
    /// This property lets you survive one more hit if the incoming damage was going to break your heart
    /// </summary>
    [SerializeField] public bool HasLastStand = true; 

    [HideInInspector] public bool OnLastStand = false;
    [HideInInspector] public bool LastStandUsed = false;

    [Space]
    [Header("UI Related")]
    [SerializeField] public Color UIBackgroundFillColor;

    public float GetMaxHealthValue()
    {
        return maxHealthValue;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealthValue;
    }
    public override string ToString()
    {
        string s = this.name + ", Max HP:  " + maxHealthValue + ", Current HP: " + currentHealth;
        return s;
    }

    // TODO: make cool item where for every percent missing on each heart container you gain some stat
    public float PercentFull()
    {
        return currentHealth / maxHealthValue;
    }

    public bool isFull()
    {
        if((int)currentHealth != (int)maxHealthValue)
        {
            return false;
        }
        return true;
    }
    public bool isEmpty()
    {
        if((int)currentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public float HealWithOverFlow(float HealWithOverFlow)
    {
        // ---50/100 HP---
        // Heal 90
        // -> max heal amount = 50
        // -> overflow = 90 - 50 = 40
        // -> currentHealth (50)+= 90 - 40 = 50
        
        // Heal 20
        // -> max heal amount = 50
        // -> overflow = -30
        // -> currentHealth (50) +== 20 = 70
        // -> overflow = 0

        if (HealWithOverFlow <= 0) return 0;
        if (isFull()) { return HealWithOverFlow; } // if the heart is at max health, all the heal amount is over flowed.
        float MaxHealAmount = maxHealthValue - currentHealth;
        float overFlow = HealWithOverFlow - MaxHealAmount; // how much overflow when healing

        if (overFlow <= 0 )
        {
            currentHealth += HealWithOverFlow;
            overFlow = 0;
        }
        else
        {
            currentHealth += (HealWithOverFlow - overFlow);
        }
        return overFlow;
    }
    public void HealFull()
    {
        ResetHealth();
    }


    bool debug1 = true;
    // TODO: Add scriptable object of type method variable. This lets you drag and drop methods into scripts.
    // easy way to add thorns and freeze enemies that hit you, etc
    public float TakeDamage(float damageAmount)
    {
        if(debug1)Debug.Log("Heart container took damage");

        if (damageAmount <= 0) return 0;
        if (isEmpty()) { return damageAmount;  }

        float overflow = damageAmount-currentHealth;
        if(overflow <= 0) // if there was no overflow
        {
            currentHealth -= damageAmount;
            overflow = 0;
        }
        else  // if there was overflow 
        {
            currentHealth = 0;
        }
        return overflow;
    }
}
