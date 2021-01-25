using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public int targetFPS = 60;

    private Scene _currentScene;
    private GameObject _player;
    private PlayerHealth _playerHealth;
    private EndlessWave _endlessWave;
    private MenuMovement _menuMovement;

    void Awake()
    {
        _endlessWave = GetComponent<EndlessWave>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _menuMovement = GetComponent<MenuMovement>();
        // _menuMovement.enabled = false;
        // StartCoroutine(Test());

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
#endif
    }

    private void Start()
    {
        _currentScene = SceneManager.GetActiveScene();
        _playerHealth = _player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if(Application.targetFrameRate != targetFPS)
            Application.targetFrameRate = targetFPS;
        // if (!_playerHealth)
        // {
        //     _playerHealth = _player.GetComponent<PlayerHealth>();
        // }
        if (!_playerHealth.isAlive)
        {
            GameOver();
            _playerHealth.isAlive = true;
        }
            
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(5f);
        GameOver();
    }

    public void GameOver()
    {
        _player.SetActive(false);
        _endlessWave.GameOver();
        // _menuMovement.enabled = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(_currentScene.name);
    }
}
