using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleEnemyLogic : MonoBehaviour
{
    public enum Hita
    {
        player,
        notAPlayer,
        nothing,
    }
    public struct HitData
    {
        public Hita hit;
        public Vector2 location;
        public Vector3 normal;

        public HitData(Vector2 location, Vector2 normal , Hita hita = Hita.nothing)
        {
            this.hit = hita;
            this.location = location;
            this.normal = normal;
        }
        public HitData SetInfo(Vector2 location, Vector2 normal, Hita hita = Hita.nothing)
        {
            this.hit = hita;
            this.location = location;
            this.normal = normal;
            return this;
        }
    }

    protected struct GrappleData{
        public bool grapplePointAvalible;
        public int direction;
        public Vector2 grapplePoint;

        public GrappleData(bool grapplePointAvalible, Vector2 grapplePoint, int direction = 1)
        {
            this.grapplePointAvalible = grapplePointAvalible;
            this.direction = direction;
            this.grapplePoint = grapplePoint;
        }
    }

    protected struct BoundingBox
    {
        readonly float xpointLeft;
        readonly float xpointRight;
        readonly float ypointTop;
        readonly float ypointBot;
        readonly float size;
        readonly Vector2 location;
        public BoundingBox(float size, Vector2 location)
        {
            this.size = size;
            this.location = location;
            xpointRight = location.x + size;
            xpointLeft = location.x - size;
            ypointTop = location.y + size;
            ypointBot = location.y - size;
        }

        public bool inBounds(Vector2 point)
        {
            if(
                point.x > xpointLeft && point.x < xpointRight && 
                point.y < ypointTop && point.y > ypointBot
                )
            {
                return true;
            }
            return false;
        }
        public void drawBox(Color c, float t)
        {
            Vector2 TR = location + new Vector2(size, size);
            Vector2 TL = location + new Vector2(-size, size);
            Vector2 BL = location + new Vector2(-size, -size);
            Vector2 BR = location + new Vector2(size, -size);

            Debug.DrawLine(TR, TL, c, t);
            Debug.DrawLine(TL, BL, c, t);
            Debug.DrawLine(BL, BR, c, t);
            Debug.DrawLine(BR, TR, c, t);
        }
    }

    bool debugLineCasts = true;
    private bool debugCircleCasts = true;
    [SerializeField] float angularCastLimit = 180f;
    [SerializeField] float minimimumAngleForBoxCheck = 45f;
    [SerializeField] float marchDistance = 0.1f;
    [SerializeField] Color marchColor = new Color(0f, 0.2f, 1f);
    static int startingDepth = 10;
    // Start is called before the first frame update
    [SerializeField] Rigidbody2D RB;
    [SerializeField] Rigidbody2D playerRB;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerPos = playerRB.position;
        HitData hitData = LineOfSiteCheck(playerPos);

        switch (hitData.hit)
        {
            case Hita.player:
                break;
            case Hita.notAPlayer:
                GrappleEnemyLogicAlg(hitData, playerPos);
                break;
            case Hita.nothing:

                break;
            default:

                break;
        }
    }
    protected HitData LineOfSiteCheck(Vector2 playerPos)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(RB.position, playerPos);

        HitData returnHit = new HitData(Vector2.zero, Vector2.zero, Hita.nothing);

        if(debugLineCasts) { Debug.DrawLine(RB.position, playerPos, Color.red); }
        foreach (RaycastHit2D hit in hits)
        {
            if (debugLineCasts)
            {
                Tracer.DrawCircle(hit.point, 0.05f, 5, Color.yellow);
            }

            if (hit.collider.CompareTag("Player")) {

                returnHit.SetInfo(Vector2.zero, Vector2.zero, Hita.player);
            }
            else
            {
                return returnHit.SetInfo(hit.point, hit.normal, Hita.notAPlayer);
            }
        }
        return returnHit;
    }
    GrappleData GrappleEnemyLogicAlg(HitData hit, Vector2 playerPos)
    {
        // First take n x k and n x -k
        Vector2 perp = Vector2.Perpendicular(hit.normal)*marchDistance;
        BoundingBox box = new BoundingBox(0.3f, playerPos);
        box.drawBox(Color.yellow, 0);
        marchAndCast(startingDepth, RB.position, hit.location, perp, box);
        marchAndCast(startingDepth, RB.position, hit.location, -perp, box);
        return new GrappleData();
    }
    /// <summary>
    /// Recursive Method for AI enemy tracking.
    /// 
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
            return new GrappleData(false, Vector2.zero);
        }
        // March
        DrawMarch(depth, startingPosition, startingPosition + perp);
        startingPosition += perp;

        // Line cast
        GrappleData grapplePoint = new GrappleData(false, startingPosition);
        RaycastHit2D[] hits = Physics2D.LinecastAll(grapplerPosition, grapplerPosition +  (startingPosition - grapplerPosition)*1.5f);
        DrawMarch(depth, grapplerPosition, grapplerPosition + (startingPosition - grapplerPosition) * 1.5f);
        bool GrapplePointIsValidSoCircleCast = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.collider.CompareTag("Player"))
            {
                Tracer.DrawCircle(hit.point, 0.02f, 5, Color.magenta);
                grapplePoint.grapplePointAvalible = true;
                grapplePoint.grapplePoint = hit.point;
                GrapplePointIsValidSoCircleCast = true;
                break;
            }
        }

        // Circle Cast only if we have a grapple point. 
        if (GrapplePointIsValidSoCircleCast) {
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
        float theta = 12.0f * Mathf.Deg2Rad;
        Vector2 jhat = grapplerPosition - grapplePoint.grapplePoint;
        float length = jhat.magnitude;
        float startingAngle = Mathf.Atan2(jhat.y, jhat.x);
        var prev = grapplerPosition;

        for (float degreesShifted = 0; degreesShifted < Mathf.PI; degreesShifted += theta)
        {
            // Next is supposed to be the vector that is theta degrees further along the arc path
            var next = CalculateNextCirclePoint(sign * degreesShifted, startingAngle, length, grapplePoint);

            if (debugCircleCasts)
            {
                DrawMarch(depth, prev, next);
            }
            RaycastHit2D[] xxxxx = Physics2D.LinecastAll(prev, next);
            foreach (RaycastHit2D hit in xxxxx)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // Stops the casting if we hit the player.
                    Tracer.DrawCircle(hit.point, 0.05f, 5, Color.red);
                    return true;
                }
                else
                {
                    Tracer.DrawCircle(hit.point, 0.05f, 4, Color.green);
                    // Stops the casting if we hit something that isn't the player.
                    return false;
                }

            }

            // If we didn't hit, check if we are in the bounding box surrounding the player
            if ( (degreesShifted > minimimumAngleForBoxCheck) && box.inBounds(next) )
            {
                Tracer.DrawCircle(next, 0.04f, 3, Color.green);
                return true;
            }
            prev = next;
        }
        // If we didn't hit anything, then there is an avalible path
        return true;
    }

    private void DrawMarch(int depth, Vector2 p1, Vector2 p2)
    {
        Color c = new Color(marchColor.r, marchColor.g, marchColor.b*(depth/(float)startingDepth));
        Debug.DrawLine(p1, p2, c);
    }

    /// <summary>
    /// Returns the shifted version of the next circle point
    /// </summary>
    /// <param name="degreesShifted"></param>
    /// <param name="startingAngle"></param>
    /// <param name="length"></param>
    /// <param name="grapplePoint"></param>
    /// <returns></returns>
    private static Vector2 CalculateNextCirclePoint(float degreesShifted, float startingAngle, float length, GrappleData grapplePoint)
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
    private static Vector2 CalculateNextCirclePoint(float degreesShifted, float startingAngle, float length)
    {
        float newAngle = degreesShifted + startingAngle;
        return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)) * length;
    }

    private static Vector2 ihat(Vector2 jhat, int direction = 1)
    {
        return direction*Vector2.Perpendicular(jhat);
    }
}
