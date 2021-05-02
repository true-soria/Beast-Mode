
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyHP : MonoBehaviour
{
    public EnemyStats baseStats;
    public EnemyMovement enemyMovement;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public EnemyStats enemyStats;
    
    
    private void Awake()
    {
        InitStats();
    }

    private void OnEnable()
    {
        Respawn();
    }

    protected virtual void InitStats()
    {
        enemyStats = Instantiate(baseStats);
    }

    protected virtual void Respawn()
    {
        isAlive = true;
        enemyStats.hitPoints = enemyStats.maxHitPoints;
    }

    public virtual void TakeDamage(int damage, Vector3 force = new Vector3())
    {
        enemyStats.hitPoints -= damage;
        enemyMovement.ReceiveKnockBack(force);
        if (enemyStats.hitPoints <= 0 && isAlive)
        {
            isAlive = false;
            StartCoroutine(enemyMovement.TriggerDeath());
        }
    }
    public virtual void HealDamage(int heal)
    {
        enemyStats.hitPoints = Mathf.Min(heal + enemyStats.hitPoints, enemyStats.maxHitPoints);
    }
}
