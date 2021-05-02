using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class Bullet : VersionedMonoBehaviour
{
    public static readonly float TickRate = 0.2f;
    [HideInInspector] public float knockBack;
    [HideInInspector] public int reflectCount;
    [HideInInspector] public int damage;
    [HideInInspector] public float lifetime;
    
    protected float speed;
    protected Vector2 direction;

    protected void ReflectBulletDirection(Vector2 surfaceNormal)
    {
        direction = (direction - 2 * Vector2.Dot(direction, surfaceNormal) * surfaceNormal).normalized;
        float angle = Mathf.Sign(direction.y) * Vector2.Angle(Vector2.right, direction);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
