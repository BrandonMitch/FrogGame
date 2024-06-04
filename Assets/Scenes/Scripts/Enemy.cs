using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrappleEnemyLogic;

public class Enemy : Fighter
{
    [SerializeField] static ChainOffState<Enemy, Object> l;
    [SerializeField] protected PlayerReferenceSO playerRef;
    [SerializeField] protected List<BaseState<Enemy>> states = new();
    [SerializeField] bool debugLineCasts = true;
    [SerializeField] BoundingBox playerBoundingBox = new BoundingBox(Vector2.zero, -1);
    protected StateMachineBase<Enemy> SM;
    
    protected override void Start()
    {
        base.Start();

        /// Create a new state machine, then intialize it by giving it a refernce to the enemy
        SM = new StateMachineBase<Enemy>();
        SM.InitializeStateMachine(this);

        /// Reset all values of the states, and intialize them
        foreach (BaseState<Enemy> state in states)
        {
            if (state != null)
            {
                state.ResetValues();
                state.Initialize(SM);
            }
        }

        /// Start the state machine with the first state
        if (states.Count > 0)
        {
            Debug.Log("Start SM with state = " + states[0]);
            SM.StartStateMachine(states[0]);
        }
    }
    protected void FixedUpdate()
    {
        SM.CurrentState.PhysicsUpdate();
    }
    public HitData LineOfSightCheck()
    {
        Vector2 playerPos = GetPlayerPos();
        return LineOfSightCheck(playerPos);
    }
    public HitData LineOfSightCheck(Vector2 playerPos)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(RB.position, playerPos);

        HitData returnHit = new HitData(Vector2.zero, Vector2.zero, Hita.nothing);

        if (debugLineCasts) { Debug.DrawLine(RB.position, playerPos, Color.red); }
        foreach (RaycastHit2D hit in hits)
        {
            if (debugLineCasts)
            {
                Tracer.DrawCircle(hit.point, 0.05f, 5, Color.yellow);
            }

            if (hit.collider.CompareTag("Player"))
            {

                returnHit.SetInfo(Vector2.zero, Vector2.zero, Hita.player);
            }
            else
            {
                return returnHit.SetInfo(hit.point, hit.normal, Hita.notAPlayer);
            }
        }
        return returnHit;
    }
    #region Struct Definitions
    public struct HitData
    {
        public Hita hit;
        public Vector2 location;
        public Vector3 normal;

        public HitData(Vector2 location, Vector2 normal, Hita hita = Hita.nothing)
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
    public struct GrappleData
    {
        public bool grapplePointAvalible;
        public int direction;
        public Vector2 grapplePoint;
        public Vector2 playerPos;
        public GrappleData(bool grapplePointAvalible, Vector2 grapplePoint, Vector2 playerPos, int direction = 1)
        {
            this.grapplePointAvalible = grapplePointAvalible;
            this.direction = direction;
            this.grapplePoint = grapplePoint;
            this.playerPos = playerPos;
        }
    }
    public struct BoundingBox
    {
        private float xpointLeft;
        private float xpointRight;
        private float ypointTop;
        private float ypointBot;
        public float size;
        private Vector2 location;
        public BoundingBox(Vector2 location, float size)
        {
            this.size = size;
            this.location = location;
            xpointRight = location.x + size;
            xpointLeft = location.x - size;
            ypointTop = location.y + size;
            ypointBot = location.y - size;
        }

        /// <summary>
        /// Used to update the position of the bounding box.
        /// </summary>
        /// <param name="location">New spot to draw bounding box</param>
        /// <param name="size">1/2 the side length of the box</param>
        public void Update(Vector2 location, float size)
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
            if (
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
    #endregion
    #region Enum Definitions
    public enum Hita
    {
        player,
        notAPlayer,
        nothing,
    }
    #endregion
    #region Getters
    public PlayerReferenceSO GetPlayerReferenceSO()
    {
        return playerRef; 
    }
    public Vector2 GetPlayerPos()
    {
        return playerRef.GetPlayer(RB.position).GetPosition();
    }
    public Rigidbody2D GetRigidbody()
    {
        return RB;
    }
    public BoundingBox GetBoundingBox(Vector2 playerPos, float size)
    {
        if(playerBoundingBox.size < 0)
        {
            playerBoundingBox = new BoundingBox(playerPos, size);
        }
        else
        {
            playerBoundingBox.Update(playerPos, size);
        }
        return playerBoundingBox;
    }
    #endregion
}
