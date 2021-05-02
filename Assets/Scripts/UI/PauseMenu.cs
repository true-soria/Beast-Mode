using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject quitMenu;
    [SerializeField] private GameObject settingsMenu;

    [HideInInspector] public PlayerInput playerInput;
    private string _currentActionMap;
    private const string MenuActionMap = "Menus";


    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
    }

    private void OnEnable()
    {
        if (playerInput)
        {
            _currentActionMap = playerInput.currentActionMap.name;
            playerInput.SwitchCurrentActionMap("Menus");
        }
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        continueButton.Select();
        continueButton.OnSelect(null);
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        if (playerInput && (_currentActionMap != null))
            playerInput.SwitchCurrentActionMap(_currentActionMap);
    }

    public void ClosePauseMenu()
    {
        gameObject.SetActive(false);
    }

    public void OpenControls()
    {
        controlsMenu.SetActive(true);
    }
    
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void OpenQuitMenu()
    {
        quitMenu.SetActive(true);
    }
}
