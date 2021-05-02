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
        damageBox.enabled = false;
        float aggressionModifier = AggressionValue();
        if (_astarAI != null)
        {
            _astarAI.onSearchPath += FixedUpdate;
            _astarAI.enabled = true;
        }
        InvokeRepeating(nameof(CycleFocus), 3.5f / aggressionModifier, 1 / aggressionModifier);
    }

    protected override void StopComponents()
    {
        base.StopComponents();
        if (_astarAI != null)
        {
            _astarAI.onSearchPath -= FixedUpdate;
            _astarAI.enabled = false;
        }
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
        float aggressionModifier = AggressionValue();
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
        damageBox.enabled = true;
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
        ChangeAttackState(aggressionModifier);
        _astarAI.repathRate = originalRepathRate;
        damageBox.enabled = false;
    }

    public override void ReceiveKnockBack(Vector3 force)
    {
        if (gameObject.activeSelf)
            StartCoroutine(KnockBackCoroutine(force, 2.5f / AggressionValue()));
    }

    private IEnumerator KnockBackCoroutine(Vector3 force, float duration)
    {
        _astarAI.enabled = false;
        base.ReceiveKnockBack(force);
        yield return new WaitForSeconds(duration);
        _astarAI.enabled = hp.isAlive;
    }
}
