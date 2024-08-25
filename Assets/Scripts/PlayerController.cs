using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInputs _inputs;
    private Vector2 _velocity;

    private float _time;

    private bool _cacheQueriesStartInColliders;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        _cacheQueriesStartInColliders = Physics2D.queriesStartInColliders;

        // Velocity.
        _velocity = new Vector2(0, 0);
        
        // Collisions.
        _grounded = false;
        _leftSided = false;
        _rightSided = false;
        _timeLeftGround = -100;

        // Dash.
        _isDashing = false;
        _dashUsable = false;
        _dashToConsume = false;
        _timeDashWasExecuted = -100;
        
        // Jump.
        _bufferedJumpUsable = false;
        _coyoteUsable= false;
        _doubleJumpUsable = false;
        _endedJumpEarly = false;
        _jumpToConsume = false;
        _timeJumpWasPressed = -100;
        
        _wallJumpToConsume = false;
        _timeWallJumpWasExecuted = -100;
        
        // Slide.
        _isSliding = false;
        _slideUsable = false;   
    }

    void Update()
    {
        _time += Time.deltaTime;
        GatherInputs();
    }

    private void GatherInputs()
    {
        _inputs = new FrameInputs()
        {
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            dashDown = Input.GetKeyDown(_stats.dashKey),
            jumpDown = Input.GetButtonDown("Jump"),
            jumpHeld = Input.GetButton("Jump")
        };

        if (_inputs.jumpDown)
        {
            _jumpToConsume = true;
            _wallJumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        if (_inputs.dashDown)
        {
            _dashToConsume = true;
        }
    }

    void FixedUpdate()
    {
        CheckCollisions();
        
        HandleMove();
        HandleDash();
        HandleJump();
        HandleWallJump();
        HandleSlide();
        HandleGravity();
        HandleDirection();
        
        ApplyMovement();
    }

    #region Collisions

    private float _timeLeftGround;
    private bool _grounded;
    private bool _rightSided;
    private bool _leftSided;
    
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        bool rightHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right,
            _stats.collisionHorizontalDistance, ~(_stats.playerLayer | _stats.camera | _stats.cameraRestrictionArea));
        bool leftHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left,
            _stats.collisionHorizontalDistance, ~(_stats.playerLayer | _stats.camera | _stats.cameraRestrictionArea));
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down,
            _stats.collisionVerticalDistance, ~(_stats.playerLayer | _stats.camera | _stats.cameraRestrictionArea));
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up,
            _stats.collisionVerticalDistance, ~(_stats.playerLayer | _stats.camera | _stats.cameraRestrictionArea));
        
        // Hit ceiling
        if (ceilingHit) _velocity.y = Mathf.Min(0, _velocity.y);

        // Touch right edge.
        if (!_rightSided && rightHit)
        {
            _rightSided = true;
            
            _slideUsable = true;
            _slidingDirection = -1;
            
            _jumpToConsume = false;
        }
        
        // Left right edge.
        if (_rightSided && !rightHit)
        {
            _rightSided = false;
            
            _slideUsable = false;
            _isSliding = false;
            
            _doubleJumpUsable = true;
        }
        
        // Touch left edge.
        if (!_leftSided && leftHit)
        {
            _leftSided = true;
            
            _slideUsable = true;
            _slidingDirection = 1;
            
            _jumpToConsume = false;
        }

        // Left left edge.
        if (_leftSided && !leftHit)
        {
            _leftSided = false;
            
            _slideUsable = false;
            _isSliding = false;
            
            _doubleJumpUsable = true;
        }
        
        // Land on ground.
        if (!_grounded && groundHit)
        {
            _grounded = true;
            
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _doubleJumpUsable = false;
            _endedJumpEarly = false;
            
            _slideUsable = false;
            _isSliding = false;
        }
        
        // Left the ground.
        if (_grounded && !groundHit)
        {
            _grounded = false;
            _timeLeftGround = _time;
            
            _doubleJumpUsable = true;
        }
        
        Physics2D.queriesStartInColliders = _cacheQueriesStartInColliders;
    }
    
    #endregion

    #region Move

    private void HandleMove()
    {
        if (_isDashing || IsWallJumping) return;
        float targetSpeed = _stats.maxSpeed * _inputs.move.x;
        float speedDiff = targetSpeed - _velocity.x;
        if (_inputs.move.x == 0)
        {
            float deceleration = _grounded ? _stats.groundDeceleration : _stats.airDeceleration;
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, Mathf.Abs(speedDiff) * deceleration * Time.fixedDeltaTime);
        }
        else
        {
            float acceleration = _grounded ? _stats.groundAcceleration : _stats.airAcceleration;
            _velocity.x = Mathf.MoveTowards(_velocity.x, _stats.maxSpeed * _inputs.move.x, Mathf.Abs(speedDiff) * acceleration * Time.fixedDeltaTime);
        }
        
        // Friction.
        if(_inputs.move.x == 0 && _grounded)
        {
            float frictionAmount = Mathf.Min(_stats.frictionAmount, Mathf.Abs(_velocity.x));
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, frictionAmount);
        }
    }

    #endregion

    #region Dash

    private bool _dashToConsume;
    private float _timeDashWasExecuted;
    private bool _dashUsable;
    private bool _isDashing;
    private bool CanDash => _stats.dash && _dashUsable && _time > _timeDashWasExecuted + _stats.dashCooling;

    private void HandleDash()
    {
        // if (IsWallJumping)
        // {
        //     _dashToConsume = false;
        //     return;
        // }
        if (_grounded || _isSliding) _dashUsable = true;
        if (CanDash && _dashToConsume) StartCoroutine(ExecuteDash());
        _dashToConsume = false;
    }

    private IEnumerator ExecuteDash()
    {
        _dashUsable = false;
        _isDashing = true;
        _timeDashWasExecuted = _time;
        _slideUsable = false;
        _isSliding = false;
        _velocity.x = transform.localScale.x * _stats.dashPower;
        _velocity.y = 0;
        yield return new WaitForSeconds(_stats.dashTime);
        _isDashing = false;
    }

    #endregion
    
    #region Jump

    private bool _bufferedJumpUsable;
    private bool _coyoteUsable;
    private bool _doubleJumpUsable;
    private bool _endedJumpEarly;
    private bool _jumpToConsume;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.jumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _timeLeftGround + _stats.coyoteTime;
    private bool CanUseDoubleJump => _doubleJumpUsable && _stats.doubleJump;

    // Normal Jump.
    private void HandleJump()
    {
        if (_isDashing || IsWallJumping)
        {
            _jumpToConsume = false;
            return;
        }
        if (!_endedJumpEarly && !_grounded && !_inputs.jumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
        if (!_jumpToConsume && !HasBufferedJump) return;
        if (CanUseDoubleJump)
        {
            ExecuteJump();
            _doubleJumpUsable = false;
        }
        if (_grounded || CanUseCoyote)
        {
            ExecuteJump();
            _doubleJumpUsable = true;
        }
        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _velocity.y = _stats.jumpPower;
    }
    
    // Wall Jump.
    private bool _wallJumpToConsume;
    private float _timeWallJumpWasExecuted; 
        
    private bool IsWallJumping => _time < _timeWallJumpWasExecuted + _stats.wallJumpTime;
    
    private void HandleWallJump()
    {
        if (!_stats.wallJump) return;
        if (!_endedJumpEarly && !_grounded && !_inputs.jumpHeld && _velocity.y > 0) _endedJumpEarly = true;
        if (!_wallJumpToConsume && !HasBufferedJump) return;
        if (_isSliding) ExecuteWallJump();
        _wallJumpToConsume = false;
    }

    private void ExecuteWallJump()
    {
        _timeWallJumpWasExecuted = _time;
        _velocity.x = _slidingDirection * _stats.wallJumpHorizontalPower;
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _velocity.y = _stats.wallJumpPower;
    }

    #endregion

    #region Slide

    private bool _isSliding;
    private bool _slideUsable;
    private int _slidingDirection;

    private bool CanSlide => _slideUsable && !_grounded && (_velocity.y < 0 || !_inputs.jumpHeld);
    
    private void HandleSlide()
    {
        if (!_stats.slide) return;
        if (CanSlide) ExecuteSlide();
    }

    private void ExecuteSlide()
    {
        _isSliding = true;
        _velocity.y = -_stats.slideFallSpeed;
    }

    #endregion
    
    #region Gravity

    private void HandleGravity()
    {
        if (_isDashing || _isSliding) return;
        if (_grounded && _velocity.y <= 0)
        {
            _velocity.y = _stats.groundingForce;
        }
        else
        {
            float inAirGravity = _stats.fallAcceleration;
            if (_endedJumpEarly && _velocity.y > 0) inAirGravity *= _stats.jumpEndEarlyGravityModifier;
            if (IsWallJumping) inAirGravity *= _stats.wallJumpGravityModifier;
            _velocity.y = Mathf.MoveTowards(_velocity.y, -_stats.maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Direction

    private void HandleDirection()
    {
        Vector3 scale;
        if (_isSliding)
        {
            scale = transform.localScale;
            scale.x = _slidingDirection;
            transform.localScale = scale;    
            return;
        }
        
        float xv = Mathf.Round(_velocity.x * 1000) / 1000;
        if (xv == 0) return;
        scale = transform.localScale;
        scale.x = xv > 0 ? 1 : -1;
        transform.localScale = scale;
    }

    #endregion

    private void ApplyMovement() => _rb.velocity = _velocity;
}

public struct FrameInputs
{
    public Vector2 move;
    public bool dashDown;
    public bool jumpDown;
    public bool jumpHeld;
}
