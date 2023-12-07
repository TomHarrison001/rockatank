using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_ChaseState : R_BaseState
{
    private R_SmartTank tank;

    public R_ChaseState(R_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        tank.stats["chaseState"] = true;

        // tank.GetComponent<Rigidbody>().isKinematic = false;

        /*if (tank.shootParticles.isPlaying)
        {
            tank.shootParticles.Stop();
        }*/

        return null;
    }

    public override Type StateExit()
    {
        tank.stats["chaseState"] = false;

        return null;
    }

    public override Type StateUpdate()
    {
        // tank.ChaseTarget();

        foreach (var item in tank.rules.GetRules)
        {
            if (item.CheckRule(tank.stats) != null)
                return item.CheckRule(tank.stats);
        }

        return null;
    }
}
