using System;
using System.Collections.Generic;
using UnityEngine;

public class R_FleeState : R_BaseState
{
    private R_SmartTank tank;
    private float t;

    public R_FleeState(R_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        tank.stats["fleeState"] = true;

        return null;
    }

    public override Type StateExit()
    {
        tank.stats["fleeState"] = false;

        return null;
    }

    public override Type StateUpdate()
    {
        tank.Flee();

        t += Time.deltaTime;
        if (t < 5) return null;

        t = 0;
        foreach (var item in tank.rules.GetRules)
        {
            if (item.CheckRule(tank.stats) != null)
                return item.CheckRule(tank.stats);
        }

        return null;
    }
}
