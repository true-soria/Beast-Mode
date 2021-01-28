using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zantetsu : Mask
{
    [SerializeField] private float spawnDistance;
    [SerializeField] private float lifetime;
    
    protected override void Fire()
    {
        Transform aimTransform = weaponAim.mask.transform;
        Vector3 spawnPosition = aimTransform.position;
        Quaternion spawnRotation = aimTransform.rotation;
        if (aimTransform.localPosition.x <= 0)
        {
            spawnRotation *= Quaternion.Euler(0, 180, 0);
        }
        
        Vector3 spawnOffset = spawnRotation * Vector3.right * spawnDistance;

        Bullet bullet = Instantiate(bulletPrefab, spawnPosition + spawnOffset, spawnRotation);
        bullet.gameObject.transform.localScale *= bulletSize;
        bullet.damage = Mathf.CeilToInt(damage * playerEffects.damageMult);
        bullet.lifetime = lifetime;
        
        Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
        bulletBody.velocity = (spawnRotation * Vector3.right) * bulletSpeed;
        bulletBody.mass = knockback * playerEffects.knockbackMult;
        WeaponCooldown();
    }
}
