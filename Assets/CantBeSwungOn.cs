using UnityEngine;

public class CantBeSwungOn : MonoBehaviour, ICantBeSwungOn
{
    public bool iCauseRetractOnUnLatchable(Player player)
    {
        return true;
    }

    public bool iCauseRetractOnUnSwingable(Player player)
    {
        return true;
    }

    public bool isLatchable(Player player)
    {
        return false;
    }

    public bool isPullable(Player player)
    {
        return false;
    }

    public bool isPushable(Player player)
    {
        return false;
    }

    public bool isSwingableOn(Player player)
    {
        return false;
    }

    public void OnSwungOn(RaycastHit2D hit)
    {
        Debug.Log("You can't swing on me!");
    }
}

