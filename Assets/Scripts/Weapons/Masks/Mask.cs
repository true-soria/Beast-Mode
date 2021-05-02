using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    
    protected bool onCooldown;
    protected float shotCooldownTime;
    protected GameObject displayAmmo;
    protected Text ammoText;
    
    public int powerLevel = 10;
    public float knockback = 1;
    public float damage = 1;
    public float spread;
    public float roundsPerSecond = 1;
    public float bulletSpeed = 1;
    public float bulletSize = 1;
    public int reflectCount;
    public int ammoPerShot;
    public Bullet bulletPrefab;
    public WeaponEffect weaponEffect;
    public WeaponSlot slot;
    [HideInInspector] public Ammo ammo;
    [HideInInspector] public PlayerEffects playerEffects;
    [HideInInspector] public WeaponAim weaponAim;
    [HideInInspector] public bool triggerHeld;

    private void Awake()
    {
        weaponAim = gameObject.AddComponent<WeaponAim>();
        displayAmmo = Resources.Load("Prefabs/UI/DisplayAmmo") as GameObject;
        if (displayAmmo)
        {
            displayAmmo = Instantiate(displayAmmo);
            ammoText = displayAmmo.GetComponentInChildren<Text>();
        }
    }

    private void OnEnable()
    {
        onCooldown = false;
        displayAmmo.SetActive(true);
        UpdateAmmoDisplay();
    }

    private void OnDisable()
    {
        triggerHeld = false;
        displayAmmo.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (onCooldown)
        {
            shotCooldownTime -= Time.deltaTime;
            if (shotCooldownTime < 0)
                onCooldown = false;
        }
        else
        {
            if (triggerHeld && HasAmmo())
                Fire();
        }
    }

    protected abstract void Fire();

    protected virtual (Vector3 position, Quaternion rotation) BulletPositionAndRotation()
    {
        Transform aimTransform = weaponAim.transform;
        Vector3 spawnPosition = aimTransform.position;
        Quaternion spawnRotation = aimTransform.rotation;
        if (aimTransform.localPosition.x <= 0)
        {
            spawnRotation *= Quaternion.Euler(0, 180, 180);
        }
        return (spawnPosition, spawnRotation);
    }

    protected virtual Quaternion BulletSpread()
    {
        return Quaternion.Euler(0, 0, Random.Range(-spread, spread));
    }

    protected virtual void ApplyBulletEffects(Bullet bullet)
    {
        bullet.damage = Mathf.CeilToInt(damage * playerEffects.damageMult);
        bullet.reflectCount = reflectCount + playerEffects.reflectCount;
        bullet.gameObject.transform.localScale *= bulletSize;
        bullet.knockBack = knockback * playerEffects.knockbackMult;
        Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
        if (bulletBody)
        {
            bulletBody.velocity = (bullet.transform.rotation * Vector3.right) * bulletSpeed;
            bulletBody.mass = 0;
        }
    }

    protected int RemainingAmmo()
    {
        switch (slot)
        {
            case WeaponSlot.Right1:
            case WeaponSlot.Right2:
            case WeaponSlot.Right3:
                return ammo.rightAmmo;
            case WeaponSlot.Left1:
            case WeaponSlot.Left2:
            case WeaponSlot.Left3:
                return ammo.leftAmmo;
            case WeaponSlot.Up1:
            case WeaponSlot.Up2:
            case WeaponSlot.Up3:
                return ammo.upAmmo;
            case WeaponSlot.Down1:
            case WeaponSlot.Down2:
            case WeaponSlot.Down3:
                return ammo.downAmmo;
        }
        return 0;
    }

    protected bool HasAmmo()
    {
        return RemainingAmmo() > 0;
    }

    protected void WeaponCooldown()
    {
        onCooldown = true;
        shotCooldownTime = 1f / (roundsPerSecond * playerEffects.fireRateMult);
        switch (slot)
        {
            case WeaponSlot.Right1:
            case WeaponSlot.Right2:
            case WeaponSlot.Right3:
                ammo.rightAmmo = Mathf.Max(0, ammo.rightAmmo - ammoPerShot);
                break;
            case WeaponSlot.Left1:
            case WeaponSlot.Left2:
            case WeaponSlot.Left3:
                ammo.leftAmmo = Mathf.Max(0, ammo.leftAmmo - ammoPerShot);
                break;
            case WeaponSlot.Up1:
            case WeaponSlot.Up2:
            case WeaponSlot.Up3:
                ammo.upAmmo = Mathf.Max(0, ammo.upAmmo - ammoPerShot);
                break;
            case WeaponSlot.Down1:
            case WeaponSlot.Down2:
            case WeaponSlot.Down3:
                ammo.downAmmo = Mathf.Max(0, ammo.downAmmo - ammoPerShot);
                break;
        }
        UpdateAmmoDisplay();
    }

    public void UpdateAmmoDisplay()
    {
        if (ammoText)
            ammoText.text = ammoPerShot <= 0 ? "Ammo: ∞" : $"Ammo: {RemainingAmmo()}";
    }
}
