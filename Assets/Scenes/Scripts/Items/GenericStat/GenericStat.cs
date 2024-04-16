using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Generic Stat",menuName ="Generic Stat"), System.Serializable]
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
        else
        {
            Debug.LogWarning("Base stat is null for " + this);
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
        if (stat.statType == null /*|| stat.statType != this*/) // TODO: WE NEED TO CHECK IF stat.Statype == this.statType. But since to use a generic stat we have to clone it we need to save a reference of the type.
        {
            Debug.LogWarning("Invalid Stat Type: " + stat.statType.name);
            return false; // if the stat type is invalid, don't add the stat
        } 

        modifiers.Add(stat);
        stat.OnRegister();

        if (updateValueOnRegister)
        {
            QuickUpdateStat(stat, updateValueOnRegister);
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

    public void QuickUpdateStat(Stat stat, bool recalculateValue = false)
    {
        if (!hasSetValueApplied) // TODO: Might be a bad line of code because now we have to do a complete update value if this switches to true
        {
            addToAmount += stat.GetAddToAmount();
            multiplyAmount += stat.GetMultiplyAmount();
        }
        if (recalculateValue)
        {
            value = addToAmount * multiplyAmount;
        }
    }

    public void DeregisterStat(Stat stat)
    {
        modifiers.RemoveAll(item => item == null);
        if (stat != null)
        {
            modifiers.Remove(stat);
            addToAmount -= stat.GetAddToAmount();
            multiplyAmount -= stat.GetMultiplyAmount();
            stat.OnDeregister();
        }
        else
        {
            Debug.LogWarning("Attempting to deregister a null stat.");
        }
    }
    public static string TypeName(GenericStat stat)
    {
        if (stat == null) return "null stat";
        return stat.name;
    }
    public override string ToString()
    {
        string s = "";
        s += string.Format("[GENERIC STAT]: {0}, [VAL]: {1:F1}, [ADD]: {2:F1}, [MULT]: {3:F1}, [SETS?]: {4}", TypeName(this), value, addToAmount, multiplyAmount, hasSetValueApplied ? "True" : "False");
        if (hasSetValueApplied)
        {
            s += string.Format(", [SET]: {0:F1}", setAmount);
        }
        s += "\n";
        
        if (modifiers?.Count > 0)
        {
            s += "Modifiers:\n";
            foreach (Stat stat in modifiers)
            {
                if (stat != null)
                {
                    s += string.Format("  {0}\n", stat.ToString());
                }
            }
        }
        return s;
    }
    [ContextMenu("Print Generic Stat")]
    public void printGenericStat()
    {
        Debug.Log(this);
    }
    #region Operators (+,-,*,/,++,--, (float) cast)
    public static float operator *(GenericStat a, GenericStat b)
    {
        return a.Value * b.Value;
    }
    public static float operator +(GenericStat a, GenericStat b)
    {
        return a.Value + b.Value;
    }
    public static float operator -(GenericStat a, GenericStat b)
    {
        return a.Value - b.Value;
    }
    public static float operator /(GenericStat a, GenericStat b)
    {
        return a.Value / b.Value;
    }
    public static GenericStat operator ++(GenericStat a)
    {
        a.value++;
        return a;
    }
    public static GenericStat operator --(GenericStat a)
    {
        a.value--;
        return a;
    }
    public static explicit operator float(GenericStat stat)
    {
        return stat.Value;
    }

    /*    public static GenericStat operator *(GenericStat a, GenericStat b)
        {
            GenericStat result = ScriptableObject.CreateInstance<GenericStat>();
            result.value = a.Value * b.Value;
            return result;
        }
        public static GenericStat operator +(GenericStat a, GenericStat b)
        {
            GenericStat result = ScriptableObject.CreateInstance<GenericStat>();
            result.value = a.Value + b.Value;
            return result;
        }
        public static GenericStat operator -(GenericStat a, GenericStat b)
        {
            GenericStat result = ScriptableObject.CreateInstance<GenericStat>();
            result.value = a.Value - b.Value;
            return result;
        }
        public static GenericStat operator /(GenericStat a, GenericStat b)
        {
            GenericStat result = ScriptableObject.CreateInstance<GenericStat>();
            result.value = a.Value / b.Value;
            return result;
        }*/
    #endregion

}

