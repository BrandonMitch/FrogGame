using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [SerializeField] private List<HeartContainer> defaultStartingHeartContainers = new List<HeartContainer>();
    [SerializeField] private List<HeartContainer> heartContainers = new List<HeartContainer>();
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private GameEvent onPlayerHealthChange;
    [SerializeField] private GameEvent onPlayerHeartContainerBreak;
    [SerializeField] private GameEvent onPlayerDamaged;
    [SerializeField] private IntegerVariable maxHealthContainers;
    const int DEFAULT_MAX_HEART_CONTAINERS = 3;
    
    private int maxHealthContainersVal = DEFAULT_MAX_HEART_CONTAINERS;
    public int MaxHealthContainersVal
    {
        get
        {
            if (maxHealthContainers != null)
            {
                maxHealthContainersVal = maxHealthContainers.value;
                return maxHealthContainersVal;
            }
            else
            {
                return maxHealthContainersVal;
            }
        }

        set
        {
            maxHealthContainersVal = value;
        }
    }

    private void Awake()
    {
        ResetHeartContainersToDefault();
    }
    bool debug2 = false; // print hearts
    [ContextMenu("Reset Heart Containers To Default")]
    private void ResetHeartContainersToDefault()
    {
        heartContainers = new List<HeartContainer>(DEFAULT_MAX_HEART_CONTAINERS);
        playerHealthSO.heartContainers = heartContainers;
        int counter = 0;
        foreach (HeartContainer heart in defaultStartingHeartContainers)
        {
            counter++;
            if (counter <= DEFAULT_MAX_HEART_CONTAINERS)
            {
                playerHealthSO.ForceAddHeartContainer(heart);
            }
        }
        playerHealthSO.heartContainers = heartContainers;
        playerHealthSO.ResetLastTimeTakenDamage();
        if (debug2) { playerHealthSO.PrintHearts(); }
    }
    #region Testing in Editor 
    [Space]
    [SerializeField] bool printDebugsForHealandDamage = false;
    [ContextMenu("Take 10 Damage")]
    public void Take10Damage()
    {
        TakeDamage(10f);
        if (printDebugsForHealandDamage)
        {
            Debug.Log("Taking 10 damage");
            PrintHearts();
        }
    }
    [SerializeField] float xDamage;
    [ContextMenu("Take x damamge")]
    public void TakeXDamage()
    {
        TakeDamage(xDamage);
        if (printDebugsForHealandDamage)
        {
            Debug.Log("Taking "+ xDamage + " damage");
            PrintHearts();
        }
    }
    [SerializeField] float xHeal;
    [ContextMenu("Heal x Health")]
    public void HealX_HP()
    {
        Heal(xDamage);
        if (printDebugsForHealandDamage)
        {
            Debug.Log("Healing " + xHeal + " HP, no overflow ");
            PrintHearts();
        }
    }

    [SerializeField] HeartContainer xHeart;
    [ContextMenu("Add X Heart Container")]
    public void AddX_Heart()
    {
        playerHealthSO.ForceAddHeartContainer(xHeart);
    }
    #endregion

    #region Using SO's Methods
    [ContextMenu("Print Hearts in SO")]
    public void PrintHearts()
    {
        playerHealthSO.PrintHearts();
    }
    public bool TakeDamage(float damage)
    {
        return playerHealthSO.TakeDamage(damage);
    }
    public void ForceTakeDamage(float damage)
    {
        playerHealthSO.ForceTakeDamage(damage);
    }

    public bool AddHeartContainer(HeartContainer heartContainer)
    {
        return playerHealthSO.AddHeartContainer(heartContainer);
    }
    private void ForceAddHeartContainer(HeartContainer heartContainer)
    {
        playerHealthSO.ForceAddHeartContainer(heartContainer);
    }
    public int GetCurrentHeartContainersCount()
    {
        return playerHealthSO.GetCurrentHeartContainersCount();
    }

    public void ChangeOrderOfHealth(int oldIndex, int newIndex)
    {
        playerHealthSO.ChangeOrderOfHealth(oldIndex, newIndex);
    }

    public void BreakHeart(int index)
    {
        playerHealthSO.BreakHeart(index);
    }
    public void BreakHeart(HeartContainer heartContainer)
    {
        playerHealthSO.BreakHeart(heartContainer);
    }

    #region Game Events
    private void OnPlayerHeartContainerBreak()
    {
        if (onPlayerHeartContainerBreak != null)
        {
            onPlayerHeartContainerBreak.Raise();
        }
    }

    private void OnPlayerDamaged()
    {
        if(onPlayerDamaged != null)
        {
            onPlayerDamaged.Raise();
        }
    }

    private void OnPlayerHealthChange()
    {
        // Raise the game event
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange.Raise();
        }
    }

    #endregion

    #region Interface Implmentation
    public int GetCurrentHealth()
    {
        return playerHealthSO.GetCurrentHealth();
    }

    public int GetMaxHealth()
    {
        return playerHealthSO.GetMaxHealth();
    }

    public void Heal(float amount)
    {
        playerHealthSO.Heal(amount);
    }
    public float HealWithOverFlow(float amount)
    {
        return playerHealthSO.HealWithOverFlow(amount);
    }
    public void HealFullHealth()
    {
        playerHealthSO.HealFullHealth();
    }

    public void OnHealthChange()
    {
        playerHealthSO.OnHealthChange();
    }

    // Just for correspondence with interface
    public void OnDamaged()
    {
        playerHealthSO.OnDamaged();
    }
    public float GetPercentHealth()
    {
        return playerHealthSO.GetPercentHealth();
    }
    #endregion
    #endregion
}
