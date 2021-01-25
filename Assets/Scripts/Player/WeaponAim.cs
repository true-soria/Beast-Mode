using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [HideInInspector] public PlayerMovement movement;
    
    public readonly float _eyeDistance = 0.13f;

    [HideInInspector] public SpriteRenderer mask;

    private void Start()
    {
        mask = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (movement.rsMove != Vector2.zero)
        {
            mask.transform.localPosition = movement.rsMove * _eyeDistance;
            Quaternion eyeRotation;
            if (movement.rsMove.x > 0)
            {
                mask.flipX = false;
                if (movement.rsMove.y > 0)
                    eyeRotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, movement.rsMove));
                else
                    eyeRotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.right, movement.rsMove));
            }
            else
            {
                mask.flipX = true;
                if (movement.rsMove.y > 0)
                    eyeRotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.left, movement.rsMove));
                else
                    eyeRotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.left, movement.rsMove));
            }
            
            mask.transform.rotation = eyeRotation;
        }
    }
}
