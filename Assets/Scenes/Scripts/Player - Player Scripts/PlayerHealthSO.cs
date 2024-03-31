using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerHealthSO",menuName ="Item/Heart Container/Player Health Storage SO")]
public class PlayerHealthSO : ScriptableObject
{
    public List<HeartContainer> heartContainers = new List<HeartContainer>();
    [Header("------REFERENCES------")]
    [SerializeField] private IntegerVariable maxHealthContainers;
    [SerializeField] private FloatVariable Player_InvincibilityTime;
/*    [Header(
        "  Player Health Script Reference... used for updating contents of the SO\n" +
        "    WARNING: Don't set player script in editor, use method within SO"
        )]
    [SerializeField] private PlayerHealth playerHealth;*/
    [Space]
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerHealthChange;
    [SerializeField] private GameEvent onPlayerHeartContainerBreak;
    [SerializeField] private GameEvent onPlayerDamaged;

    private float lastTimeDamageTaken = 0;
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
    public void SetPlayerHealthScript(PlayerHealth playerHealthScript)
    {
        //playerHealth = playerHealthScript;
    }

    public bool TakeDamage(float damage)
    {
        /*        if (playerHealth != null)
                {
                    return playerHealth.TakeDamage(damage);
                }
                return false;*/

        if (Time.time > lastTimeDamageTaken + Player_InvincibilityTime.value) //  i frames
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
        /*        if (playerHealth != null)
                {
                    playerHealth.ForceTakeDamage(damage);
                }*/
        OnPlayerHealthChange();
        lastTimeDamageTaken = Time.time;
    }


    public bool AddHeartContainer(HeartContainer heartContainer)
    {
        /*        if (playerHealth != null)
                {
                    return playerHealth.AddHeartContainer(heartContainer);
                }
                return false;*/

        int currentAmount = GetCurrentHeartContainersCount();
        if (currentAmount >= MaxHealthContainersVal)
        {
            ForceAddHeartContainer(heartContainer);
            return true;
        }
        return false; // if we don't add hearts 
    }
    public bool ForceAddHeartContainer(HeartContainer heartContainer)
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
/*        if (playerHealth != null)
        {
            return playerHealth.GetCurrentHeartContainersCount();
        }*/
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
        //playerHealth.BreakHeart(heartContainer);
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
        if (onPlayerDamaged != null)
        {
            onPlayerDamaged.Raise();
        }
    }

    private void OnPlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange.Raise();
        }
    }
    #endregion
}
