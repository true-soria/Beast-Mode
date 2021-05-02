using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nest")]
public class Nest : ScriptableObject
{
    public enum SpawnType
    {
        Ground = 0,
        Air
    }
    
    public GameObject enemyPrefab;
    public SpawnType spawnType;
    public int startingCount;
    public int maxSimultaneousActive;

    [HideInInspector] public int spawnsLeft;
    private Queue<GameObject> _enemies;

    private void Awake()
    {
        _enemies = new Queue<GameObject>();
        spawnsLeft = startingCount;
        for (int i = 0; i < maxSimultaneousActive; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            _enemies.Enqueue(enemy);
        }
    }
    
    public bool AllSpawnsActive()
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.activeSelf)
                return false;
        }

        return true;
    }

    public int SpawnsActive()
    {
        int count = 0;
        foreach (var enemy in _enemies)
        {
            if (enemy.activeSelf)
                count++;
        }

        return count;
    }
    
    public bool SpawnReady()
    {
        return !AllSpawnsActive() && spawnsLeft > 0;
    }

    public GameObject NextSpawn()
    {
        if (SpawnReady())
        {
            for (int i = 0; i < maxSimultaneousActive; i++)
            {
                GameObject enemy = _enemies.Dequeue();
                _enemies.Enqueue(enemy);
                if (!enemy.activeSelf)
                    return enemy;
            }
            SpawnFailReport();
        }
        return null;
    }

    private void SpawnFailReport()
    {
        int spawnCount = SpawnsActive();
        Debug.Log("Spawn Failed!" +
                  "\n\tSpawn Name: " + enemyPrefab +
                  "\n\tActive Count: " + spawnCount + "/" + maxSimultaneousActive +
                  "\n\tSpawns Left: " + spawnsLeft);
    }
}
