using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionHandler : MonoBehaviour
{
    public Camera camera;

    private bool _joystickAim;
    private PlayerManager _playerManager;
    private Vector2 _rsMove;

    // NOTE: fire should be released (set to false) on pause)

    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
    }
    
    public void OnMovement(InputAction.CallbackContext value)
    {
        _playerManager.movement.move = value.ReadValue<Vector2>();
    }
    
    public void OnAim(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            _playerManager.equippedMask.weaponAim.SetJoystickCrosshairs(true);
            _rsMove = value.ReadValue<Vector2>().normalized;
            _playerManager.movement.aim = _rsMove;
            _playerManager.equippedMask.weaponAim.aim = _rsMove;
        }
    }

    public void OnFire(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            _playerManager.equippedMask.triggerHeld = true;
            
            Debug.Log("Mouse Position: " +  Mouse.current.position.ReadValue() + "\nPlayerPosition: " + (Vector2) _playerManager.gameObject.transform.position);
        }
        else if (value.canceled)
        {
            _playerManager.equippedMask.triggerHeld = false;
        }
    }
    
    public void OnJump(InputAction.CallbackContext value)
    {
        if(value.started)
            _playerManager.movement.ActionJump();
    }

    public void OnHover(InputAction.CallbackContext value)
    {
        if(value.started)
            _playerManager.movement.ActionStartHover();
        else if (value.canceled)
            _playerManager.movement.ActionStopHover();
    }

    public void OnSlam(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.movement.ActionSlam();
    }
    
    public void OnDashLeft(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.movement.ActionLeftDash();
    }
    
    public void OnDashRight(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.movement.ActionRightDash();
    }
    
    public void OnSelectRight(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.CycleRightMask();
    }
    
    public void OnSelectLeft(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.CycleLeftMask();
    }
    
    public void OnSelectUp(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.CycleUpMask();
    }
    
    public void OnSelectDown(InputAction.CallbackContext value)
    {
        if (value.performed)
            _playerManager.CycleDownMask();
    }
    
    public void OnPause(InputAction.CallbackContext value)
    {
    }

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (mouseDelta != Vector2.zero)
        {
            _playerManager.equippedMask.weaponAim.SetJoystickCrosshairs(false);
            _rsMove = Mouse.current.position.ReadValue() - (Vector2) camera.WorldToScreenPoint(_playerManager.gameObject.transform.position);
            _rsMove = _rsMove.normalized;
            _playerManager.movement.aim = _rsMove;
            _playerManager.equippedMask.weaponAim.aim = _rsMove;
        }
    }
}
