
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyHP : MonoBehaviour
{
    public EnemyStats baseStats;
    public EnemyMovement enemyMovement;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public EnemyStats enemyStats;
    
    
    private void Start()
    {
        InitStats();
    }

    protected virtual void InitStats()
    {
        isAlive = true;
        enemyStats = Instantiate(baseStats);
        enemyStats.hitPoints = enemyStats.maxHitPoints;
    }

    public virtual void TakeDamage(int damage)
    {
        enemyStats.hitPoints -= damage;
        if (enemyStats.hitPoints <= 0)
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
