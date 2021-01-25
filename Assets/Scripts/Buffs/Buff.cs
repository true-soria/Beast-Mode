using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Buff")]
public class Buff : ScriptableObject
{
    public string name;
    public bool stackable;
    public float damageResistance;
    public float jumpHeightMult;
    public float moveSpeedMult;
    public float damageMult;
    public float fireRateMult;
    public float knockbackMult;
    public int reflectCount;
}
