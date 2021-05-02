using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class SlimeEnemyMovement : EnemyMovement
{
    public float maxMoveSpeed = 200f;
    public float minMoveSpeed = 30f;
    public float jumpHeightMult = 30f;
    public float leapRange = 15f;
    

    private Vector2 _velocity = Vector2.zero;
    private SpriteRenderer _spriteRenderer;
    private int _ticksPerLeap;
    private int _ticksSinceLastLeap;
    private bool _grounded;

    protected override void InitComponents()
    {
        base.InitComponents();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected override void RestartComponents()
    {
        base.RestartComponents();
        damageBox.enabled = false;
        stateUnlocked = true;
        attackState = AttackState.Stopped;
        float aggressionModifier = AggressionValue();
        _ticksPerLeap = Mathf.FloorToInt((aggressionModifier - 1f) * 10);
        InvokeRepeating(nameof(UpdateVelocity), 3.5f / aggressionModifier, 1 / aggressionModifier);
    }

    protected override void StopComponents()
    {
        base.StopComponents();
        attackState = AttackState.Stopped;
        CancelInvoke();
    }

    private void FixedUpdate()
    {
        switch (attackState)
        {
            case AttackState.Wandering:
            case AttackState.Following:
                body.velocity = new Vector2(_velocity.x * Time.deltaTime, body.velocity.y);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Platform":
            case "Floor":
                _grounded = true;
                break;
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Platform":
            case "Floor":
                _grounded = false;
                break;
        }
    }

    private void UpdateVelocity()
    {
        float aggressionModifier = AggressionValue();
        Vector2 toTarget;
        switch (attackState)
        {
            case AttackState.Wandering:
                toTarget = GetRoamingPosition() - transform.position;
                float relativeDistance = Mathf.Sqrt(Mathf.Abs(toTarget.x / leapRange));
                float expectedSpeed = relativeDistance * (maxMoveSpeed + minMoveSpeed) / 2 * aggressionModifier;
                _velocity = Mathf.Sign(toTarget.x) * Vector2.right *
                            Mathf.Clamp(expectedSpeed, minMoveSpeed, maxMoveSpeed);
                break;
            case AttackState.Following:
                toTarget = target.position - transform.position;
                relativeDistance = Mathf.Sqrt(Mathf.Abs(toTarget.x / leapRange));
                if (relativeDistance < 0.25f * aggressionModifier || _ticksSinceLastLeap >= _ticksPerLeap)
                {
                    StartCoroutine(LeapAttack(aggressionModifier, toTarget));
                }
                else
                {
                    expectedSpeed = relativeDistance * (maxMoveSpeed + minMoveSpeed) / 2 * aggressionModifier;
                    _velocity = Mathf.Sign(toTarget.x) * Vector2.right *
                                Mathf.Clamp(expectedSpeed, minMoveSpeed, maxMoveSpeed);
                    _ticksSinceLastLeap++;
                }
                break;
            case AttackState.Attacking:
                break;
        }
        
        ChangeAttackState(aggressionModifier);
    }

    private IEnumerator LeapAttack(float aggressionModifier, Vector2 toTarget)
    {
        FindNewTarget();
        _velocity = Vector2.zero;
        _ticksSinceLastLeap = 0;
        attackState = AttackState.Attacking;
        stateUnlocked = false;
        yield return new WaitForSeconds(1f / aggressionModifier);
        
        damageBox.enabled = true;
        _spriteRenderer.color = Color.red;

        float maxDistance = leapRange / 3 * aggressionModifier;
        float leapX = 2f/3f * Mathf.Clamp(toTarget.x, -maxDistance, maxDistance);
        float leapY = Mathf.Clamp(toTarget.y, 1f, maxDistance);
        body.AddForce(new Vector2(leapX, leapY) * jumpHeightMult, ForceMode2D.Impulse);
        
        yield return new WaitUntil(() => _grounded);
        yield return new WaitForSeconds(1f / aggressionModifier);
        _spriteRenderer.color = Color.white;
        damageBox.enabled = false;
        stateUnlocked = true;
    }
}
