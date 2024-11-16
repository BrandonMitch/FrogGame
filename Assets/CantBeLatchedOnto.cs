using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantBeLatchedOnto : MonoBehaviour, IModifyTongueBehavior
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
        return true;
    }
}
