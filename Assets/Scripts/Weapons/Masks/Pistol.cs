using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pistol : Mask
{

    protected override void Fire()
    {
        var bulletSpawn = BulletPositionAndRotation();
        bulletSpawn.rotation *= BulletSpread();
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        ApplyBulletEffects(bullet);
        WeaponCooldown();
    }
}
