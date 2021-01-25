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
                case PlayerAction.Slam:
                case PlayerAction.DashLeft:
                case PlayerAction.DashRight:
                case PlayerAction.Fire:
                case PlayerAction.SelectRight:
                case PlayerAction.SelectLeft:
                case PlayerAction.SelectUp:
                case PlayerAction.SelectDown:
                case PlayerAction.Pause:
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        playerManager.movement.move = _lsMove;
        playerManager.equippedMask.weaponAim.aim = _rsMove;
    }
}
