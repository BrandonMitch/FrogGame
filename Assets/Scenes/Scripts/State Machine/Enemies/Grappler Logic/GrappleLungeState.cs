using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grapple Lunge State", menuName = "State Machines/States/Grapple Lunge State")]
public class GrappleLungeState : GrapplerBaseState
{
    private NonLinearRadialAccelerator accelerator;
    [SerializeField] private int EaseInFrames;
    [SerializeField] private int EaseOutFrames;
    [SerializeField] private float DesiredLungeVelocity;
    private readonly int MASS = 1;
    bool firstGrappleFrame = true;
    float r0;
    public override void Initialize(IStateMachine<Enemy> stateMachine)
    {
        base.Initialize(stateMachine);
        accelerator = new NonLinearRadialAccelerator(EaseInFrames, EaseOutFrames, DesiredLungeVelocity, MASS);
    }
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
        stateData.grapplePointAvalible = false;
        firstGrappleFrame = true;
    }

    public override void FrameUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        // Return if we don't have state data to grapple on
        if (!stateData.grapplePointAvalible) { return;  }

        if (firstGrappleFrame)
        {
            firstGrappleFrame = false;
            r0 = Vector2.Distance(stateData.grapplePoint, RB.position);
            accelerator.intitalize();
        }

        accelerator.FixedUpdateCall(stateData.grapplePoint, RB, r0);

    }

    public override void RecieveData(Enemy.GrappleData data)
    {
        base.RecieveData(data);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        accelerator = null;
    }

    public override Enemy.GrappleData SendData()
    {
        return base.SendData();
    }
}
