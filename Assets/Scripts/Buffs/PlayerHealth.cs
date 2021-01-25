using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public PlayerEffects playerEffects;
    public GameObject displayHealthPrefab;

    private GameObject _displayHealth;
    private Text _healthText;
    // private CircleCollider2D _hurtbox;
    private bool _iFramesActive;

    [HideInInspector] public bool isAlive = true;

    private void Start()
    {
        // _hurtbox = GetComponentInChildren<CircleCollider2D>();
        if (displayHealthPrefab)
        {
            _displayHealth = Instantiate(displayHealthPrefab);
            _healthText = _displayHealth.GetComponentInChildren<Text>();
            _healthText.text = $"HP: {playerEffects.hitPoints}";
        }
    }

    public void TakeDamage(int damage)
    {
        if (!_iFramesActive)
            playerEffects.hitPoints -= damage;
        if (playerEffects.hitPoints <= 0)
        {
            isAlive = false;
            if (_displayHealth)
                _displayHealth.gameObject.SetActive(false);
        }
        _healthText.text = $"HP: {playerEffects.hitPoints}";

        StartCoroutine(ActivateIFrames(2f));
    }

    public IEnumerator ActivateIFrames(float duration)
    {
        _iFramesActive = true;
        yield return new WaitForSeconds(duration);
        _iFramesActive = false;
    }

    public void HealDamage(int heal)
    {
        playerEffects.hitPoints = Math.Min(heal + playerEffects.hitPoints, playerEffects.effectiveHitPoints);
    }

    public void RecalculateHitPoints(bool setHpToMax)
    {
        float eHP = playerEffects.damageResistance >= 100 ? Mathf.Infinity : 100 / (100 - playerEffects.damageResistance);
        playerEffects.effectiveHitPoints = Mathf.CeilToInt(playerEffects.maxHitPoints * eHP);
        playerEffects.hitPoints = setHpToMax ? playerEffects.effectiveHitPoints : Mathf.CeilToInt(playerEffects.hitPoints * eHP);
        if (_healthText)
            _healthText.text = $"HP: {playerEffects.hitPoints}";
    }
}
