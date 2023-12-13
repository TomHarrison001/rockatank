using System;
using System.Collections.Generic;
using UnityEngine;

public class R_CampState : R_BaseState
{
    private R_SmartTank tank;

    public R_CampState(R_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        tank.stats["campState"] = true;

        return null;
    }

    public override Type StateExit()
    {
        tank.stats["campState"] = false;

        return null;
    }

    public override Type StateUpdate()
    {
        tank.Search();

        foreach (var item in tank.rules.GetRules)
        {
            if (item.CheckRule(tank.stats) != null)
                return item.CheckRule(tank.stats);
        }

        return null;
    }


}
