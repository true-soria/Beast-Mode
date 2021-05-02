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
        var bulletSpawn = BulletPositionAndRotation();
        Quaternion[] pelletRotations = new Quaternion[pelletCount];

        for (int i = 0; i < pelletCount; i++)
        {
            float rotation = (2f * i / (pelletCount - 1) - 1) * pelletFanWidth / 2f;
            pelletRotations[i] = Quaternion.Euler(0, 0, rotation) * bulletSpawn.rotation;
            pelletRotations[i] *= BulletSpread();
            
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawn.position, pelletRotations[i]);
            ApplyBulletEffects(bullet);
        }

        WeaponCooldown();
    }
}
