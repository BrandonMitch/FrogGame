using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanMoveTheTongue : IModifyTongueBehavior
{
    public Rigidbody2D GetRigidbody2D();
    public Vector2 GetPosition();

    /// <summary>
    /// Returns the vector from the tongue to the Rigidbody of the stick object
    /// </summary>
    /// <param name="tonguePos"></param>
    /// <returns></returns>
    public Vector2 GetDiff(Vector2 tonguePos);
}
