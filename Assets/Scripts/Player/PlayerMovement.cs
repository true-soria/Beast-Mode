using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float baseMoveSpeed;
    public float baseJumpHeight;
    public float wallKickMultiplier;
    public float airSpeedMult;
    public float dashSpeed;
    public float slamSpeed;
    public float floatSpeed;
    public float slamGravityScale;
    public float slidingGravityScale;

    private Rigidbody2D _body;
    private BoxCollider2D _collisionBox;
    private CircleCollider2D _hurtBox;

    private State _currentState = State.Air;
    private Vector3 _slideJump;
    private int _jumpsLeft;
    private int _dashesLeft;
    private float _clingTimeLeft;
    private float _floatTimeLeft;
    private float _moveSpeed;
    private float _jumpHeight;

    [HideInInspector] public PlayerEffects playerEffects;
    [HideInInspector] public Vector2 move;
    [HideInInspector] public Vector2 aim;
    

    enum State
    {
        Air,
        Floating,
        Slamming,
        Grounded,
        Platform,
        Sliding,
        Clinging
    }

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _collisionBox = GetComponent<BoxCollider2D>();
        _hurtBox = GetComponentInChildren<CircleCollider2D>();
        RestoreJumps();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.collider.gameObject.tag)
        {
            case "Floor":
                RestoreJumps();
                _currentState = State.Grounded;
                break;
            case "Platform":
                RestoreJumps();
                _currentState = State.Platform;
                break;
            case "Wall":
                if (_clingTimeLeft > 0 && (_currentState == State.Air || _currentState == State.Floating))
                {
                    RestoreJumps();
                    _currentState = State.Clinging;
                    _body.velocity = Vector2.zero;
                    _body.gravityScale = 0;
                }
                break;
            case "Slide":
                if (_currentState != State.Grounded && _currentState != State.Platform)
                {
                    RestoreJumps();
                    _currentState = State.Sliding;
                    _slideJump = other.gameObject.transform.rotation * Vector3.up;
                    _slideJump *= _jumpHeight;
                    _body.gravityScale *= slidingGravityScale;
                }
                break;
        }
    }

    private void RestoreJumps()
    {
        _hurtBox.enabled = true;
        _body.gravityScale = playerEffects.gravityScale;
        _clingTimeLeft = playerEffects.wallClingTime;
        _jumpsLeft = playerEffects.extraJumps;
        _dashesLeft = playerEffects.extraDashes;
        _floatTimeLeft = playerEffects.floatDuration;
        _moveSpeed = baseMoveSpeed * playerEffects.moveSpeedMult;
        _jumpHeight = baseJumpHeight * playerEffects.jumpHeightMult;
    }
    
    
    
    void OnCollisionExit2D(Collision2D other)
    {
        switch (other.collider.gameObject.tag)
        {
            case "Floor":
            case "Platform":
                _currentState = State.Air;
                break;
            case "Wall":
                if (_currentState == State.Clinging)
                {
                    _clingTimeLeft = 0;
                    _body.gravityScale = playerEffects.gravityScale;
                    _currentState = State.Air;
                }
                break;
        }
    }

    public void ActionJump()
    {
        Debug.Log("State at ActionJump: " + _currentState);
        switch (_currentState)
        {
            case State.Air:
                if (_jumpsLeft > 0)
                {
                    float height = Mathf.Min(_jumpHeight, 1.25f * _jumpHeight * _jumpsLeft / playerEffects.extraJumps);
                    float deltaX = move.x * _moveSpeed * Time.deltaTime;
                    _body.velocity = Vector2.zero;
                    _body.AddForce(Vector2.up * height + deltaX * Vector2.right, ForceMode2D.Impulse);
                    _jumpsLeft--;
                }
                break;
            case State.Floating:
                _currentState = State.Air;
                _floatTimeLeft = 0;
                _body.gravityScale = playerEffects.gravityScale;
                if (_jumpsLeft > 0)
                {
                    float height = Mathf.Min(_jumpHeight, 1.25f * _jumpHeight * _jumpsLeft / playerEffects.extraJumps);
                    float deltaX = move.x * _moveSpeed * Time.deltaTime;
                    _body.velocity = Vector2.zero;
                    _body.AddForce(Vector2.up * height + deltaX * Vector2.right, ForceMode2D.Impulse);
                    _jumpsLeft--;
                }
                break;
            case State.Grounded:
                _currentState = State.Air;
                _body.AddForce(Vector2.up * _jumpHeight, ForceMode2D.Impulse);
                break;
            case State.Platform:
                if (move.y < -0.9f)
                    StartCoroutine(DropThroughPlatform());
                else
                {
                    _currentState = State.Air;
                    _body.AddForce(Vector2.up * _jumpHeight, ForceMode2D.Impulse);
                }
                break;
            case State.Sliding:
                _currentState = State.Air;
                _body.velocity = new Vector2(_body.velocity.x, 0);
                _body.AddForce(_slideJump, ForceMode2D.Impulse);
                _body.gravityScale = playerEffects.gravityScale;
                break;
            case State.Clinging:
                _body.AddForce(aim * (_jumpHeight * wallKickMultiplier), ForceMode2D.Impulse);
                break;
        }
    }

    IEnumerator DropThroughPlatform()
    {
        _collisionBox.enabled = false;
        yield return new WaitForSeconds(0.2f);
        _collisionBox.enabled = true;
    }

    public void ActionLeftDash()
    {
        if (_dashesLeft > 0 && _currentState != State.Slamming)
        {
            _body.gravityScale = playerEffects.gravityScale;
            _body.velocity = new Vector2(_body.velocity.x < 0 ? _body.velocity.x : 0, 0);
            _body.AddForce(new Vector2(-dashSpeed, _jumpHeight / 3), ForceMode2D.Impulse);
            _dashesLeft--;
        }
    }
    
    public void ActionRightDash()
    {
        if (_dashesLeft > 0 && _currentState != State.Slamming)
        {
            _body.gravityScale = playerEffects.gravityScale;
            _body.velocity = new Vector2(_body.velocity.x > 0 ? _body.velocity.x : 0, 0);
            _body.AddForce(new Vector2(dashSpeed, _jumpHeight / 3), ForceMode2D.Impulse);
            _dashesLeft--;
        }
    }

    public void ActionStartHover()
    {
        if (_floatTimeLeft > 0 && _currentState == State.Air)
        {
            _currentState = State.Floating;
            _body.velocity = new Vector2(_body.velocity.x, floatSpeed);
            _body.gravityScale = 0;
        }
    }

    public void ActionStopHover()
    {
        if (_currentState == State.Floating)
        {
            _currentState = State.Air;
            _floatTimeLeft = 0;
            _body.gravityScale = playerEffects.gravityScale;
        }
    }

    public void ActionSlam()
    {
        if (_currentState == State.Air || _currentState == State.Floating)
        {
            switch (playerEffects.slamEquipped)
            {
                case 1:
                    _currentState = State.Slamming;
                    _body.velocity = new Vector2(_body.velocity.x, slamSpeed);
                    break;
                case 2:
                    _currentState = State.Slamming;
                    _body.velocity = Vector2.zero;
                    _hurtBox.enabled = false;
                    break;
                case 3:
                    _currentState = State.Slamming;
                    _body.AddForce(Vector2.up * (_jumpHeight / 1.5f), ForceMode2D.Impulse);
                    _body.gravityScale *= slamGravityScale;
                    _hurtBox.enabled = false;
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_currentState != State.Sliding && _currentState != State.Slamming)
        {
            float deltaX = move.x * _moveSpeed * Time.deltaTime;
            if (_currentState == State.Grounded || _currentState == State.Platform)
            {
                Vector2 movement = new Vector2(deltaX, _body.velocity.y);
                _body.velocity = movement;
            }
            else if (Mathf.Sign(_body.velocity.x) * Mathf.Sign(deltaX) < 0 || Mathf.Abs(_body.velocity.x) < Mathf.Abs(deltaX))
            {
                _body.AddForce(new Vector2(deltaX * airSpeedMult, 0), ForceMode2D.Impulse);
            }
        }

        switch (_currentState)
        {
            case State.Clinging:
            {
                _clingTimeLeft -= Time.deltaTime;
                if (_clingTimeLeft < 0)
                {
                    _currentState = State.Air;
                    _body.gravityScale = playerEffects.gravityScale;
                }

                break;
            }
            case State.Floating:
            {
                _floatTimeLeft -= Time.deltaTime;
                if (_floatTimeLeft < 0)
                {
                    _currentState = State.Air;
                    _body.gravityScale = playerEffects.gravityScale;
                }

                break;
            }
        }
    }
}