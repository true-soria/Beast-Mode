using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EndlessWave : MonoBehaviour
{
     public static EndlessWave SharedInstance = null;
     // public EnemySpawner flyerSpawnPrefab;
     // public EnemySpawner walkerSpawnPrefab;
     // public EnemySpawner crawlerSpawnPrefab;
     public float waveDuration = 30f;
     public float waveShrinker = 0.98f;
     public int enemiesPerSpawner = 3;
     public int maxEnemySpawners = 7;
     public int enemySpawnLimit = 15;
     public Canvas prefabCanvas;
     public GameObject gameOverPrefab;

     public float LeftBounds;
     public float RightBounds;
     public float UpperBounds;
     public float LowerBounds;

     // private List<EnemySpawner> _spawners = new List<EnemySpawner>();
     private int _nextSpawnerIndex;
     private int _spawnCount;
     private bool _gameOver;
     private Canvas _canvas;
     private GameObject _gameOverPanel;
     private Text _text;
     private Animator _textAnimator;
     [SerializeField] private float timer;

     
     private int _waveCounter;
     
     private static readonly int TimeInterval = Animator.StringToHash("TimeInterval");
     private static readonly int Over = Animator.StringToHash("GameOver");


     void Awake()
    {
        if (SharedInstance != null && SharedInstance != this)
        {
            Destroy(SharedInstance);
        }
        SharedInstance = this;
        _canvas = Instantiate(prefabCanvas);
        _text = _canvas.GetComponentInChildren<Text>();
        _gameOverPanel = Instantiate(gameOverPrefab, _canvas.transform);
        _gameOverPanel.SetActive(false);
        _textAnimator = _text.GetComponent<Animator>();
        // InitializeSpawners();
    }


     void Update()
     {
         if (!_gameOver)
         {
             timer += Time.deltaTime;
             _text.text = $"{(int) timer / 60}:{timer % 60:00.000}";
             if (timer >= waveDuration * _waveCounter)
             {
                 waveDuration *= waveShrinker;
                 _textAnimator.SetTrigger(TimeInterval);
                 // Spawn();
                 _waveCounter++;
             }
         }
     }

    // private void Spawn()
    // {
    //     _spawnCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    //     
    //     if ( _spawnCount >= enemySpawnLimit)
    //         return;
    //
    //     int index = _nextSpawnerIndex;
    //     EnemySpawner spawner = _spawners[index];
    //     index = (index + 1) % _spawners.Count;
    //     while (spawner.enabled && index != _nextSpawnerIndex)
    //     {
    //         spawner = _spawners[index];
    //         index = (index + 1) % _spawners.Count;
    //     }
    //     if (index == _nextSpawnerIndex)
    //         return;
    //     
    //     spawner.gameObject.SetActive(true);
    //     spawner.enabled = true;
    //     
    //     _nextSpawnerIndex = (_nextSpawnerIndex + 1) % _spawners.Count;
    // }

    // private void InitializeSpawners()
    // {
    //     for (int i = 0; i < maxEnemySpawners; i++)
    //     {
    //         EnemySpawner spawner;
    //         
    //         float positionX = Random.Range(LeftBounds, RightBounds);
    //         float positionY = Random.Range(LowerBounds, UpperBounds);
    //
    //         switch (Random.Range(0, 100))
    //         {
    //             case int n when (n >= 0 && n < 40):
    //                 spawner = Instantiate(flyerSpawnPrefab, new Vector3(positionX, positionY),
    //                     Quaternion.identity);
    //                 spawner.spawnerLife = enemiesPerSpawner;
    //                 spawner.gameObject.SetActive(false);
    //                 spawner.enabled = false;
    //                 _spawners.Add(spawner);
    //                 break;
    //             case int n when (n >= 40 && n < 80):
    //                 spawner = Instantiate(walkerSpawnPrefab, new Vector3(positionX, positionY),
    //                     Quaternion.identity);
    //                 spawner.spawnerLife = enemiesPerSpawner;
    //                 spawner.gameObject.SetActive(false);
    //                 spawner.enabled = false;
    //                 _spawners.Add(spawner);
    //                 break;
    //             case int n when (n >= 80 && n < 100):
    //                 spawner = Instantiate(crawlerSpawnPrefab, new Vector3(RightBounds, positionY),
    //                     Quaternion.identity);
    //                 spawner.spawnerLife = enemiesPerSpawner / 2;
    //                 spawner.gameObject.SetActive(false);
    //                 spawner.enabled = false;
    //                 _spawners.Add(spawner);
    //                 break;
    //         }
    //     }
    // }

    public void GameOver()
    {
        _gameOver = true;
        _gameOverPanel.SetActive(true);
        _textAnimator.SetBool(Over, true);
    }
}
