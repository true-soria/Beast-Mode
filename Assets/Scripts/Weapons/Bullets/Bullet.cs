using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class Bullet : VersionedMonoBehaviour
{
    public static readonly float TickRate = 0.2f; 
    [HideInInspector] public int reflectCount;
    [HideInInspector] public int damage;
    [HideInInspector] public float lifetime;
}
