using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public readonly float eyeDistance = 0.13f;
    public readonly float crosshairDistance = 2.13f;
    public GameObject crosshair;

    public bool lockCrosshairRotation = true;
    
    [HideInInspector] public SpriteRenderer mask;
    [HideInInspector] public Vector2 aim;

    private bool _joystickCrosshairActive;

    private void Awake()
    {
        mask = GetComponent<SpriteRenderer>();
        crosshair = Resources.Load("Prefabs/UI/Crosshair") as GameObject;
        crosshair = Instantiate(crosshair, transform);
        crosshair.SetActive(false);
    }

    private void LateUpdate()
    {
        if (aim != Vector2.zero)
        {
            mask.transform.localPosition = aim * eyeDistance;
            crosshair.transform.localPosition = new Vector2(crosshairDistance, 0);
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
                crosshair.transform.localPosition = new Vector2(-crosshairDistance, 0);
                if (aim.y > 0)
                    eyeRotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.left, aim));
                else
                    eyeRotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.left, aim));
            }

            if (lockCrosshairRotation)
                crosshair.transform.rotation = quaternion.identity;
            mask.transform.rotation = eyeRotation;
        }
    }

    public void SetJoystickCrosshairs(bool usingJoystick)
    {
        if (_joystickCrosshairActive != usingJoystick)
        {
            crosshair.SetActive(usingJoystick);
            Cursor.visible = !usingJoystick;
            _joystickCrosshairActive = usingJoystick;
        }
    }
}
