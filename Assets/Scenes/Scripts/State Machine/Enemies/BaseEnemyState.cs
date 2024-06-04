using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyState : BaseState<Enemy>
{
    protected PlayerReferenceSO playerReferenceSO;
    public override void Initialize(IStateMachine<Enemy> stateMachine)
    {
        base.Initialize(stateMachine);
        playerReferenceSO = reference.GetPlayerReferenceSO();
    }
}
