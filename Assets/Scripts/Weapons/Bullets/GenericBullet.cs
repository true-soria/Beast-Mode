using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBullet : Bullet
{
    private Rigidbody2D _body;
    

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        speed = _body.velocity.magnitude;
        direction = _body.velocity.normalized;

    }

    private void FixedUpdate()
    {
        if (_body.velocity.magnitude < speed)
        {
            _body.velocity = direction * speed;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                EnemyHP enemyHp = other.gameObject.GetComponent<EnemyHP>();
                enemyHp.TakeDamage(damage, direction * knockBack);
                
                if (reflectCount <= 0)
                    Destroy(gameObject);
                else
                {
                    ReflectBulletDirection(other.GetContact(0).normal.normalized);
                    _body.velocity = direction * speed;
                }
                reflectCount--;
                break;
            case "Ceiling":
            case "Wall":
            case "Floor":
                if (reflectCount <= 0)
                    Destroy(gameObject);
                else
                {
                    ReflectBulletDirection(other.GetContact(0).normal.normalized);
                    _body.velocity = direction * speed;
                    reflectCount--;
                }
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }
}
