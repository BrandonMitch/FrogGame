using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFighter
{
    public bool HasHealth();
    public IHealth GetHealth();
}
