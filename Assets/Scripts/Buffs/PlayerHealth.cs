using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public PlayerEffects playerEffects;

    private GameObject _displayHealth;
    private Text _healthText;

    private const float IFrameDuration = 2f;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public bool invincible;
    [HideInInspector] public bool isAlive = true;

    private void Start()
    {
        _displayHealth = Resources.Load("Prefabs/UI/DisplayHealth") as GameObject;
        if (_displayHealth)
        {
            _displayHealth = Instantiate(_displayHealth);
            _healthText = _displayHealth.GetComponentInChildren<Text>();
            DisplayHP();
        }
    }

    public void TakeDamage(int damage)
    {
        playerEffects.hitPoints -= invincible ? 0 : damage;
        if (playerEffects.hitPoints <= 0)
        {
            gameManager.RespawnPlayer();
            RecalculateHitPoints(true);
        }
        
        DisplayHP();
        StartCoroutine(ActivateIFrames(IFrameDuration));
    }

    public IEnumerator ActivateIFrames(float duration)
    {
        invincible = true;
        yield return new WaitForSeconds(duration);
        invincible = playerEffects.damageResistance >= 100;
    }

    public void HealDamage(int heal)
    {
        playerEffects.hitPoints = Math.Min(heal + playerEffects.hitPoints, playerEffects.effectiveHitPoints);
        DisplayHP();
    }

    public void RecalculateHitPoints(bool setHpToMax = false)
    {
        if (playerEffects.damageResistance >= 100)
        {
            invincible = true;
            if (setHpToMax) playerEffects.hitPoints = playerEffects.maxHitPoints;
        }
        else
        {
            invincible = false;
            float eHP = 100 / (100 - playerEffects.damageResistance);
            playerEffects.effectiveHitPoints = Mathf.CeilToInt(playerEffects.maxHitPoints * eHP);
            playerEffects.hitPoints =
                setHpToMax ? playerEffects.effectiveHitPoints : Mathf.CeilToInt(playerEffects.hitPoints * eHP);
        }
        DisplayHP();
    }

    private void DisplayHP()
    {
        if (_healthText)
            _healthText.text = invincible ? "HP: ∞" : $"HP: {playerEffects.hitPoints}";
    }
}
