using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBehavior : ScriptableObject, IActivatable, IChangeCrossHair
{
    public const string XPOS = "XPOS";
    public const string YPOS = "YPOS";
    public const string XDIR = "XDIR";
    public const string YDIR = "YDIR";
    public const string XOFF = "XOFF";
    public const string YOFF = "YOFF";
    public const string VEL = "VEL";

    [SerializeField] Sprite crossHairSprite;
    public void Activate()
    {
        Activate(null, null);
    }

    public void Activate(Dictionary<string, float> floats = null, IFighter fighter = null)
    {
        Debug.LogError("Don't use this in game, use SpellData.Activate(). Can't activate spell without spell data");
        if (fighter != null)
        {
            Player player = fighter as Player;
            if (player != null)
            {
                OnCast(floats, player, null);
                return;
            }
        }
        OnCast(floats, fighter, null);
    }

    public virtual bool CanActivate()
    {
        return true;
    }

    public virtual Transform LocationOfSelf()
    {
        return null;
    }



    public virtual void OnCast(
        Dictionary<string, float> floats = null,
        Player player = null,
        SpellData spellData = null
    )
    {

    }

    public virtual void OnCast(
        Dictionary<string, float> floats = null,
        IFighter fighter = null,
        SpellData spellData = null
    )
    {

    }

    public virtual Dictionary<string, float> GetParameters()
    {
        return new Dictionary<string, float>()
        {
            [XPOS] = 0,
            [YPOS] = 0,
            [XDIR] = 0,
            [YDIR] = 0,
            [XOFF] = 0,
            [YOFF] = 0,
        };
    }

    public virtual CrossHairScript.CrossHairParams GetCrossHairParams()
    {
        CrossHairScript.CrossHairParams crossHairParams = new CrossHairScript.CrossHairParams(
            hasMaxDistance: true,
            hasMinDistance: false,
            maxDistance: 0.8f,
            minDistance: 0.0f,
            spriteTexture: crossHairSprite
            ); 
        return crossHairParams;
    }
}
