using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Generic Stat Dictionary",menuName ="Item/Generic Stat Dictionary"), System.Serializable]
public class GenericStatDictionary : ScriptableObject
{
    [SerializeReference] public Dictionary<GenericStat, GenericStat> statDictionary;
    /// <summary>
    /// Constructions A Dictionary from a list of generic stats. Each generic stat will get its own instance, which will allow players to have thier own instances of generic stats
    /// </summary>
    /// <param name="GENERIC_STAT_LIST"></param>
    /// <param name="stats"></param>
    public void ConstructDictionary(GenericStatList GENERIC_STAT_LIST, out List<GenericStat> stats)
    {
        stats = new();
        statDictionary = new();
        foreach (GenericStat stat in GENERIC_STAT_LIST.genericStats)
        {
            // Create a new instance for each stat and add it to the dictionary
            if (stat != null)
            {
                GenericStat newInstance = Instantiate(stat);    // create instance
                statDictionary[stat] = newInstance;             // add it to the dictionary of the same generic stat type
                stats.Add(newInstance);                         // set the out variable to have these new instances
                newInstance.CompleteUpdateValue();              // update the values of the instance
            }
        }
    }
    public override string ToString()
    {
        string s = "";
        s += name + " Contents\n";
        foreach (var (key, value) in statDictionary)
        {
            s += key.name + ":\n";
            s += " " + value.ToString();
        }
        return s;
    }
    [ContextMenu("Print Contents of Dictionary")]
    public void printContents()
    {
        Debug.Log(ToString());
    }

    /// <summary>
    /// Gets the player instance of the type of stat from the provided stat
    /// <br>This is used to actually obtain a reference to the player stat instance</br>
    /// </summary>
    /// <param name="stat">Stat's generic stat type will be used to retreive the instance of the player stat</param>
    /// <returns>The instance of the player stat</returns>
    public GenericStat GetPlayerStat(Stat stat)
    {
        GenericStat genericStatType = stat.statType;
        return GetPlayerStatInstance(genericStatType);
    }

    /// <summary>
    /// Gets the player instance of the specificed genericStatType
    /// <br>This is used to actually obtain a reference to the player stat instance</br>
    /// </summary>
    /// <param name="genericStatType">The type of stat</param>
    /// <returns>The instance of the player stat</returns>
    public GenericStat GetPlayerStatInstance(GenericStat genericStatType)
    {
        if (statDictionary.ContainsKey(genericStatType))
        {
            GenericStat playerStat = statDictionary[genericStatType]; // the instance of the generic stat type
            return playerStat;
        }
        else
        {
            Debug.LogError("DICTIONARY DOES NOT CONTAIN KEY:" + genericStatType.name);
            return null;
        }
    }

    /// <summary>
    /// Gets the player instance of the specificed genericStatType and saves the value into instance
    /// </summary>
    /// <param name="genericStatType"></param>
    /// <param name="genericInstance"> Out param that you want to save the result to</param>
    /// <returns></returns>
    public bool GetPlayerStatInstance(GenericStat genericStatType, out GenericStat genericInstance)
    {
        if(genericStatType == null) {
            genericInstance = null;
            return false; 
        }
        var instance = GetPlayerStatInstance(genericStatType);
        if(instance == null)
        {
            genericInstance = null;
            return false;
        }
        genericInstance = instance;
        return true;
    }

    #region (static) Register stat to dictionary, 2 overloads
    public static void RegisterStatToDictionary(Stat stat, GenericStatDictionary genericStatDictionary, bool updateValueOnRegister = false)
    {
        RegisterStatToDictionary(stat, genericStatDictionary.statDictionary, updateValueOnRegister);
    }
    public static void RegisterStatToDictionary(Stat stat, Dictionary<GenericStat, GenericStat> statDictionary, bool updateValueOnRegister = false)
    {
        GenericStat instance = statDictionary[stat.GetStatType()];
        instance.RegisterStat(stat, updateValueOnRegister);
    }
    #endregion

    #region (static) Register Item to Dictionary, 3 overloads
    public static void RegisterItemToDictionary(ItemModifier item, GenericStatDictionary genericStatDictionary, bool updateValueOnRegister = true)
    {
        RegisterItemToDictionary(item, genericStatDictionary.statDictionary, updateValueOnRegister);
    }
    public static void RegisterItemToDictionary(ItemModifier item, Dictionary<GenericStat, GenericStat> statDictionary, bool updateValueOnRegister = true)
    {
        RegisterItemToDictionary(item.stats, statDictionary, updateValueOnRegister);
    }
    private static void RegisterItemToDictionary(List<Stat> stats, Dictionary<GenericStat, GenericStat> statDictionary, bool updateValueOnRegister = true)
    {
        foreach (Stat stat in stats)
        {
            RegisterStatToDictionary(stat, statDictionary, updateValueOnRegister);
        }
    }
    #endregion

    #region (non static) Register item to dictionary
    public void RegisterItemToDictionary(ItemModifier item, bool updateValueOnRegister = false)
    {
        GenericStatDictionary.RegisterItemToDictionary(item, this.statDictionary, updateValueOnRegister);
    }
    #endregion

}
