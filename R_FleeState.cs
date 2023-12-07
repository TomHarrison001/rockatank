using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_FleeState : R_BaseState
{
    private R_SmartTank tank;

    public R_FleeState(R_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        tank.stats["fleeState"] = true;

        // tank.GetComponent<Rigidbody>().isKinematic = false;

        /*if (tank.shootParticles.isPlaying)
        {
            tank.shootParticles.Stop();
        }*/

        return null;
    }

    public override Type StateExit()
    {
        tank.stats["fleeState"] = false;

        return null;
    }

    public override Type StateUpdate()
    {
        // tank.FleeTarget();

        foreach (var item in tank.rules.GetRules)
        {
            if (item.CheckRule(tank.stats) != null)
                return item.CheckRule(tank.stats);
        }

        return null;
    }
}
