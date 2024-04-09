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
    [SerializeField] private List<Stat> modifiers = new();
    public float Value { get => value; }

    /// <summary>
    /// Intializes a stat to its base stat values.
    /// </summary>
    /// <returns>True if the intialization succeded, false if it failed</returns>
    public bool IntializeStat()
    {
        if (baseStat != null)
        {
            addToAmount = baseStat.GetAddToAmount();
            multiplyAmount = baseStat.GetMultiplyAmount();
            setAmount = baseStat.GetSetAmount();
            hasSetValueApplied = baseStat.DoesSetValue();
            return true;
        }
        return false;
    }

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

        if (modifiers.Contains(stat))
        {
            Debug.LogWarning("Stat is already registered:" + stat.name);
            return false;
        }
        if (stat.statType == null||stat.statType != this) 
        {
            Debug.LogWarning("Invalid Stat Type: " + stat.statType.name);
            return false; // if the stat type is invalid, don't add the stat
        } 

        modifiers.Add(stat);
        stat.OnRegister();

        if (updateValueOnRegister)
        {
            QuickUpdateStat(stat);
        }

        return true;

    }


    /// <summary>
    /// Completely recalculates all stat values. 
    /// <br>-Should be avoided whenever possible, meant for on Awake() and when reloading the game </br>
    /// </summary>
    public void CompleteUpdateValue()
    {
        if(!IntializeStat())
        {
            addToAmount = 0;
            multiplyAmount = 1;
            setAmount = 0;
            hasSetValueApplied = false;
        }

        // List to store null stats for deregistration
        List<Stat> nullStats = new List<Stat>();

        foreach (Stat stat in modifiers)
        {
            if (stat == null)
            {
                // If stat is null, add it to the list for deregistration
                nullStats.Add(stat);
                continue; // Skip further processing for null stats
            }
            if (stat.DoesSetValue())
            {
                setAmount = stat.GetSetAmount();
                hasSetValueApplied = true;
                value = setAmount;
                return;
            }

            QuickUpdateStat(stat);
        }

        // Deregister null stats
        foreach (Stat nullStat in nullStats)
        {
            DeregisterStat(nullStat);
        }

        // Calculate value based on modifiers
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
        modifiers.RemoveAll(item => item == null);
        if (stat != null)
        {
            stat.OnDeregister();
            modifiers.Remove(stat);
            stat.OnDeregister();
        }
        else
        {
            Debug.LogWarning("Attempting to deregister a null stat.");
        }
    }

}

