using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pistol : Mask
{
    private bool _onCooldown;
    private float _shotCooldownTime = 0;

    
    private void OnEnable()
    {
        _onCooldown = false;
    }

    private void Update()
    {
        if (_onCooldown)
        {
            _shotCooldownTime -= Time.deltaTime;
            if (_shotCooldownTime < 0)
                _onCooldown = false;
        }
        else
        {
            if (weaponAim.movement.triggerHeld)
                Fire();
        }
    }

    public override void Fire()
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

        _onCooldown = true;
        _shotCooldownTime = 1f / (roundsPerSecond * playerEffects.fireRateMult);
    }
}
