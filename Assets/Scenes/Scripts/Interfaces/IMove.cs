using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove 
{
    public void MoveTo(Vector2 location, Request.OnFuffill onComplete = null);
    public void MoveTo(Rigidbody2D location, bool keepUpdating = false, Request.OnFuffill onComplete = null);
    public void MoveTo(Transform location, bool keepUpdating = false, Request.OnFuffill onComplete = null);
}
