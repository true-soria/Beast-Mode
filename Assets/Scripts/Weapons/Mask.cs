using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mask : MonoBehaviour
{
    public enum WeaponSlot
    {
        Right1 = 0,
        Right2 = 1,
        Right3 = 2,
        Left1 = 3,
        Left2 = 4,
        Left3 = 5,
        Up1 = 6,
        Up2 = 7,
        Up3 = 8,
        Down1 = 9,
        Down2 = 10,
        Down3 = 11
    }

    public int powerLevel = 10;
    public float knockback = 1;
    public float damage = 1;
    public float spread;
    public float roundsPerSecond = 1;
    public float bulletSpeed = 1;
    public float bulletSize = 1;
    public int reflectCount;
    public Bullet bulletPrefab;
    public PassiveEffect passiveEffect;
    public WeaponSlot slot;
    [HideInInspector] public PlayerEffects playerEffects;
    [HideInInspector] public WeaponAim weaponAim;

    public abstract void Fire();
}
