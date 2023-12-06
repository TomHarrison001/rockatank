using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_SearchState : R_BaseState
{
    public override void SetState(R_StateManager stateManager)
    {
        stateManager.SetStateToSearch();
    }
}
