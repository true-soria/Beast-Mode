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
        Transform aimTransform = weaponAim.mask.transform;
        Vector3 spawnPosition = aimTransform.position;
        Quaternion spawnRotation = aimTransform.rotation;
        if (aimTransform.localPosition.x <= 0)
        {
            spawnRotation *= Quaternion.Euler(0, 180, 0);
        }
        spawnRotation *= Quaternion.Euler(0, 0, Random.Range(-spread, spread));

        Bullet bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);
        bullet.damage = Mathf.CeilToInt(damage * playerEffects.damageMult);
        bullet.reflectCount = reflectCount + playerEffects.reflectCount;
        bullet.gameObject.transform.localScale *= bulletSize;
        Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
        bulletBody.velocity = (spawnRotation * Vector3.right) * bulletSpeed;
        bulletBody.mass = knockback * playerEffects.knockbackMult;
        WeaponCooldown();
    }
}
