using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Stat", menuName = "Stat")]
public class Stat : ScriptableObject
{
    /// <summary>
    /// This is the stat type. If it is the base stat, it should reference itself,
    /// <br>if it belongs to an item, it should reference the player base stat</br>
    /// </summary>
    [SerializeField] public GenericStat statType;
    //[SerializeField] public GenericStat genericStatInstance; // TODO: WE NEED TO MAKE AN INSTANCE OF GENERIC STAT WE ARE TRYING TO USE (conflicts with the if(statType == this) line thats why I didn't do it right away
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

    /// <returns>True if this stat locks any values to the one that is set</returns>
    public bool DoesSetValue()
    {
        return willSetValue;
    }
    /*  
        private bool registered = false;
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
    */
    public override string ToString()
    {
        string s = "";
        s += string.Format("[STAT]: {0}, [TYPE]: {1}, [ADD]: {2:F1}, [MULT]: {3:F1}, [SETS?]: {4}", this.name, GenericStat.TypeName(statType), addToAmount, multiplyAmount, willSetValue ? "True" : "False");
        if (willSetValue)
        {
            s += string.Format(", [SET]: {0:F1}", setAmount);
        }
        return s;
    }

    [ContextMenu("Print Stat")]
    public void printStat()
    {
        Debug.Log(this.ToString());
    }
}