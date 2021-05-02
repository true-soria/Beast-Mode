using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ammo")]
public class Ammo : ScriptableObject
{
    public int maxRightAmmo;
    public int maxLeftAmmo;
    public int maxUpAmmo;
    public int maxDownAmmo;
    [HideInInspector] public int rightAmmo;
    [HideInInspector] public int leftAmmo;
    [HideInInspector] public int upAmmo;
    [HideInInspector] public int downAmmo;

    private void Awake()
    {
        rightAmmo = maxRightAmmo;
        leftAmmo = maxLeftAmmo;
        upAmmo = maxUpAmmo;
        downAmmo = maxDownAmmo;
    }
}
