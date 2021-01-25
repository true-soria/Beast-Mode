using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public readonly float _eyeDistance = 0.13f;

    // [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public SpriteRenderer mask;
    [HideInInspector] public Vector2 aim;

    private void Start()
    {
        mask = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (aim != Vector2.zero)
        {
            mask.transform.localPosition = aim * _eyeDistance;
            Quaternion eyeRotation;
            if (aim.x > 0)
            {
                mask.flipX = false;
                if (aim.y > 0)
                    eyeRotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, aim));
                else
                    eyeRotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.right, aim));
            }
            else
            {
                mask.flipX = true;
                if (aim.y > 0)
                    eyeRotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.left, aim));
                else
                    eyeRotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.left, aim));
            }
            
            mask.transform.rotation = eyeRotation;
        }
    }
}
