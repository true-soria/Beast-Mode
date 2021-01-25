using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    public float baseMoveSpeed;
    public float baseJumpHeight;
    public float wallKickMultiplier;
    public float dashSpeed;
    public float slamSpeed;
    public float floatSpeed;
    public float slamGravityScale;
    public float slidingGravityScale;
    [SerializeField] private GameManager gameManager;

    private Rigidbody2D _body;
    private SpriteRenderer _sprite;
    private BoxCollider2D _collisionBox;
    private CircleCollider2D _hurtBox;

    private PlayerControls _controls;
    private PlayerManager _playerManager;
    
    public Vector2 move;
    private State _currentState = State.Air;
    private Vector3 _slideJump;
    private int _jumpsLeft;
    private int _dashesLeft;
    private float _clingTimeLeft;
    private float _floatTimeLeft;
    private float _moveSpeed;
    private float _jumpHeight;

    [HideInInspector] public PlayerEffects playerEffects;
    [HideInInspector] public Vector2 rsMove;
    [HideInInspector] public bool triggerHeld = false;
    

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

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Gameplay.A.performed += ctx => ActionJump();
        _controls.Gameplay.RS.performed += ctx => ActionJump();
        _controls.Gameplay.LB.performed += ctx => ActionLeftDash();
        _controls.Gameplay.RB.performed += ctx => ActionRightDash();
        _controls.Gameplay.LS.performed += ctx => ActionSlam();
        _controls.Gameplay.LT.performed += ctx => StartHover();
        _controls.Gameplay.LT.canceled += ctx => StopHover();
        _controls.Gameplay.RT.performed += ctx => triggerHeld = true;
        _controls.Gameplay.RT.canceled += ctx => triggerHeld = false;
        _controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
        _controls.Gameplay.Aim.performed += ctx => rsMove = ctx.ReadValue<Vector2>().normalized;
        _controls.Gameplay.Aim.canceled += ctx => rsMove = Vector2.zero;
        _controls.Gameplay.Right.performed += ctx => _playerManager.CycleRightMask();
        _controls.Gameplay.Left.performed += ctx => _playerManager.CycleLeftMask();
        _controls.Gameplay.Up.performed += ctx => _playerManager.CycleUpMask();
        _controls.Gameplay.Down.performed += ctx => _playerManager.CycleDownMask();
        
        _controls.Gameplay.Select.performed += ctx => gameManager.ExitGame();
        _controls.Gameplay.Start.performed += ctx => gameManager.Restart();
    }

    private IEnumerator PauseControls(float duration)
    {
        _controls.Gameplay.Disable();
        yield return new WaitForSeconds(duration);
        _controls.Gameplay.Enable();
    }

    private void OnEnable()
    {
        _controls.Gameplay.Enable();
    }
    
    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _collisionBox = GetComponent<BoxCollider2D>();
        _hurtBox = GetComponentInChildren<CircleCollider2D>();
        _playerManager = GetComponent<PlayerManager>();
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
                    _body.velocity = Vector2.zero;
                    _body.AddForce(Vector2.up * height, ForceMode2D.Impulse);
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
                    _body.velocity = Vector2.zero;
                    _body.AddForce(Vector2.up * height, ForceMode2D.Impulse);
                    _jumpsLeft--;
                }
                break;
            case State.Grounded:
                _currentState = State.Air;
                _body.velocity = Vector2.zero;
                _body.AddForce(Vector2.up * _jumpHeight, ForceMode2D.Impulse);
                break;
            case State.Platform:
                if (move.y < -0.9f)
                    StartCoroutine(DropThroughPlatform());
                else
                {
                    _currentState = State.Air;
                    _body.velocity = Vector2.zero;
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
                _body.AddForce(rsMove * (_jumpHeight * wallKickMultiplier), ForceMode2D.Impulse);
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
            _body.velocity = Vector2.zero;
            _body.AddForce(new Vector2(-dashSpeed, _jumpHeight / 3), ForceMode2D.Impulse);
            _dashesLeft--;
        }
    }
    
    public void ActionRightDash()
    {
        if (_dashesLeft > 0 && _currentState != State.Slamming)
        {
            _body.gravityScale = playerEffects.gravityScale;
            _body.velocity = Vector2.zero;
            _body.AddForce(new Vector2(dashSpeed, _jumpHeight / 3), ForceMode2D.Impulse);
            _dashesLeft--;
        }
    }

    void StartHover()
    {
        if (_floatTimeLeft > 0 && _currentState == State.Air)
        {
            _currentState = State.Floating;
            _body.velocity = new Vector2(_body.velocity.x, floatSpeed);
            _body.gravityScale = 0;
        }
    }

    void StopHover()
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
            Vector2 movement = new Vector2(deltaX, _body.velocity.y);
            if (_currentState == State.Grounded || Mathf.Abs(_body.velocity.x) < Mathf.Abs(deltaX))
            {
                movement = new Vector2(deltaX, _body.velocity.y);
                _body.velocity = movement;
            }
            if (movement.x != 0)
                _sprite.flipX = movement.x < 0;
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