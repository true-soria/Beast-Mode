using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : VersionedMonoBehaviour
{
    public EnemyHP hp;
    public Collider2D damageBox;
    public DamagePlayer damage;
    public GameObject itemDrop;
    public float corpseDecayTime;
    public Transform[] patrolMarkers;
    public float maxWanderDistance;
    
    protected Rigidbody2D body;

    public enum AttackState
    {
        Stopped = 0,
        Wandering,
        Attacking,
        Following
    }
    
    [HideInInspector] public bool frenzied;
    [HideInInspector] public float globalAggression = 0.5f;
    [HideInInspector] public bool stateUnlocked = true;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public Transform target;
    
    [SerializeField] protected float baseAggression = 0.5f;

    protected  const float FrenzyRange = 16f;
    protected readonly string[] enemyLayers = {"Enemies", "EnemiesNoPlatform"};
    
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
        body = gameObject.GetComponent<Rigidbody2D>();
        damage.hp = hp;
        damage.enemyMovement = this;
        damage.body = body;
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
        frenzied = false;
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
                ActivateFrenzy();
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

    protected virtual void ActivateFrenzy()
    {
        frenzied = true;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, FrenzyRange * AggressionValue(), Vector2.right, 1f, (LayerMask.GetMask(enemyLayers)));
        if (hit.collider)
            target = hit.transform;
        else
            target = transform;
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
        if (patrolMarkers.Length > 0)
            return patrolMarkers[Random.Range(0, patrolMarkers.Length)].position + direction * Random.Range(0f, maxWanderDistance);
        
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

    public virtual void ReceiveKnockBack(Vector3 force)
    {
        body.velocity = force;
    }

    protected virtual float AggressionValue()
    {
        return 0.5f + globalAggression + baseAggression;
    }
}
