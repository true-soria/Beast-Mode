using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive Effect")]
public class PassiveEffect : ScriptableObject
{
    public int extraJumps;
    public int extraDashes;
    public float wallClingTime;
    public float floatDuration;
    public bool canWallCling;
}
