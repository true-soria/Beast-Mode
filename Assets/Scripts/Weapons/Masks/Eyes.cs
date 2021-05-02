using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : Mask
{
    protected override void Fire()
    {
        Debug.Log("I can only witness...");
        WeaponCooldown();
    }
}