using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum Direction
    {
        Right = 0,
        Left = 1,
        Up = 2,
        Down = 3
    }

    public static readonly int PerSlot = 3;

    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public Mask equippedMask;
    [SerializeField] private Mask eyes;
    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private PlayerEffects baseEffects;
    [SerializeField] private Ammo baseAmmoPool; 
    [SerializeField] private Mask[] maskPrefabs;


    private Mask[,] _masks = new Mask[4,PerSlot];
    private int[] _currentMask = {-1, -1, -1, -1};
    private Dictionary<string, float> _tempBuffs = new Dictionary<string, float>();
    private PlayerEffects _playerEffects;
    private Ammo _playerAmmo;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        _playerHealth = gameObject.AddComponent<PlayerHealth>();
    }

    void Start()
    {
        StartPlayer();

        foreach (Mask mask in maskPrefabs) PickupMask(mask);
        
        _playerEffects.slamEquipped = _currentMask[(int) Direction.Down] + 1;
        _playerHealth.RecalculateHitPoints(true);
    }

    private void StartPlayer()
    {
        equippedMask = Instantiate(eyes, transform, true);
        equippedMask.transform.localPosition = new Vector3(equippedMask.weaponAim.eyeDistance, 0, 0);
        _playerEffects = Instantiate(baseEffects);
        _playerAmmo = Instantiate(baseAmmoPool);
        equippedMask.playerEffects = _playerEffects;
        equippedMask.ammo = _playerAmmo;
        movement.playerEffects = _playerEffects;
        _playerHealth.playerEffects = _playerEffects;
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "PickupBuff":
                PickupBuff(other.gameObject.GetComponent<PickupBuff>());
                other.gameObject.SetActive(false);
                break;
            case "PickupAmmo":
                PickupAmmo(other.gameObject.GetComponent<PickupAmmo>());
                other.gameObject.SetActive(false);
                break;
            case "PickupMask":
                PickupMask(other.gameObject.GetComponent<PickupMask>().mask, true);
                other.gameObject.SetActive(false);
                break;
        }
    }

    private void Update()
    {
        List<string> buffNames = _tempBuffs.Keys.ToList();
        foreach (string name in buffNames)
            _tempBuffs[name] -= Time.deltaTime;

        hurtbox.enabled = !_playerHealth.invincible && !movement.intangible;
    }

    public void CycleRightMask()
    {
        CycleMask((int) Direction.Right);
    }
    
    public void CycleLeftMask()
    {
        CycleMask((int) Direction.Left);
    }
    
    public void CycleUpMask()
    {
        CycleMask((int) Direction.Up);
    }
    
    public void CycleDownMask()
    {
        CycleMask((int) Direction.Down);
        _playerEffects.slamEquipped = _currentMask[(int) Direction.Down] + 1;
    }

    
    private void CycleMask(int direction)
    {
        Vector2 currentAim = equippedMask.weaponAim.aim;
        bool currentFireState = equippedMask.triggerHeld;
        int current = _currentMask[direction];
        if (current < 0)
            return;
        
        if (equippedMask == _masks[direction, current])
        {
            int next = (current + 1) % PerSlot;
            while (next != current)
            {
                if (_masks[direction, next] != null)
                {
                    EquipNewMask((direction, next));
                    return;
                }
                next = (next + 1) % PerSlot;
            }
        }
        else
        {
            EquipNewMask((direction, current));
        }

        equippedMask.weaponAim.aim = currentAim;
        equippedMask.triggerHeld = currentFireState;
    }

    private void EquipNewMask((int direction, int mask) newMaskIndex)
    {
        equippedMask.gameObject.SetActive(false);
        _masks[newMaskIndex.direction, newMaskIndex.mask].gameObject.SetActive(true);
        equippedMask = _masks[newMaskIndex.direction, newMaskIndex.mask];
    }

    private void PickupBuff(PickupBuff pickupBuff)
    {
        if (pickupBuff.permanent)
        {
            ApplyBuff(pickupBuff.effect);
            Destroy(pickupBuff.gameObject);
        }
        else
        {
            Buff tempBuff = Instantiate(pickupBuff.effect);
            if (_tempBuffs.ContainsKey(tempBuff.buffName))
                _tempBuffs[tempBuff.buffName] = pickupBuff.duration - Time.deltaTime;
            else
            {
                ApplyBuff(tempBuff);
                _tempBuffs[tempBuff.buffName] = pickupBuff.duration - Time.deltaTime;
                StartCoroutine(TempBuff(tempBuff, pickupBuff.duration));
            }

            Destroy(pickupBuff.gameObject);
        }
    }

    private void PickupAmmo(PickupAmmo ammo)
    {
        _playerAmmo.rightAmmo = Mathf.Min(_playerAmmo.maxRightAmmo, _playerAmmo.rightAmmo +
            Mathf.CeilToInt(ammo.restoreRightAmmoPercent * _playerAmmo.maxRightAmmo));
        _playerAmmo.leftAmmo = Mathf.Min(_playerAmmo.maxLeftAmmo, _playerAmmo.leftAmmo +
            Mathf.CeilToInt(ammo.restoreLeftAmmoPercent * _playerAmmo.maxLeftAmmo));
        _playerAmmo.upAmmo = Mathf.Min(_playerAmmo.maxUpAmmo, _playerAmmo.upAmmo +
            Mathf.CeilToInt(ammo.restoreUpAmmoPercent * _playerAmmo.maxUpAmmo));
        _playerAmmo.downAmmo = Mathf.Min(_playerAmmo.maxDownAmmo, _playerAmmo.downAmmo +
            Mathf.CeilToInt(ammo.restoreDownAmmoPercent * _playerAmmo.maxDownAmmo));
    }

    private void PickupMask(Mask mask, bool equipImmediately = false)
    {
        Mask spawnedMask = Instantiate(mask, transform, true);

        spawnedMask.transform.localPosition = new Vector3(spawnedMask.weaponAim.eyeDistance, 0, 0);
        spawnedMask.playerEffects = _playerEffects;
        spawnedMask.ammo = _playerAmmo;

        int directionIndex = Convert.ToInt32(spawnedMask.slot) / PerSlot;
        int maskIndex = Convert.ToInt32(spawnedMask.slot) % PerSlot;
            
        if (_masks[directionIndex, maskIndex] == null)
        {
            ApplyMaskEffects(spawnedMask);
            _masks[directionIndex, maskIndex] = spawnedMask;
            _currentMask[directionIndex] = maskIndex;
        }
        else
        {
            if (spawnedMask.powerLevel >= _masks[directionIndex, maskIndex].powerLevel)
            {
                ApplyMaskEffects(spawnedMask);
                RemoveMaskEffects(_masks[directionIndex, maskIndex]);
                _masks[directionIndex, maskIndex] = spawnedMask;
                _currentMask[directionIndex] = maskIndex;
            }
        }
        spawnedMask.gameObject.SetActive(false);
        movement.RestoreJumps();

        if (equipImmediately) EquipNewMask((directionIndex, maskIndex));
    }

    private IEnumerator TempBuff(Buff buff, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (_tempBuffs[buff.buffName] > 0)
            StartCoroutine(TempBuff(buff, _tempBuffs[buff.buffName]));
        else
        {
            ApplyDebuff(buff);
            Destroy(buff);
        }
    }

    private void ApplyBuff(Buff buff)
    {
    _playerEffects.damageResistance += buff.damageResistance;
    _playerEffects.jumpHeightMult += buff.jumpHeightMult;
    _playerEffects.moveSpeedMult += buff.moveSpeedMult;
    _playerEffects.damageMult += buff.damageMult;
    _playerEffects.fireRateMult += buff.fireRateMult;
    _playerEffects.knockbackMult += buff.knockbackMult;
    _playerEffects.reflectCount += buff.reflectCount;
    if (buff.damageResistance > 0)
        _playerHealth.RecalculateHitPoints(true);
    }

    private void ApplyDebuff(Buff buff)
    {
        _playerEffects.damageResistance -= buff.damageResistance;
        _playerEffects.jumpHeightMult -= buff.jumpHeightMult;
        _playerEffects.moveSpeedMult -= buff.moveSpeedMult;
        _playerEffects.damageMult -= buff.damageMult;
        _playerEffects.fireRateMult -= buff.fireRateMult;
        _playerEffects.knockbackMult -= buff.knockbackMult;
        _playerEffects.reflectCount -= buff.reflectCount;
        if (buff.damageResistance > 0)
            _playerHealth.RecalculateHitPoints(true);
    }
    
    private void ApplyMaskEffects(Mask mask)
    {
        if (mask.weaponEffect != null)
        {
            _playerEffects.extraJumps += mask.weaponEffect.extraJumps;
            _playerEffects.extraDashes += mask.weaponEffect.extraDashes;
            _playerEffects.wallClingTime += mask.weaponEffect.wallClingTime;
            _playerEffects.floatDuration += mask.weaponEffect.floatDuration;
            _playerEffects.canWallCling = _playerEffects.canWallCling || mask.weaponEffect.canWallCling;
        }
    }

    private void RemoveMaskEffects(Mask mask)
    {
        if (mask.weaponEffect != null)
        {
            _playerEffects.extraJumps = Mathf.Max(0, _playerEffects.extraJumps - mask.weaponEffect.extraJumps);
            _playerEffects.extraDashes = Mathf.Max(0,_playerEffects.extraDashes - mask.weaponEffect.extraDashes);
            _playerEffects.wallClingTime = Mathf.Max(0f, _playerEffects.wallClingTime - mask.weaponEffect.wallClingTime);
            _playerEffects.floatDuration = Mathf.Max(0f, _playerEffects.floatDuration - mask.weaponEffect.floatDuration);
            _playerEffects.canWallCling = _playerEffects.wallClingTime > 0;
        }
    }
}
