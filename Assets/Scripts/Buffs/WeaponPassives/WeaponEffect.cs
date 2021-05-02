using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Effect")]
public class WeaponEffect : ScriptableObject
{
    public int extraJumps;
    public int extraDashes;
    public float wallClingTime;
    public float floatDuration;
    public bool canWallCling;
}
