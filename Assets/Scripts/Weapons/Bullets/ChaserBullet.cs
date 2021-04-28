using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;

public class ChaserBullet : Bullet
{
    private Rigidbody2D _body;
    private float _speed;
    private Vector2 _direction;
    private Transform _target;
    private IAstarAI _astarAI;

    private bool _allowRotation = true;
    private readonly float _rotationTimeDelay = 0.2f;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _speed = _body.velocity.magnitude;
        _direction = _body.velocity.normalized;
        _astarAI = GetComponent<IAstarAI>();
        _astarAI.maxSpeed = _speed * 1.5f;
    }

    private void OnEnable()
    {
        if (_astarAI != null) _astarAI.onSearchPath += FixedUpdate;
    }

    private void OnDisable()
    {
        if (_astarAI != null) _astarAI.onSearchPath -= FixedUpdate;
        _target = null;
    }

    private void FixedUpdate()
    {
        if (_target != null && _astarAI != null)
        {
            _direction = transform.rotation * Vector3.right;
            _astarAI.destination = _target.position;
        }
        if (_body.velocity.magnitude < _speed)
        {
            _body.velocity = _direction * _speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            _target = other.gameObject.transform;
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
