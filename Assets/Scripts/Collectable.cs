using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Colliad
{
    // Logic.
    // proctected means private, but children are able to acess it
    protected bool collected;
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            OnCollect();
    }
    protected virtual void OnCollect()
    {
        collected = true;
    }

}
