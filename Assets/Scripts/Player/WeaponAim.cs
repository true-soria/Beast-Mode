using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public readonly float eyeDistance = 0.13f;
    
    [HideInInspector] public Vector2 aim;

    private void LateUpdate()
    {
        if (aim != Vector2.zero)
        {
            transform.localPosition = aim * eyeDistance;
            Quaternion eyeRotation;
            
            if (aim.x > 0)
            {
                if (aim.y > 0)
                    eyeRotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, aim));
                else
                    eyeRotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.right, aim));
            }
            else
            {
                if (aim.y > 0)
                    eyeRotation = Quaternion.Euler(0, 180, Vector2.Angle(Vector2.left, aim));
                else
                    eyeRotation = Quaternion.Euler(0, 180, -Vector2.Angle(Vector2.left, aim));
            }
            
            transform.rotation = eyeRotation;
        }
    }
}
