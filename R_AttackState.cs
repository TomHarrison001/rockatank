using System.Collections;
using System.Collections.Generic;
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

        // tank.GetComponent<Rigidbody>().isKinematic = true;

        /*if (!tank.shootParticles.isPlaying)
        {
            tank.shootParticles.Play();
        }*/

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
        // tank.AttackTarget();

        time += Time.deltaTime;

        if (time > 1f)
        {
            if (tank.stats["lowHealth"])
                return typeof(R_FleeState);
            if (tank.stats["targetReached"])
                return typeof(R_AttackState);
            else
                return typeof(R_SearchState);
        }

        return null;
    }
}
