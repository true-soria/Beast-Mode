using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCombat : Combat
{
    private Animator _animator;
    private runEnemyAI _enemyAI;
    private Rigidbody2D _body;
    
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int IsDead = Animator.StringToHash("isDead");


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _enemyAI = GetComponent<runEnemyAI>();
        _body = GetComponent<Rigidbody2D>();
        enemyStats = Instantiate(baseStats);
        enemyStats.hitPoints = enemyStats.maxHitPoints;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                if (isAlive)
                {
                    PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
                    playerHealth.TakeDamage(enemyStats.damage);                    
                }
                break;
        }
    }

    
    public override void TakeDamage(int damage)
    {
        _animator.SetTrigger(Hit);
        enemyStats.hitPoints -= damage;
        if (enemyStats.hitPoints <= 0)
        {
            isAlive = false;
            _enemyAI.enabled = false;
            _animator.SetBool(IsDead, true);
            DropOnDeath();
            StartCoroutine(Die());
        }
    }
    
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    public override void HealDamage(int heal)
    {
        enemyStats.hitPoints = Math.Min(heal + enemyStats.hitPoints, enemyStats.maxHitPoints);
    }
}
