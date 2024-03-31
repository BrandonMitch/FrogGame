using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private List<HeartContainer> defaultStartingHeartContainers = new List<HeartContainer>();
    [SerializeField] private List<HeartContainer> heartContainers = new List<HeartContainer>();
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private GameEvent onPlayerHealthChange;
    [SerializeField] private GameEvent onPlayerHeartContainerBreak;
    [SerializeField] private GameEvent onPlayerDamaged;
    [SerializeField] private IntegerVariable maxHealthContainers;
    private float lastTimeDamageTaken = 0;
    private float invincibilityTime = 1;
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
        playerHealthSO.SetPlayerHealthScript(this);
        ResetHeartContainersToDefault();
    }
    bool debug2 = true; // print hearts
    [ContextMenu("Reset Heart Containers To Default")]
    private void ResetHeartContainersToDefault()
    {
        heartContainers = new List<HeartContainer>(DEFAULT_MAX_HEART_CONTAINERS);
        playerHealthSO.heartContainers = heartContainers;
        int counter = 0;
        foreach (HeartContainer heart in defaultStartingHeartContainers)
        {
            counter++;
            if(counter <= DEFAULT_MAX_HEART_CONTAINERS)
            {
                playerHealthSO.ForceAddHeartContainer(heart);
            }
        }
        playerHealthSO.heartContainers = heartContainers;
        if (debug2) { PrintHearts();  }
    }
    [ContextMenu("Print Hearts in SO")]
    public void PrintHearts()
    {
        if (playerHealthSO != null)
        {
            playerHealthSO.PrintHearts();
        }
        else
        {
            Debug.Log("PLAYER HEALTH SO IS NULL");
        }
    }
    public bool TakeDamage(float damage)
    {
        if (Time.time > lastTimeDamageTaken + invincibilityTime) // if i frames
        {
            OnPlayerDamaged();
            OnPlayerHealthChange();
            lastTimeDamageTaken = Time.time;
            return true;
        }
        return false;
    }
    public void ForceTakeDamage(float damage)
    {
        OnPlayerHealthChange();
        lastTimeDamageTaken = Time.time;
    }


    public bool AddHeartContainer(HeartContainer heartContainer)
    {
        int currentAmount = GetCurrentHeartContainersCount();
        if(currentAmount >= MaxHealthContainersVal)
        {
            ForceAddHeartContainer(heartContainer);
            return true;
        }
        return false; // if we don't add hearts 
    }

/*    bool debug1 = true;*/
    private bool ForceAddHeartContainer(HeartContainer heartContainer)
    {
        HeartContainer newHeartContainer = Instantiate(heartContainer);
/*        if (debug1) { Debug.Log(newHeartContainer); }*/
        newHeartContainer.ResetHealth();

        heartContainers.Add(newHeartContainer);
/*        if (debug1) { Debug.Log(newHeartContainer); }*/
        return true;
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
        OnPlayerHealthChange();
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
        OnPlayerHealthChange();
        OnPlayerHeartContainerBreak();
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
}
