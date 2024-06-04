using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class GrapplerBaseState : BaseDataState<Enemy, GrappleData>
{
    protected Rigidbody2D RB;
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void Initialize(IStateMachine<Enemy> stateMachine)
    {
        base.Initialize(stateMachine);
        RB = reference.GetRigidbody();
    }

    public override void PhysicsUpdate()
    {

    }

    public override void RecieveData(GrappleData data)
    {
        base.RecieveData(data);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    public override GrappleData SendData()
    {
        return base.SendData();
    }
    /// <summary>
    /// Returns the shifted version of the next circle point
    /// </summary>
    /// <param name="degreesShifted"></param>
    /// <param name="startingAngle"></param>
    /// <param name="length"></param>
    /// <param name="grapplePoint"></param>
    /// <returns></returns>
    protected static Vector2 CalculateNextCirclePoint(float degreesShifted, float startingAngle, float length, GrappleData grapplePoint)
    {
        return CalculateNextCirclePoint(degreesShifted, startingAngle, length) + grapplePoint.grapplePoint;
    }

    /// <summary>
    /// Returns the unshifted version of the next circle point
    /// </summary>
    /// <param name="degreesShifted"></param>
    /// <param name="startingAngle"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    protected static Vector2 CalculateNextCirclePoint(float degreesShifted, float startingAngle, float length)
    {
        float newAngle = degreesShifted + startingAngle;
        return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)) * length;
    }

   /// <summary>
   /// Lets you find the direction perpendicular to your grapple point
   /// </summary>
   /// <param name="jhat">jhat is pointing towards the grapple point</param>
   /// <param name="direction">+1 for counter clockwise, -1 for clockwise</param>
   /// <returns>ihat which points towards the perpendicular direction of the grapple point</returns>
    protected static Vector2 ihat(Vector2 jhat, int direction = 1)
    {
        return direction * Vector2.Perpendicular(jhat);
    }

    protected static void DrawMarch(int depth, Vector2 p1, Vector2 p2, Color marchColor, int startingDepth = 10)
    {
        Color c = new Color(marchColor.r, marchColor.g, marchColor.b * (depth / (float)startingDepth));
        Debug.DrawLine(p1, p2, c);
    }
}
