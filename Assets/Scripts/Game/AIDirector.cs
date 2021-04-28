using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    [SerializeField] private Transform hiddenSpawner;
    [SerializeField] private float globalAggression;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int spawnsPerEnemy;
    private List<GameObject>[] _pooledEnemies;
    private List<Transform> _targets;


    private void Awake()
    {
        _pooledEnemies = new List<GameObject>[enemyPrefabs.Length];
        _targets = new List<Transform>();
        int enemyIndex = 0;
        foreach (var enemyPrefab in enemyPrefabs)
        {
            _pooledEnemies[enemyIndex] = new List<GameObject>();
            for (int i = 0; i < spawnsPerEnemy; i++)
            {
                _pooledEnemies[enemyIndex].Add(Instantiate(enemyPrefab));
            }
            enemyIndex++;
        }
    }

    private void Start()
    {
        foreach (var list in _pooledEnemies)
        {
            foreach (var enemy in list)
            {
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (enemyMovement)
                {
                    enemyMovement.target = transform;
                    enemyMovement.globalAggression = globalAggression;
                }
            }
        }
        
    }

    public static void AddTarget()
    {
    }
}
