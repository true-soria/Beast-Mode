using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBullet : Bullet
{
    private Rigidbody2D _body;
    private float _speed;
    private Vector2 _direction;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _speed = _body.velocity.magnitude;
        _direction = _body.velocity.normalized;

    }

    private void FixedUpdate()
    {
        if (_body.velocity.magnitude < _speed)
        {
            _body.velocity = _direction * _speed;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                EnemyHP enemyHp = other.gameObject.GetComponent<EnemyHP>();
                enemyHp.TakeDamage(damage);
                
                if (reflectCount <= 0)
                    Destroy(gameObject);
                else
                {
                    Vector2 surfaceNormal = other.GetContact(0).normal.normalized;
                    _direction = (_direction - 2 * Vector2.Dot(_direction, surfaceNormal) * surfaceNormal).normalized;
                    float angle = Mathf.Sign(_direction.y) * Vector2.Angle(Vector2.right, _direction);
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    _body.velocity = _direction * _speed;
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
                    Vector2 surfaceNormal = other.GetContact(0).normal.normalized;
                    _direction = (_direction - 2 * Vector2.Dot(_direction, surfaceNormal) * surfaceNormal).normalized;
                    float angle = Mathf.Sign(_direction.y) * Vector2.Angle(Vector2.right, _direction);
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                    
                    _body.velocity = _direction * _speed;
                }
                reflectCount--;
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }
}
