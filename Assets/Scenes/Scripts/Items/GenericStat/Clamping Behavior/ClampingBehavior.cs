using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClampingBehavior : ScriptableObject
{
    static public float Clamp(float value, ClampingBehavior statClampingBehavior)
    {
        return statClampingBehavior.Clamp(value);
    }
    public abstract float Clamp(float value);
}
