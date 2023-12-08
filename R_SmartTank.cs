using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// require R_StateMachine
[RequireComponent(typeof(R_StateMachine))]

public class R_SmartTank : AITank
{
    // consumables: hp +25, ammo +3, fuel +30
    // projectile: 15 dmg

    // store ALL currently visible 
    public Dictionary<GameObject, float> enemyTanksFound = new();
    public Dictionary<GameObject, float> consumablesFound = new();
    public Dictionary<GameObject, float> enemyBasesFound = new();

    // store ONE from ALL currently visible
    public GameObject enemyTankPosition;
    public GameObject consumablePosition;
    public GameObject enemyBasePosition;

    // timer
    private float t;

    // store facts and rules
    public Dictionary<string, bool> stats = new();
    public R_Rules rules = new();

    private void Awake()
    {
        InitialiseStateMachine();
    }

    // AITankStart() used instead of Start()
    public override void AITankStart()
    {
        // add facts
        stats.Add("lowHealth", false);
        stats.Add("lowFuel", false);
        stats.Add("noAmmo", false);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("targetEscaped", false);
        stats.Add("searchState", true);
        stats.Add("chaseState", false);
        stats.Add("fleeState", false);
        stats.Add("attackState", false);

        // add rules
        rules.AddRule(new R_Rule("chaseState", "targetReached", typeof(R_AttackState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("chaseState", "lowHealth", typeof(R_FleeState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("chaseState", "noAmmo", typeof(R_FleeState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("attackState", "lowHealth", typeof(R_FleeState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("attackState", "noAmmo", typeof(R_FleeState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("attackState", "targetEscaped", typeof(R_SearchState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("searchState", "targetSpotted", typeof(R_ChaseState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("fleeState", "targetEscaped", typeof(R_SearchState), R_Rule.Predicate.AND));
        rules.AddRule(new R_Rule("chaseState", "targetEscaped", typeof(R_SearchState), R_Rule.Predicate.AND));
    }

    // AITankUpdate() in place of Update()
    public override void AITankUpdate()
    {
        // update all currently visible enemies, consumables and bases
        enemyTanksFound = VisibleEnemyTanks;
        consumablesFound = VisibleConsumables;
        enemyBasesFound = VisibleEnemyBases;

        // check for enemies, consumables and bases found
        enemyTankPosition = (enemyTanksFound.Count > 0) ? enemyTanksFound.First().Key : null;
        consumablePosition = (consumablesFound.Count > 0) ? consumablesFound.First().Key : null;
        enemyBasePosition = (enemyBasesFound.Count > 0) ? enemyBasesFound.First().Key : null;

        // set facts
        stats["lowHealth"] = TankCurrentHealth < 35f;
        stats["lowFuel"] = TankCurrentFuel < 20f;
        stats["noAmmo"] = TankCurrentAmmo == 0f;
        stats["targetSpotted"] = enemyTankPosition != null || consumablePosition != null || enemyBasePosition != null;
        stats["targetEscaped"] = !stats["targetSpotted"];
        stats["targetReached"] = false;
        if (!stats["targetSpotted"]) return;
        if (enemyTankPosition != null)
            stats["targetReached"] = Vector3.Distance(transform.position, enemyTankPosition.transform.position) < 25f;
        else if (consumablePosition != null)
            stats["targetReached"] = Vector3.Distance(transform.position, consumablePosition.transform.position) < 0f;
        else if (enemyBasePosition != null)
            stats["targetReached"] = Vector3.Distance(transform.position, enemyBasePosition.transform.position) < 25f;
    }

    // add states to dictionary
    protected void InitialiseStateMachine()
    {
        Dictionary<Type, R_BaseState> states = new()
        {
            { typeof(R_SearchState), new R_SearchState(this) },
            { typeof(R_ChaseState), new R_ChaseState(this) },
            { typeof(R_FleeState), new R_FleeState(this) },
            { typeof(R_AttackState), new R_AttackState(this) }
        };

        GetComponent<R_StateMachine>().SetStates(states);
    }

    public void Search()
    {
        TurretReset();
        FollowPathToRandomWorldPoint(0.7f);
        t += Time.deltaTime;
        if (t > 10)
        {
            GenerateNewRandomWorldPoint();
            t = 0;
        }
    }

    public void Chase()
    {
        GameObject pos;
        float normalisedSpeed;
        if (enemyTankPosition != null)
        {
            pos = enemyTankPosition;
            normalisedSpeed = 1f;
        }
        else if (consumablePosition != null)
        {
            pos = consumablePosition;
            normalisedSpeed = 0.7f;
        }
        else if (enemyBasePosition != null)
        {
            pos = enemyBasePosition;
            normalisedSpeed = 0.5f;
        }
        else return;
        FollowPathToWorldPoint(pos, normalisedSpeed);
        TurretFaceWorldPoint(pos);
    }

    public void Flee()
    {
        // TODO: implement flee function
        Search();
    }

    public void Attack()
    {
        if (enemyTankPosition != null)
        {
            TurretFireAtPoint(enemyTankPosition);
            TankGo();
        }
        else if (enemyBasePosition != null)
            TurretFireAtPoint(enemyBasePosition);
    }

    // AIOnCollisionEnter() in place of OnCollisionEnter()
    public override void AIOnCollisionEnter(Collision collision) { }

    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject)
    /// </summary>
    public void GeneratePathToWorldPoint(GameObject pointInWorld)
    {
        FindPathToPoint(pointInWorld);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1)
    /// </summary>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed)
    {
        FollowPathToPoint(pointInWorld, normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1)
    /// </summary>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed)
    {
        FollowPathToRandomPoint(normalizedSpeed);
    }

    /// <summary>
    ///Generate new random point
    /// </summary>
    public void GenerateNewRandomWorldPoint()
    {
        GenerateRandomPoint();
    }

    /// <summary>
    /// Stop Tank at current position.
    /// </summary>
    public void TankStop()
    {
        StopTank();
    }

    /// <summary>
    /// Continue Tank movement at last know speed and pointInWorld path.
    /// </summary>
    public void TankGo()
    {
        StartTank();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject)
    /// </summary>
    public void TurretFaceWorldPoint(GameObject pointInWorld)
    {
        FaceTurretToPoint(pointInWorld);
    }

    /// <summary>
    /// Reset turret to forward facing position
    /// </summary>
    public void TurretReset()
    {
        ResetTurret();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject) and fire (has delay).
    /// </summary>
    public void TurretFireAtPoint(GameObject pointInWorld)
    {
        FireAtPoint(pointInWorld);
    }

    /// <summary>
    /// Returns true if the tank is currently in the process of firing.
    /// </summary>
    public bool TankIsFiring()
    {
        return IsFiring;
    }

    /// <summary>
    /// Returns float value of remaining health.
    /// </summary>
    public float TankCurrentHealth
    {
        get
        {
            return GetHealthLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining ammo.
    /// </summary>
    public float TankCurrentAmmo
    {
        get
        {
            return GetAmmoLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining fuel.
    /// </summary>
    public float TankCurrentFuel
    {
        get
        {
            return GetFuelLevel;
        }
    }

    /// <summary>
    /// Returns list of friendly bases.
    /// </summary>
    protected List<GameObject> MyBases
    {
        get
        {
            return GetMyBases;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject target, float distance) of visible targets (tanks in TankMain LayerMask).
    /// </summary>
    protected Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return TanksFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject consumable, float distance) of visible consumables (consumables in Consumable LayerMask).
    /// </summary>
    protected Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return ConsumablesFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject base, float distance) of visible enemy bases (bases in Base LayerMask).
    /// </summary>
    protected Dictionary<GameObject, float> VisibleEnemyBases
    {
        get
        {
            return BasesFound;
        }
    }
}
