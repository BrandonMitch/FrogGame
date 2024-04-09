using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Generic Stat",menuName ="Generic Stat")]
public class GenericStat : ScriptableObject
{
    [SerializeField] private float value;
    [Space]
    [SerializeField] private float addToAmount = 0;
    [SerializeField] private float multiplyAmount = 0;
    [SerializeField] private bool hasSetValueApplied = false;
    [SerializeField] private float setAmount = 0;
    /// <summary>
    /// This is the base stat that we taken reference's from
    /// </summary>
    [SerializeField] private Stat baseStat;
    /// <summary>
    /// List of all registered stats that can modify the generic stat
    /// </summary>
    [SerializeField] private List<Stat> modifiers = new List<Stat>();
    public float Value { get => value; }



    /// <summary>
    /// This method registers new modifiers to the generic stat.
    /// </summary>
    /// <param name="stat">The stat you want to add as a modifier to the generic stat</param>
    /// <param name="updateValueOnRegister">Optional parameter that will recalculate the stat when you register it</param>
    /// <returns>True if the stat was registered, false if the stat was not registered</returns>
    public bool RegisterStat(Stat stat, bool updateValueOnRegister = false)
    {
        if(stat == null)
        {
            Debug.Log("Stat is null");
            return false;
        }
        if (stat.statType != this) 
        {
            Debug.Log("Invalid Stat Type");
            return false; // if the stat type is invalid, don't add the stat
        } 
        else
        {
            modifiers.Add(stat);
            if (updateValueOnRegister)
            {
                QuickUpdateStat(stat);
            }
            return true;
        }
    }


    /// <summary>
    /// Completely recalculates all stat values. 
    /// <br>-Should be avoided whenever possible, meant for on Awake() and when reloading the game </br>
    /// </summary>
    public void CompleteUpdateValue()
    {
        if (baseStat != null) 
        {
            addToAmount = baseStat.GetAddToAmount();
            multiplyAmount = baseStat.GetMultiplyAmount();
            setAmount = baseStat.GetSetAmount();
            hasSetValueApplied = baseStat.DoesSetValue();
        }
        else
        {
            addToAmount = 0;
            multiplyAmount = 1;
            setAmount = 0;
            hasSetValueApplied = false;
        }


        foreach (Stat stat in modifiers)
        {
            if (stat.DoesSetValue())
            {
                setAmount = stat.GetSetAmount();
                hasSetValueApplied = true;
                value = setAmount;
                return;
            }
            if(stat != null)
            {
                QuickUpdateStat(stat);
            }
            else
            {
                // Deregister the stat
                DeregisterStat(stat); // we should send a message to the stat if it's not null
            }
        }
        value = addToAmount * multiplyAmount;
    }
    public void QuickUpdateStat(Stat stat)
    {
        if (!hasSetValueApplied) // TODO: Might be a bad line of code because now we have to do a complete update value if this switches to true
        {
            addToAmount += stat.GetAddToAmount();
            multiplyAmount += stat.GetMultiplyAmount();
        }
    }
    public void DeregisterStat(Stat stat)
    {
        modifiers.Remove(stat);
        if(stat != null)
        {
            stat.OnDeregister();
        }
    }

}
[CreateAssetMenu(fileName ="Stat",menuName ="Stat")]
public class Stat : ScriptableObject
{
    /// <summary>
    /// This is the stat type. If it is the base stat, it should reference itself,
    /// <br>if it belongs to an item, it should reference the player base stat</br>
    /// </summary>
    [SerializeField] public GenericStat statType;
    [SerializeField] private float addToAmount = 0;
    [SerializeField] private float multiplyAmount;
    [SerializeField] private bool willSetValue = false;
    [SerializeField] private float setAmount = 0;
    public float GetAddToAmount()
    {
        return addToAmount;
    }
    public float GetMultiplyAmount()
    {
        return multiplyAmount;
    }
    public float GetSetAmount()
    {
        return setAmount;
    }
    public GenericStat GetStatType()
    {
        return statType;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if this stat locks any values to the one that is set</returns>
    public bool DoesSetValue()
    {
        return willSetValue;
    }
    public void OnDeregister()
    {

    }
}
