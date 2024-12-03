using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireSpellBehavior", menuName = "Item/Spells/Behaviours/Fire Spell Behavior")]
public class FireSpellBehavior : SpellBehavior
{
    [SerializeField] private GameObject bouncySpellPrefab;
    public override bool CanActivate()
    {
        return base.CanActivate();
    }

    public override Dictionary<string, float> GetParameters()
    {
        // Get parameters from super class
        Dictionary<string,float> paramaters = base.GetParameters();

        // Add new parameters if needed
        paramaters[VEL] = 0;

        return paramaters;
    }

    public override Transform LocationOfSelf()
    {
        return base.LocationOfSelf();
    }

    public override void OnCast(Dictionary<string, float> floats = null, Player player = null, SpellData spellData = null)
    {
        base.OnCast(floats, player, spellData);

        float xpos = 0;
        float ypos = 0;
        float xdir = 0;
        float ydir = 0;
        float xoff = 0;
        float yoff = 0;

        if (floats == null)
        {
            floats = new();
        }

        #region Get Values from player and dictionary

        // if x/y pos wasn't specificed, use the player location
        if (
            (floats.TryGetValue(XPOS, out xpos) && 
             floats.TryGetValue(YPOS, out ypos)) == false
        ) {
            if(player != null)
            {
                var pos = player.GetPosition();
                (xpos, ypos) = (pos.x, pos.y);
            }
        }

        // if the x/y direction wasnt specificed use the direction of the cursor
        if (
            (floats.TryGetValue(XDIR, out xdir) &&
             floats.TryGetValue(YDIR, out ydir)) == false
        ) {
            Vector2 crossHair = player.GetCrossHairPosition();
            Vector2 pos = player.GetPosition();
            // Vector that points from the player to the cross hair
            Vector2 direction = (crossHair - pos).normalized;
            (xdir, ydir) = (direction.x, direction.y);
        }

        // if the x/y offset wasnt specificed use the off set of the cursor
        if (
            (floats.TryGetValue(XOFF, out yoff) &&
             floats.TryGetValue(YOFF, out yoff)) == false
        ) {
            Vector2 crossHair = player.GetCrossHairPosition();
            Vector2 pos = player.GetPosition();
            Vector2 offset = (crossHair - pos);
            (xoff, yoff) = (offset.x, offset.y);
        }

        #endregion

        // Save the values we retrived from the player
        floats[XPOS] = xpos;
        floats[YPOS] = ypos;
        floats[XDIR] = xdir;
        floats[YDIR] = ydir;
        floats[XOFF] = xoff;
        floats[YOFF] = yoff;

        // Pass it to OnCast()
        OnCast(floats, spellData);
    }

    public override void OnCast(Dictionary<string, float> floats = null, IFighter fighter = null, SpellData spellData = null)
    {
        Debug.LogWarning("FIGHTER IS TRYING TO USE A SPELL NOT THE PLAYER");
        float xpos = 0;
        float ypos = 0;
        float xdir = 0;
        float ydir = 0;
        float xoff = 0;
        float yoff = 0;

        #region Get Values from player and dictionary

        // if x/y pos wasn't specificed, use the player location
        if (
            (floats.TryGetValue(XPOS, out xpos) &&
             floats.TryGetValue(YPOS, out ypos)) == false
        )
        {
            if (fighter != null)
            {
                var pos = fighter.GetMono().transform.position;
                (xpos, ypos) = (pos.x, pos.y);
            }
        }

        // if the x/y direction wasnt specificed use the direction of the cursor
        if (
            (floats.TryGetValue(XDIR, out xdir) &&
             floats.TryGetValue(YDIR, out ydir)) == false
        )
        {
            Vector2 crossHair = fighter.GetMono().transform.position;
            Vector2 pos = fighter.GetMono().transform.position;
            // Vector that points from the player to the cross hair
            Vector2 direction = (crossHair - pos).normalized;
            (xdir, ydir) = (direction.x, direction.y);
        }

        // if the x/y offset wasnt specificed use the off set of the cursor
        if (
            (floats.TryGetValue(XOFF, out yoff) &&
             floats.TryGetValue(YOFF, out yoff)) == false
        )
        {
            Vector2 offset = new Vector2(0.1f, 0.1f);
            (xoff, yoff) = (offset.x, offset.y);
        }

        #endregion
        // Save the values we retrived from the iFighter
        floats[XPOS] = xpos;
        floats[YPOS] = ypos;
        floats[XDIR] = xdir;
        floats[YDIR] = ydir;
        floats[XOFF] = xoff;
        floats[YOFF] = yoff;

        OnCast(floats, spellData);
    }

    /// <summary>
    /// This OnCast should execute with only needing a dictionary of float values
    /// </summary>
    /// <param name="floats"></param>
    protected virtual void OnCast(Dictionary<string, float> floats, SpellData spellData = null)
    {

        float xpos = floats[XPOS];
        float ypos = floats[YPOS];
        float xdir = floats[XDIR];
        float ydir = floats[YDIR];
        float xoff = floats[XOFF];
        float yoff = floats[YOFF];
        float vel  = 0;

        // if velocity wasn't found
        if (floats.TryGetValue(VEL, out vel) == false)
        {
            // try to get it from spell data
            if(spellData != null)
            {
                vel = spellData.speedOnCast;
            }
        }

        // Define the target vector
        //Vector2 targetVector = new Vector2(xdir, ydir);

        // Calculate the angle of rotation
        //float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;

        float angle = 0;
        // Construct the quaternion using the angle
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

#if UNITY_EDITOR
        Tracer.DrawCircle(new Vector2(xpos + xoff, ypos + yoff), 0.05f, 5, Color.yellow, 5);
        Debug.DrawLine(new Vector2(xpos, ypos), new Vector2(xpos + xoff, ypos + yoff), Color.red, 5);
#endif

        var instance = GameObject.Instantiate(bouncySpellPrefab, new Vector3(xpos + xoff, ypos + yoff), rotation);

        Rigidbody2D SpellRB = instance.GetComponent<Rigidbody2D>();

        
        var force = vel*(new Vector2(xdir, ydir).normalized);

        SpellRB.velocity = force;
    }
}
