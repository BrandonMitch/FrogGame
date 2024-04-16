using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Item with Modifier",menuName ="Item/Item with Modifiers")]
public class ItemModifier : Item
{
    [Header("Stats")]
    [SerializeField] public List<Stat> stats;

    public void RegisterItem(GenericStatDictionary statDictionary, bool updateStatOnRegister = false)
    {
        foreach (Stat stat in stats)
        {
            /*            if (stat != null && statDictionary != null)
                        {
                            // register to an inventory (which is a dictionary data structure)
                            var dic = statDictionary.statDictionary;
                            var genericStat = stat.statType;
                            dic.ContainsKey(genericStat);

                            var playerStat = dic[stat.statType];

                            playerStat.RegisterStat(stat, updateStatOnRegister);
                        }*/
            GenericStat playerGenericStat = statDictionary.GetPlayerStat(stat);
            if(playerGenericStat != null)
            {
                playerGenericStat.RegisterStat(stat, updateStatOnRegister);
            }
            else
            {
                Debug.LogError("REGISTER ITEM FAILED, INVALID GENERIC STAT TYPE");
                return;
            }
        }
    }

    public override string ToString()
    {
        string s = base.ToString();
        s += "\n";
        foreach (Stat stat in stats)
        {
            s += " " + stat.ToString() + "\n";
        }
        return s;
    }
    [ContextMenu("Print Item")]
    public void printItem()
    {
        Debug.Log(this.ToString());
    }
}
