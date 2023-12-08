using System;
using UnityEngine;

public class R_AttackState : R_BaseState
{
    private R_SmartTank tank;
    private float time = 0;

    public R_AttackState(R_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        tank.stats["attackState"] = true;

        return null;
    }

    public override Type StateExit()
    {
        tank.stats["attackState"] = false;

        time = 0;
        return null;
    }

    public override Type StateUpdate()
    {
        tank.Attack();

        time += Time.deltaTime;

        if (time > 1f)
        {
            if (tank.stats["lowHealth"] || tank.stats["noAmmo"])
                return typeof(R_FleeState);
            if (tank.stats["targetEscaped"])
                return typeof(R_SearchState);
            else
                return typeof(R_AttackState);
        }

        return null;
    }
}
