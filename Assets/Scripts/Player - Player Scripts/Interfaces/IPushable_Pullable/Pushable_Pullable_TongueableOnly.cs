using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable_Pullable_TongueableOnly : Pushable_Pullable, IPushable_Pullable
{
    protected override void Start()
    {
        base.Start();
        RB.bodyType = RigidbodyType2D.Static; 
    }
    public override void OnLatchedTo()
    {
        RB.bodyType = RigidbodyType2D.Dynamic;
        base.OnLatchedTo();
    }

    public override void OnRetract()
    {
        RB.bodyType = RigidbodyType2D.Static;
    }

    public override void OnStopBeingPulled()
    {
        base.OnStopBeingPulled();
    }

    public override void OnStopBeingPushed()
    {
        base.OnStopBeingPushed();
    }

    public override void WhileBeingPulled()
    {
        base.WhileBeingPulled();
    }

    public override void WhileBeingPushed()
    {
        base.WhileBeingPushed();
    }

    protected override void decelerate()
    {
        base.decelerate();
    }
}
