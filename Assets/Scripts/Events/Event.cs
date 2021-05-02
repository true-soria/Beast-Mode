using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Event : MonoBehaviour
{
    public enum EventState
    {
        Inactive = 0,
        Active,
        Complete,
    }
    
    [SerializeField] private Transform cameraFixPoint;
    [SerializeField] private PolygonCollider2D cameraBounds;
    [SerializeField] private Transform playerSpawnStart;
    [SerializeField] private Transform playerSpawnEnd;
    [SerializeField] private GameObject[] barriers;
    
    [SerializeField] private Zone[] enemySpawners;
    [SerializeField] private Zone[] patrolMarkers;
    [SerializeField] private Nest[] nestObjects;
    [SerializeField] private float globalAggression;
    [SerializeField] private float spawnFrequency;
    [SerializeField] private bool startImmediately;
    [SerializeField] private bool waitForEndTrigger;

    private List<Nest> _nests;
    private Transform[][] _sortedPatrolMarkers;
    private GameManager _gameManager;
    private EventState _eventState;
    private int _spawnFails;

    private const int SpawnFailThreshold = 5;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        InitNests();
        SortPatrolMarkers();
    }

    private void Start()
    {
        ActiveBarriers(false);
        DisableAllSpawners();
        if (startImmediately)
            StartEvent();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && _eventState == EventState.Inactive)
        {
            StartEvent();
        }
    }

    public void StartEvent()
    {
        _eventState = EventState.Active;
        ActiveBarriers(true);
        if (_gameManager)
            _gameManager.SetSpawn(playerSpawnStart);
        AdjustCamera();
        
        InvokeRepeating(nameof(SpawnEnemy), 2f, Mathf.Max(1f, spawnFrequency));
    }

    public void EndEvent()
    {
        _eventState = EventState.Complete;
        CancelInvoke();
        ActiveBarriers(false);
        DisableAllSpawners();
        if (_gameManager)
        {
            _gameManager.SetSpawn(playerSpawnEnd);
            _gameManager.ReleaseCamera();
        }
    }

    private void AdjustCamera()
    {
        if (cameraBounds)
            _gameManager.SetCameraBounds(cameraBounds);
        else if (cameraFixPoint)
            _gameManager.FixCameraPosition(cameraFixPoint);
    }

    private void SpawnEnemy()
    {
        if (_nests.Count <= 0)
        {
            if (waitForEndTrigger)
                CancelInvoke(nameof(SpawnEnemy));
            else
                EndEvent();
            return;
        }
        Nest nest = _nests[Random.Range(0, _nests.Count)];
        if (nest.AllSpawnsActive())
            return;
        
        int randomSpawnerIndex = Random.Range(0, enemySpawners.Length);
        Zone zone = null;
        for (int i = 0; i < enemySpawners.Length; i++)
        {
            Zone nextZone = enemySpawners[(i + randomSpawnerIndex) % enemySpawners.Length];
            if (nextZone != null && nextZone.SpawnType == nest.spawnType)
            {
                zone = nextZone;
                break;
            }
        }
        StartCoroutine(ReleaseEnemy(zone, nest));
    }

    private IEnumerator ReleaseEnemy(Zone zone, Nest nest)
    {
        GameObject enemy = nest.NextSpawn();
        if (zone && enemy)
        {
            _spawnFails = 0;
            nest.spawnsLeft--;
            zone.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(spawnFrequency * 0.9f);
            enemy.transform.position = zone.gameObject.transform.position;
            enemy.SetActive(true);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            enemyMovement.globalAggression = globalAggression;
            if (patrolMarkers.Length > 0)
                enemyMovement.patrolMarkers = _sortedPatrolMarkers[(int) zone.SpawnType];
            
            zone.gameObject.SetActive(false);
        }
        else
        {
            _spawnFails++;
            if (_spawnFails > SpawnFailThreshold)
            {
                if (!waitForEndTrigger)
                    EndEvent();
            }
        }
    }
    
    private void ActiveBarriers(bool active)
    {
        foreach (var barrier in barriers)
            barrier.SetActive(active);
    }

    private void DisableAllSpawners()
    {
        foreach (var spawner in enemySpawners)
            spawner.gameObject.SetActive(false);
    }

    private void InitNests()
    {
        _nests = new List<Nest>();
        foreach (var nestObject in nestObjects)
            _nests.Add(Instantiate(nestObject));
    }

    private void SortPatrolMarkers()
    {
        if (patrolMarkers.Length > 0)
        {
            int numOfSpawnTypes = Enum.GetNames(typeof(Nest.SpawnType)).Length; 
            _sortedPatrolMarkers = new Transform[numOfSpawnTypes][];

            List<Transform>[] tempSorted = new List<Transform>[numOfSpawnTypes];
            for (int i = 0; i < tempSorted.Length; i++)
                tempSorted[i] = new List<Transform>();
            
            foreach (var marker in patrolMarkers)
                tempSorted[(int) marker.SpawnType].Add(marker.transform);

            for (int i = 0; i < _sortedPatrolMarkers.Length; i++)
            {
                _sortedPatrolMarkers[i] = new Transform[tempSorted[i].Count];
                _sortedPatrolMarkers[i] = tempSorted[i].ToArray();
            }
        }
    }

    public EventState GetEventState => _eventState;
}
