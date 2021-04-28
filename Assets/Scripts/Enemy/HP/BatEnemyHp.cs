using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BatEnemyHp : EnemyHP
{
    private Rigidbody2D _body;

    protected override void InitStats()
    {
        base.InitStats();
        _body = GetComponent<Rigidbody2D>();
        _body.gravityScale = 0;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (enemyStats.hitPoints <= 0)
        {
            _body.gravityScale = 1;
        }
    }
}
