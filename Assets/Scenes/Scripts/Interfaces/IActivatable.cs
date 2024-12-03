using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    public void Activate();

    /// <summary>
    /// Activate activatable item or thing
    /// </summary>
    /// <param name="floats"></param>
    /// <param name="fighter">IFighter has a mono behaviour</param>
    public void Activate(Dictionary<string, float> floats = null, IFighter fighter = null);
    public Transform LocationOfSelf();

    public bool CanActivate();

    /// <summary>
    /// a dictionary with all of the required values for the spell to be cast.
    /// </summary>
    /// <returns>Key: name of the parameter, Value: always 0 </returns>
    public Dictionary<string, float> GetParameters();
}


