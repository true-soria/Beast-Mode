using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public int targetFPS = 60;
    public Transform playerSpawnPosition;
    [HideInInspector] public CinemachineVirtualCamera cmCamera;
    [HideInInspector] public Camera mainCamera;
    public Collider2D defaultCameraBounds;

    [SerializeField] private GameObject cameraCmPrefab;
    [SerializeField] private GameObject playerPrefab;

    private CinemachineConfiner _cinemachineConfiner;
    private Scene _currentScene;
    private PlayerHealth _playerHealth;
    private GameObject _player;
    private Transform _fixedPosition;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
#endif
        
        if (playerSpawnPosition)
            _player = Instantiate(playerPrefab, playerSpawnPosition.position, Quaternion.identity);
        else
            _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cmCamera = Instantiate(cameraCmPrefab).GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        _cinemachineConfiner = cmCamera.gameObject.GetComponent<CinemachineConfiner>();
        ActionHandler actionHandler = _player.GetComponent<ActionHandler>();
        
        cmCamera.Follow = cmCamera.LookAt = _player.transform;
        _cinemachineConfiner.m_BoundingShape2D = defaultCameraBounds;
        actionHandler.camera = mainCamera;
        _currentScene = SceneManager.GetActiveScene();
    }
    
    void Update()
    {
        if(Application.targetFrameRate != targetFPS)
            Application.targetFrameRate = targetFPS;
    }
    
    

    public static void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(_currentScene.name);
    }
}
