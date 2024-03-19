using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable_Pullable
{

    public Vector2 GetPosition();
    public Rigidbody2D GetRigidBody();
    public Transform GetTransform();
    public void OnLatchedTo(Vector3 latchLocation);
    public Vector3 GetLatchLocation();

    public void OnRetract();

    public void WhileBeingPulled();
    public void WhileBeingPushed();
    public void OnStopBeingPulled();
    public void OnStopBeingPushed();

    public bool isPullableQ();
    public bool isPushableQ();
    
}
