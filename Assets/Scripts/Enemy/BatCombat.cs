using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BatCombat : Combat
{
    private Animator _animator;
    // private EnemyAI _enemyAI;
    private Rigidbody2D _body;
    private static readonly int PlayDead = Animator.StringToHash("PlayDead");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        // _enemyAI = GetComponent<EnemyAI>();
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
            case "Platform":
            case "Floor":
                if (!isAlive)
                    _animator.SetTrigger(PlayDead);
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
            // _enemyAI.enabled = false;
            _body.gravityScale = 1;
            _animator.SetBool(Dead, true);
            DropOnDeath();
            StartCoroutine(Die());
        }
    }

    public override void HealDamage(int heal)
    {
        enemyStats.hitPoints = Math.Min(heal + enemyStats.hitPoints, enemyStats.maxHitPoints);
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
}
