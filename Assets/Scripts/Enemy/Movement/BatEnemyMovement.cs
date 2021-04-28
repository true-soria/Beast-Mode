using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class BatEnemyMovement : EnemyMovement
{
    [SerializeField] private float followDistance = 5f;
    [SerializeField] private float attackRetargetingFrequency = 5f;
    
    private AIPath _astarAI;
    private Vector3 _currentFocus;

    private readonly float _followDuration = 3.5f;

    protected override void InitComponents()
    {
        base.InitComponents();
        _astarAI = GetComponent<AIPath>();
    }

    protected override void RestartComponents()
    {
        base.RestartComponents();
        _currentFocus = GetRoamingPosition();
        float aggressionModifier = 0.5f + globalAggression + baseAggression;
        if (_astarAI != null) _astarAI.onSearchPath += FixedUpdate;
        InvokeRepeating(nameof(CycleFocus), 3.5f / aggressionModifier, 1 / aggressionModifier);
    }

    protected override void StopComponents()
    {
        base.StopComponents();
        if (_astarAI != null) _astarAI.onSearchPath -= FixedUpdate;
        CancelInvoke();
    }

    private void FixedUpdate()
    {
        if (_astarAI != null)
        {
            switch (attackState)
            {
                case AttackState.Stopped:
                case AttackState.Wandering:
                    _astarAI.destination = _currentFocus;
                    break;
                case AttackState.Attacking:
                case AttackState.Following:
                    if (target != null)
                        _astarAI.destination = target.position;
                    break;
            }
        }
    }

    private void CycleFocus()
    {
        float aggressionModifier = 0.5f + globalAggression + baseAggression;
        switch (attackState)
        {
            case AttackState.Stopped:
                _currentFocus = transform.position;
                break;
            case AttackState.Wandering:
                _currentFocus = GetRoamingPosition();
                break;
            case AttackState.Following:
                if (stateUnlocked) StartCoroutine(FollowRoutine(aggressionModifier));
                break;
            case AttackState.Attacking:
                if (stateUnlocked) StartCoroutine(AttackRoutine(aggressionModifier));
                break;
        }
        ChangeAttackState(aggressionModifier);
    }

    private IEnumerator FollowRoutine(float aggressionModifier)
    {
        FindNewTarget();
        stateUnlocked = false;
        float originalSlowdownDistance = _astarAI.slowdownDistance;
        float originalEndReachedDistance = _astarAI.endReachedDistance;
        _astarAI.slowdownDistance = followDistance * 1.5f;
        _astarAI.endReachedDistance = followDistance;
        
        yield return new WaitForSeconds(_followDuration / aggressionModifier);
        stateUnlocked = true;
        _astarAI.slowdownDistance = originalSlowdownDistance;
        _astarAI.endReachedDistance = originalEndReachedDistance;
        attackState = AttackState.Attacking;
    }

    private IEnumerator AttackRoutine(float aggressionModifier)
    {
        FindNewTarget();
        stateUnlocked = false;
        float originalRepathRate = _astarAI.repathRate;
        _astarAI.repathRate = attackRetargetingFrequency;
        
        yield return new WaitForSeconds(_followDuration / aggressionModifier);
        stateUnlocked = true;
        _astarAI.repathRate = originalRepathRate;
    }
}
