using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZantetsuSlash : Bullet
{
    private float _tick = 0;
    private HashSet<EnemyHP> _enemies = new HashSet<EnemyHP>();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            _enemies.Add(other.gameObject.GetComponent<EnemyHP>());
        }
    }

    private void FixedUpdate()
    {
        if (_tick <= 0)
        {
            foreach (EnemyHP enemy in _enemies)
            {
                if (enemy)
                    enemy.TakeDamage(damage);
            }
            _enemies = new HashSet<EnemyHP>();
            _tick = TickRate;
        }
        
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }

        _tick -= Time.deltaTime;
        lifetime -= Time.deltaTime;
    }
}
