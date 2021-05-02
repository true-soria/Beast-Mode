﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static readonly string ContinueString = "ContinueFromLevel";
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitGameButton;

    [SerializeField] private GameObject controlsMenuPrefab;
    [SerializeField] private GameObject quitMenuPrefab;


    private GameObject _settingsMenu;
    private GameObject _quitMenu;

    private void Awake()
    {
        _settingsMenu = Instantiate(controlsMenuPrefab, transform);
        _settingsMenu.SetActive(false);
        _quitMenu = Instantiate(quitMenuPrefab, transform);
        _quitMenu.SetActive(false);
    }

    private void Start()
    {
        // if (SaveSystem.GetBool("HelloWorld"))
        //     levelSelectButton.interactable = true;
        // if (!string.IsNullOrEmpty(SaveSystem.GetString(ContinueString)))
        //     continueButton.interactable = true;
        
        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        newGameButton.Select();
        newGameButton.OnSelect(null);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("SSH");
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
