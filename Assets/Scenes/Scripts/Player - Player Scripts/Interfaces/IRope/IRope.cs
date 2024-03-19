using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRope 
{
    float GetMaxLength();
    float GetSpringCoefficient();
    void SetSpringCoefficient(float springCoefficient);
    void SetMaxLength(float maxLength);

    void SetParent(Rigidbody2D obj);
    void SetAttched(Rigidbody2D obj);
    void RemoveAttached();

}
