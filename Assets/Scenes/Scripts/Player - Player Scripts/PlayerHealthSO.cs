using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerHealthSO",menuName ="Item/Heart Container/Player Health Storage SO")]
public class PlayerHealthSO : ScriptableObject, IHealth
{
    public List<HeartContainer> heartContainers = new List<HeartContainer>();
    [Header("------REFERENCES------")]
    [SerializeField] private IntegerVariable maxHealthContainers;
    [SerializeField] private FloatVariable Player_InvincibilityTime;

    [Space]
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerHealthChange;
    [SerializeField] private GameEvent onPlayerHeartContainerBreak;
    [SerializeField] private GameEvent onPlayerDamaged;
    [SerializeField] private GameEvent onPlayerHealed;
    [SerializeField] private GameEvent onPlayerNoHealth;

    private float lastTimeDamageTaken = 0;
    [SerializeField] const int DEFAULT_MAX_HEART_CONTAINERS = 3;

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
    [ContextMenu("Print Hearts")]
    public void PrintHearts()
    {
        string s = "\n";
        int i = 0;
        foreach (HeartContainer heart in heartContainers)
        {

            s += i + ": ";
            if (heart != null)
            {
                s += heart.ToString();
            }
            else
            {
                s += "null";
            }
            s += "\n";
            i++;
        }
        Debug.Log(s);
    }

    /// <summary>
    /// Take a float damage amount 
    /// </summary>
    /// <param name="damage"> </param>
    /// <returns>True </returns>
    public bool TakeDamage(float damage)
    {
        //Debug.Log("SO Take damage");
        //Debug.Log("time " + Time.time + ">" + lastTimeDamageTaken + Player_InvincibilityTime);
        if (Time.time > lastTimeDamageTaken + Player_InvincibilityTime.value) //  i frames
        {
            ForceTakeDamage(damage);
            return true;
        }
        return false;
    }
    public void ForceTakeDamage(float damage)
    {
        foreach (HeartContainer heart in heartContainers)
        {
            if(heart != null && !heart.isEmpty())
            {
                float overflow = heart.TakeDamage(damage);
                //Debug.Log("overflow:" + overflow);
                break;
            }
        }
        lastTimeDamageTaken = Time.time;
        OnDamaged();
    }


    public bool AddHeartContainer(HeartContainer heartContainer)
    {
        int currentAmount = GetCurrentHeartContainersCount();
        if (currentAmount >= MaxHealthContainersVal)
        {
            ForceAddHeartContainer(heartContainer);
            return true;
        }
        return false; // if we don't add hearts 
    }
    public void ForceAddHeartContainer(HeartContainer heartContainer)
    {
        HeartContainer newHeartContainer = Instantiate(heartContainer);
        newHeartContainer.ResetHealth();
        heartContainers.Insert(0,newHeartContainer);
        OnHealthChange();
    }
    public int GetCurrentHeartContainersCount()
    {

        return heartContainers.Count;
    }
    public void ChangeOrderOfHealth(int oldIndex, int newIndex)
    {
        HeartContainer temp = heartContainers[newIndex];
        heartContainers[newIndex] = heartContainers[oldIndex];
        heartContainers[oldIndex] = temp;
        OnHealthChange();
    }

    /// <summary>
    /// Helper method for Check for broken hearts
    /// </summary>
    /// <returns></returns>
    private bool ShouldHeartContainersBreak()
    {
        foreach (HeartContainer heartContainer in heartContainers)
        {
            if (heartContainer.isEmpty()) { return true; }
        }
        return false;
    }
    public List<HeartContainer> CheckForBrokenHearts()
    {
        if (!ShouldHeartContainersBreak()) { return null; }
        List<HeartContainer> brokenHearts = new List<HeartContainer>();
        foreach (HeartContainer heartContainer in heartContainers)
        {
            if (heartContainer.isEmpty())
            {
                brokenHearts.Add(heartContainer);
            }
        }
        return brokenHearts;
    }
    public void BreakHeart(int index)
    {
        HeartContainer heartContainer = heartContainers[index];
        BreakHeart(heartContainer);
    }
    public void BreakHeart(HeartContainer heartContainer)
    {
        heartContainers.Remove(heartContainer);
        Destroy(heartContainer);
        OnHeartContainerBreak();
    }


    #region Game Events ----------------------
    /// <summary>
    /// Used for calling the game Event OnPlayerHeartContainerBreak()
    /// Only shoud have one reference from OnHeartContainerBreak();
    /// </summary>
    private void OnPlayerHeartContainerBreak()
    {
        if (onPlayerHeartContainerBreak != null)
        {
            onPlayerHeartContainerBreak.Raise();
        }
    }

    /// <summary>
    /// Used for calling the Game Event.
    /// Should only have one reference from OnDamaged()
    /// </summary>
    private void OnPlayerDamaged()
    {
        if (onPlayerDamaged != null)
        {
            onPlayerDamaged.Raise();
        }
    }
    /// <summary>
    /// Used for calling the Game Event OnPlayerHealed()
    /// Should only have one reference from OnHealed()
    /// </summary>
    private void OnPlayerHealed()
    {
        if (onPlayerHealed != null)
        {
            onPlayerHealed.Raise();
        }
    }
    /// <summary>
    /// Used for calling the Game Event OnPlayerHealthChange()
    /// Should only have one reference from OnHealthChange()
    /// </summary>
    private void OnPlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange.Raise();
        }
    }
    private void OnPlayerNoHealth()
    {
        if(onPlayerNoHealth != null)
        {
            onPlayerNoHealth.Raise();
        }
    }
    #endregion -----------------

    public void OnHeartContainerBreak()
    {
        OnPlayerHeartContainerBreak();
        OnHealthChange();
    }

    #region Interface Implmentation
    public int GetCurrentHealth()
    {
        int currentHealth = 0;
        foreach (HeartContainer heartContainer in heartContainers)
        {
            currentHealth += (int)heartContainer.currentHealth;
        }
        return currentHealth;
    }
    public int GetMaxHealth()
    {
        int maxHealth = 0;
        foreach(HeartContainer heartContainer in heartContainers)
        {
            maxHealth += (int)heartContainer.GetMaxHealthValue();
        }
        return maxHealth;
    }

    public void Heal(float amount)
    {
        foreach (HeartContainer heartContainer in heartContainers)
        {
            if (!heartContainer.isFull()) // for the first heart that is not full apply the healing amount.
            {
                heartContainer.HealWithOverFlow(amount);
                break;
            }
        }
        OnHealed();
    }
    public float HealWithOverFlow(float amount)
    {
        // Will heal hearts with overflow
        foreach (HeartContainer heartContainer in heartContainers)
        {
            amount = heartContainer.HealWithOverFlow(amount);
            if (amount <= 0) return 0;
        }
        return amount;
    }

    public void HealFullHealth()
    {
        foreach (HeartContainer heartContainer in heartContainers)
        {
            heartContainer.HealFull();
        }
        OnHealed();
    }

    public void OnHealthChange()
    {
        OnPlayerHealthChange();
        List<HeartContainer> brokenHearts = CheckForBrokenHearts();
        if(brokenHearts != null)
        {
            BreakHeart(brokenHearts[0]); // Do not need to loop through because the method is recursive
        }
    }

    // Just for correspondence with interface
    public void OnDamaged()
    {
        OnPlayerDamaged();
        OnHealthChange();
    }
    // Just for correspondence with interface
    public void OnHealed()
    {
        OnPlayerHealed();
        OnHealthChange();
    }
    public float GetPercentHealth()
    {
        return  GetCurrentHealth() /(float) GetMaxHealth();
    }
    #endregion
    public void ResetLastTimeTakenDamage()
    {
        lastTimeDamageTaken = 0;
    }
}
