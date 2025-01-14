using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SpellData", menuName = "Item/Spells/Spell Data")]
public class SpellData : Item, IActivatable, IChangeCrossHair
{
    [Space]
    [Header("--Spell Related--")]
    public int maxAmount = 1;
    public float speedOnCast = 1;
    [SerializeField] float coolDownDuration = 1f;
    CoolDownBasic coolDown = null;

    public SpellBehavior spellBehavior;
    public void Activate()
    {
        Activate(null, null);
    }

    public void Activate(Dictionary<string, float> floats = null, IFighter fighter = null)
    {
        // if the cooldown is not complete return
        if (!CanActivate()) { return; }

        // Cast the spell
        if (floats == null) { floats = new(); }
        PrepCast(
            floats: floats,
            fighter: fighter
        );

        // start the cooldown
        coolDown.Duration = coolDownDuration;
        coolDown.StartCooldown();
    }

    /// <summary>
    /// Trys to cast IFighter to Player before calling on cast
    /// </summary>
    /// <param name="floats"></param>
    /// <param name="fighter">Dictionary continaing data related to the spell</param>
    private void PrepCast(
        Dictionary<string, float> floats = null,
        IFighter fighter = null
        )
    {
        if (fighter != null)
        {
            Player player = fighter as Player;
            if (player != null)
            {
                spellBehavior.OnCast(floats, player, this);
                return;
            }
        }

        spellBehavior.OnCast(floats, fighter, this);
    }
    public bool CanActivate()
    {
        if(coolDown == null)
        {
            coolDown = new CoolDownBasic(duration: 0, startOnCreation: true);
        }
        return coolDown.IsCooldownComplete;
    }

    public Dictionary<string, float> GetParameters()
    {
        return spellBehavior.GetParameters();
    }

    public Transform LocationOfSelf()
    {
        return spellBehavior.LocationOfSelf();
    }

    public CrossHairScript.CrossHairParams GetCrossHairParams()
    {
        return spellBehavior.GetCrossHairParams();
    }

}
