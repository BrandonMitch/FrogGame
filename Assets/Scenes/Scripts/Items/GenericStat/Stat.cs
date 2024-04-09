using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Stat")]
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
    private bool registered = false;
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

    /// <returns>True if this stat locks any values to the one that is set</returns>
    public bool DoesSetValue()
    {
        return willSetValue;
    }


    public void OnDeregister()
    {
        registered = false;
    }
    public void OnRegister()
    {
        registered = true;
    }
    public bool isRegistered()
    {
        return registered;
    }
}