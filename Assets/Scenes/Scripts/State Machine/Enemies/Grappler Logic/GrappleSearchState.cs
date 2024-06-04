using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;
[CreateAssetMenu(fileName = "Grapple Search State", menuName ="State Machines/States/Grapple/Grapple Search State")]
public class GrappleSearchState : GrapplerBaseState
{
    #region Parameters
    [Header("Marching Parameters")]
    [SerializeField] float marchDistance = 0.1f;
    [SerializeField] Color marchColor = new Color(0f, 0.2f, 1f);
    [Tooltip("Determines how many marches will be preformed (The total is 2x this since it is done on both sides")]
    [SerializeField] int startingDepth = 10;
    [Header("Circle Cast Parameters")]
    [SerializeField,Range(0,90)] float minimimumAngleForBoxCheck = 45f;
    [SerializeField,Range(20,270)] float angularCastLimit = 180f;
    [SerializeField] float boundingBoxSize = 1.0f;
    [SerializeField, Range(3, 30)] float angularCircleCastPercision = 12f;
    float AngularCastLimit { get => angularCastLimit * Mathf.Deg2Rad; }
    float MinimumAngleForBoxChek { get => minimimumAngleForBoxCheck * Mathf.Deg2Rad; }
    [Header("Debug Features")]
    [SerializeField] bool debugCircleCast = false;
    [SerializeField] bool debugLineCast = false;

    #endregion
    #region Basic State Functions
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

    }

    public override void Initialize(IStateMachine<Enemy> stateMachine)
    {
        base.Initialize(stateMachine);
    }

    public override void PhysicsUpdate()
    {
        Vector2 playerPos = reference.GetPlayerPos();
        HitData hitData = reference.LineOfSightCheck(playerPos);
        switch (hitData.hit)
        {
            case Hita.player:

                break;
            case Hita.notAPlayer:
                GrappleEnemyLogic(hitData, playerPos);
                break;
            case Hita.nothing:

                break;
            default:

                break;
        }
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
    #endregion
    #region Special Functions
    private GrappleData GrappleEnemyLogic(HitData hitData, Vector2 playerPos)
    {
        Vector2 perp = Vector2.Perpendicular(hitData.normal) * marchDistance;
        BoundingBox box = reference.GetBoundingBox(playerPos, boundingBoxSize);

        box.drawBox(Color.yellow, 0);
        var t = Time.realtimeSinceStartupAsDouble;
        marchAndCast(startingDepth, RB.position, hitData.location, perp, box);
        marchAndCast(startingDepth, RB.position, hitData.location, -perp, box);
        Debug.Log((Time.realtimeSinceStartupAsDouble - t) * 1000 + "ms");
        // TODO: Don't forget to set the player position in the GrappleData
        return new GrappleData();
    }

    /// <summary>
    /// Recursive Method for AI enemy tracking.
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="grapplerPosition">Position of the grappler</param>
    /// <param name="startingPosition">Where the LOS hit's the an object (point of view obstruction), every iteration of this method is supposed to march it to the left or right of the normal vector</param>
    /// <param name="perp">Perpendicular Vector to the left or right of the starting normal vector position</param>
    /// <param name="box"> Bounding box surrounding where is a valid grapple location</param>
    /// <returns>A GrappleData struct. If the grappleData is valid then the struct will contain information to grapple around</returns>
    private GrappleData marchAndCast(int depth, Vector2 grapplerPosition, Vector2 startingPosition, Vector2 perp, BoundingBox box)
    {
        // Break from recursion if nothing was found
        if (depth <= 0)
        {
            return new GrappleData(false, Vector2.zero, Vector2.zero);
        }
        // March
        DrawMarch(depth, startingPosition, startingPosition + perp, DrawType.March);
        startingPosition += perp;

        // Line cast
        GrappleData grapplePoint = new GrappleData(false, startingPosition, Vector2.zero);
        RaycastHit2D[] hits = Physics2D.LinecastAll(grapplerPosition, grapplerPosition + (startingPosition - grapplerPosition) * 1.5f);
        DrawMarch(depth, grapplerPosition, grapplerPosition + (startingPosition - grapplerPosition) * 1.5f, DrawType.March);
        bool GrapplePointIsValidSoCircleCast = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.collider.CompareTag("Player"))
            {
                if (debugLineCast) { Tracer.DrawCircle(hit.point, 0.02f, 5, Color.magenta); }
                grapplePoint.grapplePointAvalible = true;
                grapplePoint.grapplePoint = hit.point;
                GrapplePointIsValidSoCircleCast = true;
                break;
            }
        }

        // Circle Cast only if we have a grapple point. 
        if (GrapplePointIsValidSoCircleCast)
        {
            if (CircleCast(depth, grapplePoint, grapplerPosition, box, 1))
            {
                grapplePoint.direction = 1;
                // if circle cast passes, return the grapple point, and direction
                // TODO: Uncomment
                //return grapplePoint;
            }
            else
            {
                // if circle cast fails,
                grapplePoint.grapplePointAvalible = false;
            }

            if (CircleCast(depth, grapplePoint, grapplerPosition, box, -1))
            {
                grapplePoint.direction = -1;
                // if circle cast passes, return the grapple point, and direction
                // TODO: Uncomment/Comment if you want to see full cast range
                //return grapplePoint;
            }
            else
            {
                // if circle cast fails,
                grapplePoint.grapplePointAvalible = false;
            }
        }

        // Decrease depth
        --depth;

        // Recurse
        return marchAndCast(depth, grapplerPosition, startingPosition, perp, box);
    }

    private bool CircleCast(int depth, GrappleData grapplePoint, Vector2 grapplerPosition, BoundingBox box, int sign = 1)
    {
        float theta = angularCircleCastPercision * Mathf.Deg2Rad;
        Vector2 jhat = grapplerPosition - grapplePoint.grapplePoint;
        float length = jhat.magnitude;
        float startingAngle = Mathf.Atan2(jhat.y, jhat.x);
        var prev = grapplerPosition;

        for (float degreesShifted = 0; degreesShifted < AngularCastLimit; degreesShifted += theta)
        {
            // Next is supposed to be the vector that is theta degrees further along the arc path
            var next = CalculateNextCirclePoint(sign * degreesShifted, startingAngle, length, grapplePoint);

            DrawMarch(depth, prev, next, DrawType.Circle);

            RaycastHit2D[] xxxxx = Physics2D.LinecastAll(prev, next);
            foreach (RaycastHit2D hit in xxxxx)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // Stops the casting if we hit the player.
                    if (debugCircleCast) Tracer.DrawCircle(hit.point, 0.05f, 5, Color.red);
                    return true;
                }
                else
                {
                    // Stops the casting if we hit something that isn't the player.
                    if (debugCircleCast) Tracer.DrawCircle(hit.point, 0.05f, 4, Color.green);
                    return false;
                }
            }

            // If we didn't hit, check if we are in the bounding box surrounding the player
            if ((degreesShifted > minimimumAngleForBoxCheck) && box.inBounds(next))
            {
                if (debugCircleCast) { Tracer.DrawCircle(next, 0.03f, 3, Color.green); }
                return true;
            }
            prev = next;
        }
        // If we didn't hit anything, then there is an avalible path
        return true;
    }

    private void DrawMarch(int depth, Vector2 p1, Vector2 p2, DrawType drawType = DrawType.March)
    {
        switch (drawType)
        {
            case DrawType.March:
                if (debugLineCast)
                {
                    Color c = new Color(marchColor.r, marchColor.g, marchColor.b * (depth / (float)startingDepth));
                    Debug.DrawLine(p1, p2, c);
                }
                return;
            case DrawType.Circle:
                if (debugCircleCast)
                {
                    Color c2 = new Color(marchColor.r, marchColor.g, marchColor.b * (depth / (float)startingDepth));
                    Debug.DrawLine(p1, p2, c2);
                }
                return;
            default:
                break;
        }
    }
    #endregion
    #region Enum Definitions
    private enum DrawType
    {
        March,
        Circle,
    }
    #endregion
}
