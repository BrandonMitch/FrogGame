using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage 
{
    public bool CanDamage(IHealth thing);
    public Damage Damage();
}
