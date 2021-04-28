using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class EnemyMovement : VersionedMonoBehaviour
{
    public EnemyHP hp;
    public Collider2D damageBox;
    public DamagePlayer damage;
    public GameObject itemDrop;
    public float corpseDecayTime;
    public Vector3[] guardPositions;
    public float maxWanderDistance;

    public enum AttackState
    {
        Stopped = 0,
        Wandering,
        Attacking,
        Following,
        Frenzy
    }
    
    [HideInInspector] public float globalAggression = 0.5f;
    [HideInInspector] public bool stateUnlocked = true;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public Transform target;
    
    [SerializeField] protected float baseAggression = 0.5f;
    private void Start()
    {
        InitComponents();
        FindNewTarget();
    }

    private void OnEnable()
    {
        RestartComponents();
    }

    private void OnDisable()
    {
        StopComponents();
    }

    protected virtual void InitComponents()
    {
        damage.hp = hp;
        hp.enemyMovement = this;
    }

    protected virtual void RestartComponents()
    {
        FindNewTarget();
    }

    protected virtual void StopComponents()
    {
    }

    public void FindNewTarget(Transform forcedTarget = null)
    {
        if (forcedTarget)
            target = forcedTarget;
        else
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            if (players.Length > 1)
            {
                target = FindClosestTransform(players);
            }
            else if (players.Length == 1)
            {
                target = players[0].transform;
            }
            else
            {
                FrenzyActive(true);
            }
        }
    }

    protected virtual Transform FindClosestTransform(GameObject[] objects)
    {
        float closestDistance = Mathf.Infinity;
        Transform closest = null;
        foreach (var obj in objects)
        {
            float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
            if (!(distance < closestDistance)) continue;
            closestDistance = distance;
            closest = obj.transform;
        }

        return closest;
    }

    protected virtual void FrenzyActive(bool frenzy)
    {
        attackState = AttackState.Frenzy;
    }

    public IEnumerator TriggerDeath()
    {
        StopComponents();
        if (itemDrop)
        {
            GameObject item = Instantiate(itemDrop);
            Rigidbody2D itemBody = item.GetComponent<Rigidbody2D>();
            if (itemBody)
            {
                Quaternion randomDirection = Quaternion.Euler(0, 0, Random.Range(-75f, 75f));
                itemBody.AddForce(randomDirection * Vector2.up, ForceMode2D.Impulse);
            }
        }
        
        yield return new WaitForSeconds(corpseDecayTime);
        gameObject.SetActive(false);
    }
    
    protected virtual Vector3 GetRoamingPosition()
    {
        Vector3 direction = new Vector3(Random.value, Random.value).normalized;
        if (guardPositions.Length > 0)
            return guardPositions[Random.Range(0, guardPositions.Length - 1)] + direction * Random.Range(0, maxWanderDistance);
        
        return transform.position + direction * Random.Range(0, maxWanderDistance);
    }

    protected virtual void ChangeAttackState(float aggressionModifier, float followThreshold = 0.7f, float wanderThreshold = 0.4f)
    {
        float randomValue = Random.value; 
        if (stateUnlocked)
        {
            if (randomValue > (followThreshold / aggressionModifier))
                attackState = AttackState.Following;
            else if (randomValue > (wanderThreshold / aggressionModifier))
                attackState = AttackState.Wandering;
            else
                attackState = AttackState.Stopped;
        }
    }
}
