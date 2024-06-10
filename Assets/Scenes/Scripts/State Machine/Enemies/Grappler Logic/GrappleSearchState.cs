using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enemy;
[CreateAssetMenu(fileName = "Grapple Search State", menuName ="State Machines/States/Grapple/Grapple Search State")]
public class GrappleSearchState : GrapplerBaseState
{
    #region Parameters
    [Header("Marching Parameters")]
    [SerializeField] float marchDistance = 0.1f;
    [SerializeField] Color marchColor = new Color(0f, 0.2f, 1f);
    [Tooltip("Determines how many marches will be preformed (The total is 2x this since it is done on both sides)")]
    [SerializeField] int startingDepth = 10;
    int StartingDepth
    {
        get => startingDepth;
        set
        {
            startingDepth = value;
            OnDepthChanged();
        }
    }
    // Ensure the setter is called when the value is changed in the inspector
    private void OnValidate()
    {
        StartingDepth = startingDepth;
    }
    private void OnDepthChanged()
    {
        grapplePoints_neg = new GrappleData[2 * startingDepth];
        grapplePoints_pos = new GrappleData[2 * startingDepth];
        grapplePoints_combined = new GrappleData[4 * startingDepth];
    }
    private GrappleData[] grapplePoints_neg;
    private GrappleData[] grapplePoints_pos;
    private GrappleData[] grapplePoints_combined;
    private HitData lineOfSightCollision;
    [Header("Circle Cast Parameters")]
    [SerializeField,Range(0,90)] float minimimumAngleForBoxCheck = 45f;
    [SerializeField,Range(20,270)] float angularCastLimit = 180f;
    [SerializeField] float boundingBoxSize = 1.0f;
    [SerializeField, Range(3, 30)] float angularCircleCastPercision = 12f;
    float AngularCastLimit { get => angularCastLimit * Mathf.Deg2Rad; }
    float MinimumAngleForBoxChek { get => minimimumAngleForBoxCheck * Mathf.Deg2Rad; }

    [SerializeField] int scoreForSuccess = 10;

    [Header("References")]
    BaseDataState<Enemy, object> moverState;

    [Header("Debug Features")]
    [SerializeField] bool debugCircleCast = false;
    [SerializeField] bool debugLineCast = false;
    #endregion

    #region Basic State Functions
    public override void EnterState()
    {
        Debug.Log("Changing To Grapple Search");
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
        grapplePoints_neg = new GrappleData[2 * StartingDepth];
        grapplePoints_pos = new GrappleData[2 * StartingDepth];
        grapplePoints_combined = new GrappleData[4 * startingDepth];
        var request = new Request(GetMoverState, GameManager.moveTag);
        SM.TakeRequest(request);
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
                var g = GrappleEnemyLogic(hitData, playerPos);
                lineOfSightCollision = hitData;
                ParseGrappleData(g, playerPos);
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
        moverState = null;
    }

    public override GrappleData SendData()
    {
        return base.SendData();
    }

    public void GetMoverState(IState state)
    {
        moverState = state as BaseDataState<Enemy, object>;
    }
    public void ChangeBackToMe()
    {
        Debug.Log("Changing back to" + this.name);
        SM.ChangeState(this);
    }
    #endregion
    #region Grapple Functions
    private GrappleData GrappleEnemyLogic(HitData hitData, Vector2 playerPos)
    {
        Vector2 perp = Vector2.Perpendicular(hitData.normal) * marchDistance;
        BoundingBox box = reference.GetBoundingBox(playerPos, boundingBoxSize); /// TODO: replace the player position with the next location after applying the velocity of the player

        box.drawBox(Color.yellow, 0);
        MarchAndCast(StartingDepth, RB.position, hitData.location, -perp, box, ref grapplePoints_neg);
        MarchAndCast(StartingDepth, RB.position, hitData.location, perp, box, ref grapplePoints_pos);
        // Print this to see the form of both arrays.
        //Debug.Log("Before \n" + PrintArray(grapplePoints_neg) + "\n" + PrintArray(grapplePoints_pos));

        // Rearrange the arrays
        for (int i = 0; i < 4 * StartingDepth; i++)
        {
            var g = GetGrappleFromTwoArrays(grapplePoints_neg, grapplePoints_pos, StartingDepth, i);
            grapplePoints_combined[i] = g;
        }
        //Debug.Log("After \n" + PrintArray(grapplePoints_combined));
        int bestIndex = ScoringFunction(ref grapplePoints_combined, StartingDepth, scoreDebug);
        return grapplePoints_combined[bestIndex];

    }

    /// <summary>
    /// Recursive Method for AI enemy tracking.
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="grapplerPosition">Position of the grappler</param>
    /// <param name="startingPosition">Where the LOS hit's the an object (point of view obstruction), every iteration of this method is supposed to march it to the left or right of the normal vector</param>
    /// <param name="perp">Perpendicular Vector to the left or right of the starting normal vector position</param>
    /// <param name="box"> Bounding box surrounding where is a valid grapple location</param>
    /// <returns>[CHANGED, now it's void to not create copies of grapples]A GrappleData struct. If the grappleData is valid then the struct will contain information to grapple around</returns>
    private void MarchAndCast(int depth, in Vector2 grapplerPosition, Vector2 startingPosition, Vector2 perp, BoundingBox box, ref GrappleData[] grapples)
    {
        // Break from recursion if nothing was found
        if (depth <= 0)
        {
            return;
        }

        // March
        DrawMarch(depth, startingPosition, startingPosition + perp, DrawType.March);
        startingPosition += perp;

        // Line cast
        GrappleData grapplePoint = new GrappleData(false, startingPosition);
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

        // Points get arranage so that the array is [negative side swings far -> close | positive side swings close -> far] 
        // Basically so that it reads left to right.
        // Negative side index  = (starting - depth)
        // Postive side index   = (startiing - depth) + starting
        grapplePoint.direction = -1;
        grapples[StartingDepth - depth] = grapplePoint;
        grapplePoint.direction = 1;
        grapples[2 * StartingDepth - depth] = grapplePoint;


        // Circle Cast only if we have a grapple point. Add extra score to the grapple point if it got close to the player or hit the player
        if (GrapplePointIsValidSoCircleCast)
        {
            {
                // Cast in the negative direction
                if (CircleCast(depth, grapplePoint, grapplerPosition, box, out bool hitPlayer, out bool madeItIntoBox,-1))
                {
                    grapples[StartingDepth - depth].circleCastPassed = true;
                    if (hitPlayer) grapples[StartingDepth - depth].score += 5;
                    else if (madeItIntoBox) grapples[StartingDepth - depth].score += 3;
                }
            }
            {
                // Cast in the positive direction
                if (CircleCast(depth, grapplePoint, grapplerPosition, box, out bool hitPlayer, out bool madeItIntoBox ,1))
                {
                    grapples[2 * StartingDepth - depth].circleCastPassed = true;
                    if (hitPlayer) grapples[2 * startingDepth - depth].score += 5;
                    else if (madeItIntoBox) grapples[2 * startingDepth - depth].score += 3;
                }
            }
        }

        // Decrease depth
        --depth;

        // Recurse
        MarchAndCast(depth, grapplerPosition, startingPosition, perp, box, ref grapples);
    }

    private bool CircleCast(int depth, in GrappleData grapplePoint, in Vector2 grapplerPosition, in BoundingBox box, out bool hitPlayer, out bool madeitIntoBox, int sign = 1)
    {
        float theta = angularCircleCastPercision * Mathf.Deg2Rad;
        Vector2 jhat = grapplerPosition - grapplePoint.grapplePoint;
        float length = jhat.magnitude;
        float startingAngle = Mathf.Atan2(jhat.y, jhat.x);
        Vector2 prev = grapplerPosition;
        hitPlayer = false;
        madeitIntoBox = false;
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
                    Debug.Log("Hit the player!");
                    hitPlayer = true;
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
                madeitIntoBox = true;
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
                    Color c = new Color(marchColor.r, marchColor.g, marchColor.b * (depth / (float)StartingDepth));
                    Debug.DrawLine(p1, p2, c);
                }
                return;
            case DrawType.Circle:
                if (debugCircleCast)
                {
                    Color c2 = new Color(marchColor.r, marchColor.g, marchColor.b * (depth / (float)StartingDepth));
                    Debug.DrawLine(p1, p2, c2);
                }
                return;
            default:
                break;
        }
    }

    #region Enum Definitions
    private enum DrawType
    {
        March,
        Circle,
    }
    #endregion

    /// <summary>
    /// Helper function to treat two arrays as one array
    /// </summary>
    /// <param name="negativeSide"></param>
    /// <param name="positiveSide"></param>
    /// <returns></returns>
    static GrappleData GetGrappleFromTwoArrays(in GrappleData[] negativeSide, in GrappleData[] positiveSide, in int startingDepth,in int position)
    {
        // The total length of the array is 4*depth because for each depth we march one way and then try to swing in two directions
        // if depth was 10, then there would be 40 elements if both arrays were combined.

        if (position < (2 * startingDepth))
        {
            /// All negative side swings
           if(position < startingDepth)
            {
                return negativeSide[position];
            }
            else
            {
                return positiveSide[position - startingDepth];
            }
        } else if(position < 4 * startingDepth) {
            /// All positive angle swings
            if(position < 3 * startingDepth)
            {
                return negativeSide[position - startingDepth];
            }
            else
            {
                return positiveSide[position - 2 * startingDepth];
            }
        }
        else
        {
            Debug.LogError("Out of bounds");
            return negativeSide[0];
        }
    }

    public static string PrintArray<T>(T[] array)
    {
        // Convert the array to a string in the desired format
        string result = "[" + string.Join(", ", array) + "]";

        // Print the result
        return result;
    }

    [SerializeField] bool scoreDebug = false;

    /// <summary>
    /// Scores a list of GrappleData and retuns the array, uses a moving window.
    /// </summary>
    /// <param name="grapples"></param>
    /// <param name="startingDepth"></param>
    /// <param name="scoreDebug</param>
    /// <returns>The index of the best point</returns>
    public static int ScoringFunction(ref GrappleData[] grapples, in int startingDepth, bool scoreDebug = false)
    {

        string s="";
        int maxScore = int.MinValue;
        int bestIndex = 0;
        for (int i = 0; i < startingDepth * 2; i++)
        {
            int score;
            ScoreIndiviudalGrapple(ref grapples[i]);

            if ( i == 0 )
            {
                score = ScoreWithNeighbors(ref grapples[i], new GrappleData(), grapples[i + 1], false, true) + 3;
            } 
            else if( i == 2 * startingDepth - 1)
            {
                score = ScoreWithNeighbors(ref grapples[i], grapples[i - 1], new GrappleData(), true, false);
            }
            else
            {
                score = ScoreWithNeighbors(ref grapples[i], grapples[i - 1], grapples[i + 1], true, true) -2;
            }
            if(score > maxScore)
            {
                bestIndex = i;
                maxScore = score;
            }
            //if(scoreDebug) s += "("+i+") " + PrintArray(grapples) + "\n";
        }

        // Score Positive side swings
        for (int i = startingDepth * 2; i < startingDepth * 4; i++)
        {
            int score;
            ScoreIndiviudalGrapple(ref grapples[i]);

            if (i == startingDepth * 2)
            {
                score = ScoreWithNeighbors(ref grapples[i], new GrappleData(), grapples[i + 1], false, true)  -2;
            }
            else if (i == 4 * startingDepth - 1)
            {
                score = ScoreWithNeighbors(ref grapples[i], grapples[i - 1], new GrappleData(), true, false);
            }
            else
            {
                score = ScoreWithNeighbors(ref grapples[i], grapples[i - 1], grapples[i + 1], true, true) + 3;
            }
            if (score > maxScore)
            {
                bestIndex = i;
                maxScore = score;
            }
            //if (scoreDebug) s += "(" + i + ") " + PrintArray(grapples) + "\n";
        }

        if (scoreDebug) { s += "(a) " + PrintArray(grapples) + "\n"; s += "Best Num=" + grapples[bestIndex] + "\n"; }
        if (scoreDebug) Debug.Log(s);
        return bestIndex;
    }
    public static int ScoreIndiviudalGrapple(ref GrappleData grapple)
    {
        if (grapple.grapplePointAvalible)
        {
            grapple.score += 2;
            if (grapple.circleCastPassed)
            {
                grapple.score += 3;
            }
        }
        else
        {
            grapple.score-=5;
        }
        return grapple.score;
    }
    public static int ScoreWithNeighbors(ref GrappleData grapple, in GrappleData Lneighbor, in GrappleData Rneighbor, bool hasLneighbor = true, bool hasRneighbor = true)
    {
        if(!grapple.grapplePointAvalible) { return grapple.score; }

        if (hasLneighbor)
        {
            if(Lneighbor.grapplePointAvalible) {
                grapple.score++;
                if (Lneighbor.circleCastPassed)
                {
                    grapple.score++;
                }
            }
        }

        if (hasRneighbor)
        {
            if (Rneighbor.grapplePointAvalible)
            {
                grapple.score++;
                if (Rneighbor.circleCastPassed)
                {
                    grapple.score++;
                }
            }
        }
        Tracer.DrawCircle(grapple.grapplePoint, 0.01f * grapple.score, 3, new Color(grapple.score / 10, grapple.score / 10, grapple.score / 10)); 
        return grapple.score;
    }
    #endregion

    /// <summary>
    /// The best point to grapple from theoretically is so that the grapple point lies directly at the center of the player and the grappler.
    /// </summary>
    /// <param name="lineOfSightCollision"> Point where the line of site collides with a collider</param>
    /// <param name="playerPos"></param>
    /// <returns></returns>
    public static Vector2 FindOptimalGrapplingPostion(in HitData lineOfSightCollision, in Vector2 playerPos)
    {
        Vector2 pos = (2 * lineOfSightCollision.location) - playerPos;
        return pos;
    }
    /// <summary>
    /// Used as a delegate if it is desired for the mover state to keep updating its location to a certain point
    /// </summary>
    /// <returns></returns>
    public Vector2 FrameUpdatedGrapplingPosition()
    {
        var pos = FindOptimalGrapplingPostion(lineOfSightCollision, reference.GetPlayerPos());
#if UNITY_EDITOR
        Tracer.DrawCircle(lineOfSightCollision.location, 0.5f, 5, Color.magenta);
        Debug.DrawLine(RB.position, pos);
#endif
        return pos;
    }

    #region Brains and Requester Logic
    public void ParseGrappleData(GrappleData data, Vector2 playerPos)
    {
        Debug.Log("Parsing Grapple Data: " + data);
        if (!data.grapplePointAvalible)
        {
            // Implement something
            // Walk left or right and back
            if (moverState != null)
            {
                Vector2 location = Vector2.zero;
                moverState.RecieveData(location);
                SM.ChangeState(moverState);
            }
            return;
        }
        if (!data.circleCastPassed)
        {
            // Request to move towards a location and then call back to this state after X amount of time
            ParseGrappleData_AdjustPos(data, playerPos);
            return;
        }
        // If we meet the requirements then we should try to grapple
        if(data.score > scoreForSuccess)
        {
            var chainState = (SM as BasicChainState);
            if(chainState != null)
            {
                RecieveData(data);
                chainState.NextState(); // TODO: in grapple state make sure to call RecieveData(SM.PreviousState.SendData())
            } else
            {
                var request = new Request(Vector2.zero, Request.SimpleRequest.Grapple);
                request.dataRequest = data;
                SM.TakeRequest(request);
            }
            return;
        }
        // If we didn't meet the required grapple score we should readjust our position
        else
        {
            ParseGrappleData_AdjustPos(data, playerPos);
        }
    }

    /// <summary>
    /// Helper function for parsing grapple data: this function will try to readjust the players position
    /// </summary>
    /// <param name="data"></param>
    /// <param name="playerPos"></param>
    private void ParseGrappleData_AdjustPos(GrappleData data, Vector2 playerPos)
    {
        Debug.Log("Trying to readjust Position");
        Vector2 location = FrameUpdatedGrapplingPosition();/* FindOptimalGrapplingPostion(lineOfSightCollision, playerPos);*/
        if (moverState != null)
        {
            // directly handle with the move state
            moverState.RecieveData(new MoveRequest(location));
        }
        else
        {
            // Ask state machine to deal with the request
            var request = new Request(locationRequest: location, Request.SimpleRequest.MoveTo, onFuffill: ChangeBackToMe);
            SM.TakeRequest(request);
        }
    }
    #endregion
}
