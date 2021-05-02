using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zantetsu : Mask
{
    [SerializeField] private float spawnDistance;
    [SerializeField] private float lifetime;
    
    protected override void Fire()
    {
        var bulletSpawn = BulletPositionAndRotation();
        
        Vector3 spawnOffset = bulletSpawn.rotation * Vector3.right * spawnDistance;

        Bullet bullet = Instantiate(bulletPrefab, bulletSpawn.position + spawnOffset, bulletSpawn.rotation);
        bullet.lifetime = lifetime;
        ApplyBulletEffects(bullet);
        WeaponCooldown();
    }
}
