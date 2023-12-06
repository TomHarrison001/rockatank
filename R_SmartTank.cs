using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class R_SmartTank : AITank
{
    // minimum no. of state machines (3): Search, Chase, Attack
    // obstacles stun the tank
    // consumables: hp +25, ammo +3, fuel +30
    // projectile: 15 dmg

    // methods to use:
    // FindPathToPoint - A* Search
    // FollowPathToPoint - includes FindPathToPoint
    // FollowPathToRandomPoint
    // GenerateRandomPoint
    // StopTank
    // StartTank
    // FaceTurretToPoint
    // ResetTurret
    // FireAtPoint

    // presentation - show testing

    //store ALL currently visible 
    public Dictionary<GameObject, float> enemyTanksFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> enemyBasesFound = new Dictionary<GameObject, float>();

    //store ONE from ALL currently visible
    public GameObject enemyTankPosition;
    public GameObject consumablePosition;
    public GameObject enemyBasePosition;

    // timer
    float t;

    // AITankStart() used instead of Start()
    public override void AITankStart() { }

    // AITankUpdate() in place of Update()
    public override void AITankUpdate()
    {
        // update all currently visible
        enemyTanksFound = VisibleEnemyTanks;
        consumablesFound = VisibleConsumables;
        enemyBasesFound = VisibleEnemyBases;

        // TODO: get health and ammo just to prevent enemies
        // TODO: keep tank 26 away from enemy
        // TODO: circle enemy
        // TODO: fight if ammo > 0, tank.hp > enemyTank.hp, or if hp == hp, hp > 35
        // TODO: fire without delay + stopping?
        // TODO: predictive shooting

        // check for enemy tank found
        enemyTankPosition = (enemyTanksFound.Count > 0) ? enemyTanksFound.First().Key : null;
        // set enemyTankPosition to closest
        if (enemyTanksFound.Count > 1)
        {
            foreach (GameObject enemyTank in enemyTanksFound.Keys)
            {
                if (Vector3.Distance(transform.position, enemyTankPosition.transform.position) > Vector3.Distance(transform.position, enemyTank.transform.position))
                    enemyTankPosition = enemyTank;
            }
        }
        // check for consumables found
        consumablePosition = (consumablesFound.Count > 0) ? consumablesFound.First().Key : null;
        // set consumablePosition to closest
        if (consumablesFound.Count > 1)
        {
            foreach (GameObject consumable in consumablesFound.Keys)
            {
                if (Vector3.Distance(transform.position, consumablePosition.transform.position) > Vector3.Distance(transform.position, consumable.transform.position))
                    enemyTankPosition = consumable;
            }
        }
        // check for enemy bases found
        enemyBasePosition = (enemyBasesFound.Count > 0) ? enemyBasesFound.First().Key : null;
        // set enemyBasePosition to closest
        if (enemyBasesFound.Count > 1)
        {
            foreach (GameObject enemyBase in enemyBasesFound.Keys)
            {
                if (Vector3.Distance(transform.position, enemyBasePosition.transform.position) > Vector3.Distance(transform.position, enemyBase.transform.position))
                    enemyBasePosition = enemyBase;
            }
        }
        // if tank in range, attack or fall back
        if (enemyTankPosition != null)
        {
            // move and shoot
            if (Vector3.Distance(transform.position, enemyTankPosition.transform.position) < 25f)
                Shoot();
            else if (TankCurrentHealth <= 10) Fallback();
            else Follow();
        }
        else if (consumablePosition != null)
        {
            Consume();
        }
        else if (enemyBasePosition != null)
        {
            AttackBase();
        }
        else
        {
            Random();
        }
    }

    // State Machines
    private void Shoot()
    {
        // TODO: only shoot when aimed
        // TODO: find shooting distance and replace in if statement
        // get closer to target and fire if in range
        TurretFireAtPoint(enemyTankPosition);
    }

    private void Follow()
    {
        FollowPathToWorldPoint(enemyTankPosition, 1f);
    }

    private void Fallback() { }

    private void Consume()
    {
        FollowPathToWorldPoint(consumablePosition, 1f);
        t += Time.deltaTime;
        // TODO: decrease t before generating point
        if (t > 10)
        {
            GenerateRandomPoint();
            t = 0;
        }
    }

    private void AttackBase()
    {
        // go close to it and fire
        if (Vector3.Distance(transform.position, enemyBasePosition.transform.position) < 25f)
            TurretFireAtPoint(enemyBasePosition);
        else
            FollowPathToWorldPoint(enemyBasePosition, 1f);
    }

    private void Defend() { }

    private void Random()
    {
        // searching
        enemyTankPosition = null;
        consumablePosition = null;
        enemyBasePosition = null;
        FollowPathToRandomWorldPoint(1f);
        t += Time.deltaTime;
        // TODO: decrease t before generating point
        if (t > 10)
        {
            GenerateNewRandomWorldPoint();
            t = 0;
        }
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