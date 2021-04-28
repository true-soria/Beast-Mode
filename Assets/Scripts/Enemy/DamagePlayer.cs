using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [HideInInspector] public EnemyHP hp;

    private void Start()
    {
        hp = gameObject.GetComponentInParent<EnemyHP>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                if (hp.isAlive)
                {
                    PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
                    playerHealth.TakeDamage(hp.enemyStats.damage);                    
                }
                break;
        }
    }
}
