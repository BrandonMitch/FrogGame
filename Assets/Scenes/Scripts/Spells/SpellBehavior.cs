using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBehavior : ScriptableObject
{

    public virtual void OnCast(Player player = null, float xPos = 0, float yPos = 0, float xDirection = 0, float yDirection = 0, float accuracy = 0)
    {

    }

    /// <summary>
    /// This method is used if a spell spawns another spell
    /// </summary>
    public virtual void OnCastLite()
    {
        OnCast();
    }
}
