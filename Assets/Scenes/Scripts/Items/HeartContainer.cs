using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Heart Container", menuName ="Item/Heart Container/Heart Container")]
public class HeartContainer : Item
{
    [Space]
    [Header("Health Related")]
    [SerializeField] private float maxHealthValue;
    [HideInInspector] public float currentHealth;
    [Space]
    [Header("UI Related")]
    [SerializeField] public Color UIBackgroundFillColor;

    public float GetMaxHealthValue()
    {
        return maxHealthValue;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealthValue;
    }
    public override string ToString()
    {
        string s = this.name + ", Max HP:  " + maxHealthValue + ", Current HP: " + currentHealth;
        return s;
    }
    public float PercentFull()
    {
        return currentHealth / maxHealthValue;
    }
}
