using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMovement : MonoBehaviour
{
    private PlayerControls _controls;
    private GameManager _gameManager;
    

    private void Awake()
    {
        _controls = new PlayerControls();
        _gameManager = GetComponent<GameManager>();
        _controls.Gameplay.Select.performed += ctx => _gameManager.ExitGame();
        _controls.Gameplay.Start.performed += ctx => _gameManager.Restart();
    }
}
