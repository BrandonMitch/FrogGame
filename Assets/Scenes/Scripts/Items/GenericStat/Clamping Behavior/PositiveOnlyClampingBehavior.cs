using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PostiveOnlyClampingBehavior", menuName = "Generic Stat/Clamping Behavior/Postive Only")]
public class PositiveOnlyClampingBehavior : ClampingBehavior
{
    public override float Clamp(float value)
    {
        if (value > 0) { return value; }
        return 0;
    }
}

