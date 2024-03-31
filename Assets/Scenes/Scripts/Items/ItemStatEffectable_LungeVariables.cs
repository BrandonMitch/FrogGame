using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lunge Effect Item", menuName = "Item/LungeEffectItem")]
public class ItemStatEffectable_LungeVariables : ItemStatEffectable
{                                                   
    [Header("|-----Lunge Variables-----|")]                        
    public float lateralForceModiferADD;
    public float minimumLateralDurationADD;
    public float lateralDragCoefficientADD;
    public float dampingCoefficientADD;
    public float minimumDistanceToSpawnANewPointADD;
    public float minimumTimeToSpawnANewPointADD;
    public float forwardLungeCoefficientADD;
    public float forwardLungeForceModiferADD;
    [Space]
    public float lateralLungeEaseInFramesADD;
    public float lateralLungeEaseOutFramesADD;
    public float lateralLungeDesiredVELADD;               

    public override void ApplyStats()
    {

    }
}
