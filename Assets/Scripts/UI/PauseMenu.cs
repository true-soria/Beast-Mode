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
    [SerializeField] private GameObject settingsMenuPrefab;
    [SerializeField] private GameObject controlsMenuPrefab;
    [SerializeField] private GameObject quitMenuPrefab;
    [SerializeField] private GameObject quitToMenuOnlyPrefab;

    [HideInInspector] public PlayerInput playerInput;
    private string _currentActionMap;
    private GameObject _settingsMenu;
    private GameObject _controlsMenu;
    private GameObject _quitMenu;

    private const string MenuActionMap = "Menus";


    private void Awake()
    {
#if UNITY_WEBGL
        _quitMenu = Instantiate(quitToMenuOnlyPrefab, transform);
        _quitMenu.SetActive(false);
#else
        _quitMenu = Instantiate(quitMenuPrefab, transform);
        _quitMenu.SetActive(false);
#endif
        _settingsMenu = Instantiate(settingsMenuPrefab, transform);
        _settingsMenu.SetActive(false);
        _controlsMenu = Instantiate(controlsMenuPrefab, transform);
        _controlsMenu.SetActive(false);
        
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
    }

    private void OnEnable()
    {
        if (playerInput)
        {
            _currentActionMap = playerInput.currentActionMap.name;
            playerInput.SwitchCurrentActionMap(MenuActionMap);
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
        _controlsMenu.SetActive(true);
    }
    
    public void OpenSettings()
    {
        _settingsMenu.SetActive(true);
    }

    public void OpenQuitMenu()
    {
        _quitMenu.SetActive(true);
    }
}
