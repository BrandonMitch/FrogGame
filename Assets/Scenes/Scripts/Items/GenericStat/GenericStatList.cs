using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="GENERIC_STAT_LIST",menuName ="Item/GENERIC_STAT_LIST"), System.Serializable]
public class GenericStatList : ScriptableObject
{
    /// <summary>
    /// This list is supposed to be constant for all players. This is basically a singleton.
    /// <br>Just used in the formation of the dictionary and used as a data storage container</br>
    /// </summary>
    [SerializeField] public List<GenericStat> genericStats = new();
}
