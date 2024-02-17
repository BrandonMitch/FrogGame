using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Component")]
public class WeaponComponent : Item
{
    public enum ComponentType
    {
        Blade,
        Handle,
        CrossGuard,
        Gem,
        Complete,
    }
    public ComponentType componentType;

    public enum Collection
    {
        // Add colections
        None,
        Guppy,
    }
    public Collection collection = Collection.None;

    [Space]
    [Header("Referencing")]
    public int ID;
    public string rarity = "COMMON";
    

    [Space]
    [Header("Attack Related")]
    public int damageAmount = 1;
    public float knockBackForce = 1;
    public float AttackcoolDown = 1;
    public float critChance = 0;

    [Space]
    [Header("Blade Information")]
    public float bladeLength = 12.7f;



}
