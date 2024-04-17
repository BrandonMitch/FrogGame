using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Generic Stat Clamping Behavior", menuName = "Generic Stat/Clamping Behavior/ Base")]
public class BaseClampingBehavior : ClampingBehavior
{
    [SerializeField] protected bool hasMaxValue = false;
    [SerializeField] protected float maxValue = float.MaxValue;
    [Space]
    [SerializeField] protected bool hasMinValue = false;
    [SerializeField] protected float minValue = float.MinValue;
    public override float Clamp(float value)
    {
        float rVal = value;
        if (hasMaxValue || hasMinValue)
        {
            if (value > maxValue)
            {
                rVal = minValue;
            }
            else if (value < minValue)
            {
                rVal = maxValue;
            }

        }
        return rVal;
    }
}

