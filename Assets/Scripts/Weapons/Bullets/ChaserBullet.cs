using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;

public class ChaserBullet : Bullet
{
    private Rigidbody2D _body;
    private Transform _target;
    private IAstarAI _astarAI;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        speed = _body.velocity.magnitude;
        direction = _body.velocity.normalized;
        _astarAI = GetComponent<IAstarAI>();
        _astarAI.maxSpeed = speed * 1.5f;
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
            direction = transform.rotation * Vector3.right;
            _astarAI.destination = _target.position;
        }
        if (_body.velocity.magnitude < speed)
        {
            _body.velocity = direction * speed;
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
                enemyHp.TakeDamage(damage, direction * knockBack);

                if (reflectCount <= 0)
                    Destroy(gameObject);
                else
                {
                    ReflectBulletDirection(other.GetContact(0).normal.normalized);
                    _body.velocity = direction * speed;
                    _body.AddForce(direction * speed * 2f, ForceMode2D.Impulse);
                    reflectCount--;
                }
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
