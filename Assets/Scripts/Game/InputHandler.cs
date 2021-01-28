using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Pause
    }

    public bool inMenu;
    public Dictionary<string, PlayerAction> controlMapping;

    [SerializeField] private PlayerManager playerManager;
    private PlayerControls _controls;
    private Vector2 _lsMove;
    private Vector2 _rsMove;

    private void Awake()
    {
        controlMapping = new Dictionary<string, PlayerAction>
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

    private void OnEnable()
    {
        _controls.Gameplay.Enable();
    }
    
    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }
    
    private void PerformAction(string button)
    {
        if (inMenu)
        {
            return;
        }
        else
        {
            PlayerAction action = PlayerAction.NoAction;
            controlMapping.TryGetValue(button, out action);
            switch (action)
            {
                case PlayerAction.Jump:
                    playerManager.movement.ActionJump();
                    break;
                case PlayerAction.Hover:
                    playerManager.movement.ActionStartHover();
                    break;
                case PlayerAction.Slam:
                    playerManager.movement.ActionSlam();
                    break;
                case PlayerAction.DashLeft:
                    playerManager.movement.ActionLeftDash();
                    break;
                case PlayerAction.DashRight:
                    playerManager.movement.ActionRightDash();
                    break;
                case PlayerAction.Fire:
                    playerManager.equippedMask.triggerHeld = true;
                    break;
                case PlayerAction.SelectRight:
                    playerManager.CycleRightMask();
                    break;
                case PlayerAction.SelectLeft:
                    playerManager.CycleLeftMask();
                    break;
                case PlayerAction.SelectUp:
                    playerManager.CycleUpMask();
                    break;
                case PlayerAction.SelectDown:
                    playerManager.CycleDownMask();
                    break;
                case PlayerAction.Pause:
                default:
                    break;
            }
        }
    }
    
    private void CancelAction(string button)
    {
        if (inMenu)
        {
            return;
        }
        else
        {
            PlayerAction action = PlayerAction.NoAction;
            controlMapping.TryGetValue(button, out action);
            switch (action)
            {
                case PlayerAction.Hover:
                    playerManager.movement.ActionStopHover();
                    break;
                case PlayerAction.Fire:
                    playerManager.equippedMask.triggerHeld = false;
                    break;
            }
        }
    }

    private void Update()
    {
        playerManager.movement.move = _lsMove;
        playerManager.movement.aim = _rsMove;
        playerManager.equippedMask.weaponAim.aim = _rsMove;
    }
}
