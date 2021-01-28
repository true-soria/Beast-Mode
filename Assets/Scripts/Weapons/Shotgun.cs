using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shotgun : Mask
{
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float pelletFanWidth = 30;

    protected override void Fire()
    {
        Transform aimTransform = weaponAim.mask.transform;
        Vector3 spawnPosition = aimTransform.position;
        Quaternion spawnRotation = aimTransform.rotation;
        if (aimTransform.localPosition.x <= 0)
        {
            spawnRotation *= Quaternion.Euler(0, 180, 0);
        }

        Quaternion[] pelletRotations = new Quaternion[pelletCount];

        for (int i = 0; i < pelletCount; i++)
        {
            float rotation = (2f * i / (pelletCount - 1) - 1) * pelletFanWidth / 2f;
            pelletRotations[i] = Quaternion.Euler(0, 0, rotation) * spawnRotation;
            pelletRotations[i] *= Quaternion.Euler(0, 0, Random.Range(-spread, spread));

            Bullet bullet = Instantiate(bulletPrefab, spawnPosition, pelletRotations[i]);
            bullet.reflectCount = reflectCount + playerEffects.reflectCount;
            bullet.gameObject.transform.localScale *= bulletSize;
            bullet.damage = Mathf.CeilToInt(damage * playerEffects.damageMult);
            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.velocity = (pelletRotations[i] * Vector3.right) * bulletSpeed;
            bulletBody.mass = knockback * playerEffects.knockbackMult;
        }

        WeaponCooldown();
    }
}
