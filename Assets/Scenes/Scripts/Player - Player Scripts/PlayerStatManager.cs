using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class PlayerStatManager : MonoBehaviour
{
    /// <summary>
    /// This is a list of the instances of generic stats for the player
    /// </summary>
    [SerializeField] private List<GenericStat> genericStats = new(); // should be static
    /// <summary>
    /// This is a reference to all of generic stat types. We create instances of these so that each player can have a unique stat set.
    /// </summary>
    [SerializeField] private GenericStatList GENERIC_STAT_LIST;
    //[SerializeField] public Dictionary<GenericStat, GenericStat> statDictionary = new();


    /// <summary>
    /// Unique instance of the Stat dictionary SO. This is where our instances of our stats are stored
    /// </summary>
    [SerializeField] public GenericStatDictionary statDictionary;

    [SerializeField] public List<ItemModifier> items = new();
    public void Awake()
    {
        if (GENERIC_STAT_LIST != null && statDictionary != null)
        {
            statDictionary.ConstructDictionary(GENERIC_STAT_LIST, out genericStats);
        }
/*        // Iterate over the stats list
        foreach (GenericStat stat in GENERIC_STAT_LIST.genericStats)
        {
            // Create a new instance for each stat and add it to the dictionary
            if (stat != null)
            {
                var newInstance = Instantiate(stat);
                statDictionary[stat] = newInstance;
                genericStats.Add(newInstance);
                newInstance.CompleteUpdateValue();
            }
        }*/
    }
    public void RegisterItem(ItemModifier item, bool updateValueOnRegister = true)
    {
        statDictionary.RegisterItemToDictionary(item, updateValueOnRegister);
    }
    #region Debugs
    [Header("Debugs")]
    [SerializeField] ItemModifier itemX = null;
    [SerializeField] bool updateOnRegisterOfX = true;
    [SerializeField] bool debug = true;
    [ContextMenu("Add Item X")]
    private void TestAddItemX()
    {
        if (debug) {
            Debug.Log("Before Register:\n" + statDictionary.ToString());
        }
        var item = itemX;
        if(itemX == null)
        {
            item = items[0];
        }
        RegisterItem(item, updateOnRegisterOfX);
        if (debug)
        {
            Debug.Log("After:\n" + statDictionary.ToString());
        }
    }
    #endregion
}
