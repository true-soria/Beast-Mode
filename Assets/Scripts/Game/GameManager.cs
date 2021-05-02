using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public Transform defaultPlayerSpawnPosition;
    [HideInInspector] public CinemachineVirtualCamera cmCamera;
    [HideInInspector] public Camera mainCamera;
    public Collider2D defaultCameraBounds;

    [SerializeField] private GameObject cameraCmPrefab;
    [SerializeField] private GameObject playerPrefab;

    private CinemachineConfiner _cinemachineConfiner;
    private Scene _currentScene;
    private PlayerHealth _playerHealth;
    private ActionHandler _actionHandler;
    private GameObject _player;
    private Transform _fixedCameraPosition;
    private Collider2D _specialCameraBounds;
    private Transform _currentPlayerSpawnPosition;

    private const float TemporaryDamping = 1.5f;

        private void Awake()
    {
        _currentPlayerSpawnPosition = defaultPlayerSpawnPosition;
        if (_currentPlayerSpawnPosition)
            _player = Instantiate(playerPrefab, _currentPlayerSpawnPosition.position, Quaternion.identity);
        else
            _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cmCamera = Instantiate(cameraCmPrefab).GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        _cinemachineConfiner = cmCamera.gameObject.GetComponent<CinemachineConfiner>();
        _actionHandler = _player.GetComponent<ActionHandler>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _playerHealth.gameManager = this;
        
        cmCamera.Follow = _actionHandler.crosshair.transform;
        cmCamera.LookAt = _player.transform;
        _cinemachineConfiner.m_BoundingShape2D = defaultCameraBounds;
        _actionHandler.playerCamera = mainCamera;
        _currentScene = SceneManager.GetActiveScene();
    }

    public void RespawnObject(GameObject item)
    {
        if (_currentPlayerSpawnPosition)
            item.transform.position = _currentPlayerSpawnPosition.position;
    }

    public void RespawnPlayer()
    {
        if (_currentPlayerSpawnPosition)
            _player.transform.position = _currentPlayerSpawnPosition.position;
    }

    public void SetSpawn(Transform spawn)
    {
        _currentPlayerSpawnPosition = spawn;
    }

    public void ResetSpawn()
    {
        _currentPlayerSpawnPosition = defaultPlayerSpawnPosition;
    }

    public void FixCameraPosition(Transform fixedPosition)
    {
        cmCamera.Follow = fixedPosition;
    }

    public void SetCameraBounds(Collider2D bounds)
    {
        StartCoroutine(CameraBoundsSlowIn());
        _cinemachineConfiner.m_BoundingShape2D = bounds;
    }

    private IEnumerator CameraBoundsSlowIn()
    {
        _cinemachineConfiner.m_Damping = TemporaryDamping;
        yield return new WaitForSeconds(TemporaryDamping);
        _cinemachineConfiner.m_Damping = 0f;
    }

    public void ReleaseCamera()
    {
        cmCamera.Follow = _actionHandler.crosshair.transform;
        _cinemachineConfiner.m_BoundingShape2D = defaultCameraBounds;
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
