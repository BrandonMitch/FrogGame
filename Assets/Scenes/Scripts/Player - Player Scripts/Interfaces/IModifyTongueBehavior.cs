using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifyTongueBehavior 
{
    /// <summary>
    /// Latchable means you can latch onto and object and then lunge towards it
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool isLatchable(Player player);

    /// <summary>
    /// Pullable means you can latch onto an object and then drag it around, but cannot lunge towards it
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool isPullable(Player player);

    /// <summary>
    /// Pullable means you can push the object by hitting your tongue to it
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool isPushable(Player player);
    /// <summary>
    /// if swingable, new points will be created when swung on in a lateral lunge
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool isSwingableOn(Player player);
    public bool iCauseRetractOnUnSwingable(Player player);

    public bool iCauseRetractOnUnLatchable(Player player);

}
