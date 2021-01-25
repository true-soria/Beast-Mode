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

    public WeaponAim eyes;
    public PlayerMovement movement;
    public Mask equippedMask;
    [SerializeField] private PlayerEffects baseEffects;
    [SerializeField] private Mask[] maskPrefabs;
    [SerializeField] private GameObject displayHealthPrefab;


    private Mask[,] _masks = new Mask[4,PerSlot];
    private int[] _currentMask = {-1, -1, -1, -1};
    private Dictionary<string, float> _tempBuffs = new Dictionary<string, float>();
    private PlayerEffects _playerEffects;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        equippedMask = eyes.gameObject.GetComponent<Mask>();
    }

    void Start()
    {
        _playerEffects = Instantiate(baseEffects);
        movement.playerEffects = _playerEffects;
        _playerHealth = gameObject.AddComponent<PlayerHealth>();
        _playerHealth.displayHealthPrefab = displayHealthPrefab;
        _playerHealth.playerEffects = _playerEffects;

        foreach (Mask mask in maskPrefabs)
        {
            Mask spawnedMask = Instantiate(mask, transform, true);
            WeaponAim maskAim = spawnedMask.GetComponent<WeaponAim>();
            
            spawnedMask.transform.localPosition = new Vector3(maskAim._eyeDistance, 0, 0);
            spawnedMask.weaponAim = maskAim;
            spawnedMask.playerEffects = _playerEffects;
            
            // TODO function
            if (spawnedMask.passiveEffect != null)
            {
                _playerEffects.extraJumps += spawnedMask.passiveEffect.extraJumps;
                _playerEffects.extraDashes += spawnedMask.passiveEffect.extraDashes;
                _playerEffects.wallClingTime += spawnedMask.passiveEffect.wallClingTime;
                _playerEffects.floatDuration += spawnedMask.passiveEffect.floatDuration;
                _playerEffects.canWallCling = _playerEffects.canWallCling || spawnedMask.passiveEffect.canWallCling;
            }

            int directionIndex = Convert.ToInt32(spawnedMask.slot) / PerSlot;
            int maskIndex = Convert.ToInt32(spawnedMask.slot) % PerSlot;
            if (_masks[directionIndex, maskIndex] == null)
            {
                _masks[directionIndex, maskIndex] = spawnedMask;
                _currentMask[directionIndex] = maskIndex;
            }
            else
            {
                if (spawnedMask.powerLevel > _masks[directionIndex, maskIndex].powerLevel)
                {
                    _masks[directionIndex, maskIndex] = spawnedMask;
                    _currentMask[directionIndex] = maskIndex;
                }
            }
            spawnedMask.gameObject.SetActive(false);
        }

        _playerHealth.RecalculateHitPoints(true);
        RecalculateSlamEquipped();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Pickup":
                Debug.Log("Collided");
                Pickup(other.gameObject.GetComponent<Pickup>());
                break;
        }
    }

    private void Update()
    {
        List<string> buffNames = _tempBuffs.Keys.ToList();
        foreach (string name in buffNames)
        {
            _tempBuffs[name] -= Time.deltaTime;
        }
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
        if (_playerEffects.slamPower > 0)
            _playerEffects.slamEquipped = _playerEffects.slamEquipped % _playerEffects.slamPower + 1;
        CycleMask((int) Direction.Down);
    }

    
    private void CycleMask(int direction)
    {
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
    }

    private void EquipNewMask((int, int) newMaskIndex)
    {
        equippedMask.gameObject.SetActive(false);
        _masks[newMaskIndex.Item1, newMaskIndex.Item2].gameObject.SetActive(true);
        equippedMask = _masks[newMaskIndex.Item1, newMaskIndex.Item2];
    }

    private void RecalculateSlamEquipped()
    {
        
        if (_currentMask[(int) Direction.Down] >= 0)
        {
            int slamEquipped = 0;
            for (int i = 0; i < _currentMask[(int) Direction.Down] ; i++)
            {
                if (_masks[(int) Direction.Down, i] != null)
                    slamEquipped++;
            }
            _playerEffects.slamEquipped = slamEquipped;
        }
    }

    public void Pickup(Pickup pickup)
    {
        if (pickup.permanent)
        {
            ApplyBuff(pickup.effect);
            Destroy(pickup.gameObject);
        }
        else
        {
            Buff tempBuff = Instantiate(pickup.effect);
            if (_tempBuffs.ContainsKey(tempBuff.name))
                _tempBuffs[tempBuff.name] = pickup.duration - Time.deltaTime;
            else
            {
                ApplyBuff(tempBuff);
                _tempBuffs[tempBuff.name] = pickup.duration - Time.deltaTime;
                StartCoroutine(TempBuff(tempBuff, pickup.duration));
            }

            Destroy(pickup.gameObject);
            
        }
    }

    public IEnumerator TempBuff(Buff buff, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (_tempBuffs[buff.name] > 0)
            StartCoroutine(TempBuff(buff, _tempBuffs[buff.name]));
        else
        {
            ApplyDebuff(buff);
            Destroy(buff);
        }
    }


    public void ApplyBuff(Buff buff)
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

    public void ApplyDebuff(Buff buff)
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
}
