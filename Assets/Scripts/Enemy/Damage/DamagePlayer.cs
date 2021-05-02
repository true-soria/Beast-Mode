using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [HideInInspector] public EnemyHP hp;
    [HideInInspector] public EnemyMovement enemyMovement;
    [HideInInspector] public Rigidbody2D body;

    public const float knockBackForce = 8f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                if (hp.isAlive)
                {
                    PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
                    playerHealth.TakeDamage(hp.enemyStats.damage);
                    KnockBackOther(other.gameObject);
                }
                break;
            case "Enemy":
                KnockBackOther(other.gameObject);
                if (enemyMovement.frenzied)
                {
                    EnemyHP enemyHp = other.gameObject.GetComponent<EnemyHP>();
                    enemyHp.TakeDamage(hp.enemyStats.damage);
                    KnockBackOther(other.gameObject);
                }
                break;
        }
    }

    private void KnockBackOther(GameObject other)
    {
        Vector3 force = other.transform.position - transform.position;
        force = force.normalized * (knockBackForce);
        
        Rigidbody2D otherBody = other.gameObject.GetComponentInParent<Rigidbody2D>();
        otherBody.velocity = force;
        body.velocity = -force;
    }
}
