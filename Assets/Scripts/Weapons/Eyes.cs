using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : Mask
{
    [SerializeField] private float shotFrequency = 0.35f;
    
    private float _shotCooldown = 0;

    private void OnEnable()
    {
        _shotCooldown = 0;
    }

    public override void Fire()
    {
        if (_shotCooldown <= 0)
        {
            Debug.Log("I can only witness...");
            _shotCooldown = shotFrequency * (1f / playerEffects.fireRateMult);
        }
    }
}
