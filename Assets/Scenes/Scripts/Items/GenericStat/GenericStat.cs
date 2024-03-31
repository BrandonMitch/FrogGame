using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class GenericStat : ScriptableObject
{
    public enum StatType
    {
        HP,
        SPD,
        ATK,
    }
    [SerializeField] private StatType statType;
    [SerializeField] private float addToAmount;
    [SerializeField] private float multiplyAmount;

    public float GetAddToAmount()
    {
        return addToAmount;
    }
    public float GetMultiplyAmount()
    {
        return multiplyAmount;
    }
    public StatType GetStatType()
    {
        return statType;
    }
}
