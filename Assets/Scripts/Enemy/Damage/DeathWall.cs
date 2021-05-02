using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    public int damagePenalty;
    public bool respawnsPlayer;

    private GameManager _gameManager;
    
    public const float knockBackForce = 8f;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
                playerHealth.TakeDamage(damagePenalty);
                
                if (respawnsPlayer)
                    _gameManager.RespawnPlayer();
                else
                {
                    // knocks back player
                }
                break;
            case "Pickup":
                _gameManager.RespawnObject(other.gameObject);
                break;
        }
    }
    
    private void KnockBackPlayer(GameObject other)
    {
        Vector3 force = other.transform.position - transform.position;
        force = force.normalized * (knockBackForce);
        
        Rigidbody2D otherBody = other.gameObject.GetComponentInParent<Rigidbody2D>();
        otherBody.velocity = force;
    }
}
