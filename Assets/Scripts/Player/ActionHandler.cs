using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPrefab;

    [HideInInspector] public GameObject crosshair;
    [HideInInspector] public Camera playerCamera;
    
    public readonly float crosshairDistance = 3.13f;

    private PlayerInput _playerInput;
    private PauseMenu _pauseMenu;
    private bool _joystickAim;
    private bool _joystickCrosshairActive;
    private PlayerManager _playerManager;
    private Vector2 _rsMove;
    
    


    // NOTE: fire should be released (set to false) on pause)
    
    private void Awake()
    {
        crosshair = Resources.Load("Prefabs/UI/Crosshair") as GameObject;
        crosshair = Instantiate(crosshair, transform);
        crosshair.SetActive(false);
    }

    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerInput = GetComponent<PlayerInput>();
        LoadPauseMenu();
    }

    private void LoadPauseMenu()
    {
        GameObject pauseGameObject = Instantiate(pauseMenuPrefab);
        _pauseMenu = pauseGameObject.GetComponent<PauseMenu>();
        _pauseMenu.playerInput = _playerInput;
        pauseGameObject.SetActive(false);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        _playerManager.movement.move = value.ReadValue<Vector2>();
    }
    
    public void OnAim(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            SetJoystickCrosshairs(true);
            _rsMove = value.ReadValue<Vector2>().normalized;
            _playerManager.movement.aim = _rsMove;
            _playerManager.equippedMask.weaponAim.aim = _rsMove;
            crosshair.transform.localPosition = _rsMove * crosshairDistance;
        }
    }

    public void OnFire(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            _playerManager.equippedMask.triggerHeld = true;
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
        if (value.performed)
        {
            if (_pauseMenu.gameObject.activeSelf)
            {
                _pauseMenu.ClosePauseMenu();
            }
            else
            {
                _pauseMenu.gameObject.SetActive(true);
                _playerManager.equippedMask.triggerHeld = false;
            }
        }
    }

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (mouseDelta != Vector2.zero)
        {
            SetJoystickCrosshairs(false);
            _rsMove = Mouse.current.position.ReadValue() - (Vector2) playerCamera.WorldToScreenPoint(_playerManager.gameObject.transform.position);
            _rsMove = _rsMove.normalized;
            _playerManager.movement.aim = _rsMove;
            _playerManager.equippedMask.weaponAim.aim = _rsMove;
            crosshair.transform.localPosition = _rsMove * crosshairDistance;
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
