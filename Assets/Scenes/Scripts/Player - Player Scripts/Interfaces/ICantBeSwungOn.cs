using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICantBeSwungOn : IModifyTongueBehavior
{
    public void OnSwungOn(RaycastHit2D hit);
}