using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_ChaseState : R_BaseState
{
    public override void SetState(R_StateManager stateManager)
    {
        stateManager.SetStateToChase();
    }
}
