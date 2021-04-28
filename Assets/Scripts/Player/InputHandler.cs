using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public enum PlayerAction
    {
        NoAction = 0,
        Jump ,
        Hover,
        Slam,
        DashLeft,
        DashRight,
        Fire,
        SelectRight,
        SelectLeft,
        SelectUp,
        SelectDown,
        MoveRight,
        MoveLeft,
        MoveUp,
        MoveDown,
        Pause
    }

    public static bool inMenu;
    public static bool usingController;
    public Dictionary<string, PlayerAction> controllerMapping;
    public Dictionary<KeyCode, PlayerAction> keyboardMapping;

    private PlayerManager _playerManager;
    private PlayerControls _controls;
    private Vector2 _lsMove;
    private Vector2 _rsMove;

    private void Awake()
    {
        controllerMapping = new Dictionary<string, PlayerAction>
        {
            {"A", PlayerAction.Jump},
            {"RS", PlayerAction.Jump},
            {"LB", PlayerAction.DashLeft},
            {"RB", PlayerAction.DashRight},
            {"LS", PlayerAction.Slam},
            {"LT", PlayerAction.Hover},
            {"RT", PlayerAction.Fire},
            {"Right", PlayerAction.SelectRight},
            {"Left", PlayerAction.SelectLeft},
            {"Up", PlayerAction.SelectUp},
            {"Down", PlayerAction.SelectDown},
            {"Start", PlayerAction.Pause}
        };
        
        keyboardMapping = new Dictionary<KeyCode, PlayerAction>
        {
            {KeyCode.Space, PlayerAction.Jump},
            {KeyCode.Q, PlayerAction.DashLeft},
            {KeyCode.E, PlayerAction.DashRight},
            {KeyCode.LeftControl, PlayerAction.Slam},
            {KeyCode.Mouse1, PlayerAction.Hover},
            {KeyCode.Mouse0, PlayerAction.Fire},
            {KeyCode.Alpha1, PlayerAction.SelectRight},
            {KeyCode.Alpha2, PlayerAction.SelectLeft},
            {KeyCode.Alpha3, PlayerAction.SelectUp},
            {KeyCode.Alpha4, PlayerAction.SelectDown},
            {KeyCode.Escape, PlayerAction.Pause}
        };
        
        _controls = new PlayerControls();
        
        _controls.Gameplay.A.performed += ctx => PerformAction("A");
        _controls.Gameplay.RS.performed += ctx => PerformAction("RS");
        _controls.Gameplay.LB.performed += ctx => PerformAction("LB");
        _controls.Gameplay.RB.performed += ctx => PerformAction("RB");
        _controls.Gameplay.LS.performed += ctx => PerformAction("LS");
        _controls.Gameplay.LT.performed += ctx => PerformAction("LT");
        _controls.Gameplay.RT.performed += ctx => PerformAction("RT");
        _controls.Gameplay.Right.performed += ctx => PerformAction("Right");
        _controls.Gameplay.Left.performed += ctx => PerformAction("Left");
        _controls.Gameplay.Up.performed += ctx => PerformAction("Up");
        _controls.Gameplay.Down.performed += ctx => PerformAction("Down");
        _controls.Gameplay.Start.performed += ctx => PerformAction("Start");
        
        _controls.Gameplay.A.canceled += ctx => CancelAction("A");
        _controls.Gameplay.RS.canceled += ctx => CancelAction("RS");
        _controls.Gameplay.LB.canceled += ctx => CancelAction("LB");
        _controls.Gameplay.RB.canceled += ctx => CancelAction("RB");
        _controls.Gameplay.LS.canceled += ctx => CancelAction("LS");
        _controls.Gameplay.LT.canceled += ctx => CancelAction("LT");
        _controls.Gameplay.RT.canceled += ctx => CancelAction("RT");
        _controls.Gameplay.Right.canceled += ctx => CancelAction("Right");
        _controls.Gameplay.Left.canceled += ctx => CancelAction("Left");
        _controls.Gameplay.Up.canceled += ctx => CancelAction("Up");
        _controls.Gameplay.Down.canceled += ctx => CancelAction("Down");
        _controls.Gameplay.Start.canceled += ctx => CancelAction("Start");
        
        _controls.Gameplay.Move.performed += ctx => _lsMove = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Move.canceled += ctx => _lsMove = Vector2.zero;
        _controls.Gameplay.Aim.performed += ctx => _rsMove = ctx.ReadValue<Vector2>().normalized;
        _controls.Gameplay.Aim.canceled += ctx => _rsMove = Vector2.zero;
    }

    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        _controls.Gameplay.Enable();
    }
    
    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    private void PerformAction(string button, bool fromController = true)
    {
        PerformAction(KeyCode.F15, button, fromController);
    }
    
    private void PerformAction(KeyCode key = KeyCode.F15, string button = "", bool fromController = false)
    {
        if (usingController != fromController)
            ChangeInput();
        
        if (!inMenu)
        {
            PlayerAction action = PlayerAction.NoAction;
            if (usingController)
                controllerMapping.TryGetValue(button, out action);
            else
                keyboardMapping.TryGetValue(key, out action);

            switch (action)
            {
                case PlayerAction.Jump:
                    _playerManager.movement.ActionJump();
                    break;
                case PlayerAction.Hover:
                    _playerManager.movement.ActionStartHover();
                    break;
                case PlayerAction.Slam:
                    _playerManager.movement.ActionSlam();
                    break;
                case PlayerAction.DashLeft:
                    _playerManager.movement.ActionLeftDash();
                    break;
                case PlayerAction.DashRight:
                    _playerManager.movement.ActionRightDash();
                    break;
                case PlayerAction.Fire:
                    _playerManager.equippedMask.triggerHeld = true;
                    break;
                case PlayerAction.SelectRight:
                    _playerManager.CycleRightMask();
                    break;
                case PlayerAction.SelectLeft:
                    _playerManager.CycleLeftMask();
                    break;
                case PlayerAction.SelectUp:
                    _playerManager.CycleUpMask();
                    break;
                case PlayerAction.SelectDown:
                    _playerManager.CycleDownMask();
                    break;
                case PlayerAction.Pause:
                default:
                    break;
            }
        }
    }

    private void CancelAction(string button)
    {
        PlayerAction action = PlayerAction.NoAction;
        controllerMapping.TryGetValue(button, out action);
        switch (action)
        {
            case PlayerAction.Hover:
                _playerManager.movement.ActionStopHover();
                break;
            case PlayerAction.Fire:
                _playerManager.equippedMask.triggerHeld = false;
                break;
        }
    }

    private void ChangeInput()
    {
        usingController = !usingController;
        if (usingController)
            Cursor.visible = false;
        else
            Cursor.visible = true;
        _playerManager.movement.ActionStopHover();
        _playerManager.equippedMask.triggerHeld = false;
    }

    private void Update()
    {
        if (!inMenu)
        {
            if (usingController)
            {
                _playerManager.movement.move = _lsMove;
                _playerManager.movement.aim = _rsMove;
                _playerManager.equippedMask.weaponAim.aim = _rsMove;
            }
            else
            {
                // _playerManager.movement.move = Vector2.up * (Input.GetKey(KeyCode.W) ? 1 : 0) +
                //                                Vector2.up * (Input.GetKey(KeyCode.A) ? 1 : 0) +
                //                                Vector2.up * (Input.GetKey(KeyCode.S) ? 1 : 0) +
                //                                Vector2.up * (Input.GetKey(KeyCode.D) ? 1 : 0);
                // _playerManager.movement.move = _playerManager.movement.move.normalized;
                
                // Vector2 sightVector = Mouse.current.position.ReadValue() - (Vector2) _player.gameObject.transform.position;
                // sightVector = sightVector.normalized;
                // _playerManager.movement.aim = sightVector;
                // _playerManager.equippedMask.weaponAim.aim = sightVector;
            }
        }
        
        if (usingController && (Keyboard.current.anyKey.wasPressedThisFrame))
        {
            ChangeInput();
        }
    }
}
