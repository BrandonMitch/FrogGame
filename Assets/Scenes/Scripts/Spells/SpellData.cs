using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SpellData", menuName = "Item/Spell Data")]
public class SpellData : Item
{
    [Space]
    [Header("--Spell Related--")]
    public float coolDown = 1;
    public int maxAmount = 1;

}
