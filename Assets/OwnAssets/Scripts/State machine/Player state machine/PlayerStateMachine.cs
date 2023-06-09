using Events;
using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    //FOR DEVELOPMENT:
    [SerializeField] public GameObject _prefab;
    //

    SpriteRenderer _sprite;
    BoxCollider2D _boxCollider;
    //bool _isFalling;
    //bool _isJumping = false;

    //INPUT SYSTEM:
    PlayerInput _playerInput;
        //horizontal:
    float _lateralMovementInput;
    float _lateralMovementModified;
    bool _isLateralMovementPressed = false;
    bool _monitorLeftInput = false, _monitorRightInput = false;

    //vertical:
    Vector3 _verticalMovementCalculated;
    bool _isJumpPressed = false;
    bool _requierNewJumpPress = false;

    //CEILING CHECKS
    RaycastHit2D _ceilingRay;

    //HORIZONTAL MOVEMENT:
    float _walkSpeed = 8f;
    [SerializeField] float _runSpeed = 5f;
    float _fractionOfMaxVel = 0f;
    float _counterForAcc = 0f;
    float _lazyDirection = 1f;
    //acceleration / deaceleration times pre-defined variables:
    //THE MORE LITTLE THE FASTER!
        //on ground:
            float _accGround = 2f;
            float _deAccGround = 0.5f;
            float _deAccGroundTurning = 0.15f; //should be faster that if not turning
        //on air: (accelerates faster, deAccelerates slower)
            float _accAir = 1.25f;
            float _deAccAir = 2f;
            float _deAccAirTurning = 1.5f; //should be faster that if not turning
    //acceleration / deaceleration variables to apply in the code:
    float _accelerationTime;
    float _deAccelerationTimeReleaseInput;
    float _deAccelerationTimeTurning;

    //JUMP PHYSICS:
    float _gravity;
    float _initialJumpVelocity;
    float _maxJumpHeight = 2f;
    float _maxJumpTime = 0.5f;

    //FALL FASTER:
    float _fallMultiplier = 3f;
    float _fallSpeedClamp = -15f;
    float _jumpToFallEarly = 0f;

    //FLOOR SNAP:
    [SerializeField] LayerMask _layerMask;
    float _footAdjust = 0.01f;
    RaycastHit2D _raycastHit;
    bool _isGrounded;
    Vector2 _surfaceDown;

    //WALL CHECKS
    RaycastHit2D _leftWallRaycast;
    RaycastHit2D _rightWallRaycast;
    bool _wallOnLeft;
    bool _wallOnRight;
    bool _hasJustWallJumped = false;
    Vector2 _surfaceLeft, _surfaceRight;
    float _timeToWallSlide = 1f;
    float _wallSlideSpeed = 1f;
    bool _wallPenetrationBug = false;

    //COYOTE TIME:
    float _coyoteTimer = 0f;
    float _coyoteTime = 0.07f;

    //JUMP BUFFER:
    float _jumpBufferTime = 0.08f;
    float _jumpBufferTimer;

    //ANIMATOR HASHES:
    Animator _animator;
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _isFallingHash;

    //STATE MACHINE:
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //PROYECTILE
    [SerializeField] GameObject _projectilePrefab;

    #region GETTERS & SETTERS
    public Animator Animator { get { return _animator; } }
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public float WalkSpeed { get { return _walkSpeed; } set { _walkSpeed = value; } }
    public float RunSpeed { get { return _runSpeed; } set { _runSpeed = value; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }
    public bool IsLateralMovementPressed { get { return _isLateralMovementPressed; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireNewJumpPress { get { return _requierNewJumpPress; } set { _requierNewJumpPress = value; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public float LateralMovementInput { get { return _lateralMovementInput; } set { _lateralMovementInput = value; } }
    public float LateralMovementModified { get { return _lateralMovementModified; } set { _lateralMovementModified = value; } }
    public float VerticalMovementCalculated { get { return _verticalMovementCalculated.y; } set { _verticalMovementCalculated.y = value; } }
    public int IsWalkingHash { get { return _isWalkingHash; } set { _isWalkingHash = value; } }
    public int IsRunningHash { get { return _isRunningHash; } set { _isRunningHash = value; } }
    public int IsJumpingHash { get { return _isJumpingHash; } set { _isJumpingHash = value; } }
    public int IsFallingHash { get { return _isFallingHash; } set { _isFallingHash = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float FallMultiplier { get { return _fallMultiplier; } set { _fallMultiplier = value; } }
    public float FallSpeedClamp { get { return _fallSpeedClamp; } set { _fallSpeedClamp = value; } }
    public float CoyoteTimer { get { return _coyoteTimer; } set { _coyoteTimer = value; } }
    public float CoyoteTime { get { return _coyoteTime; } set { _coyoteTime = value; } }
    public float JumpBufferTime {  get { return _jumpBufferTime; } set { _jumpBufferTime = value; } }
    public float JumpBufferTimer { get { return _jumpBufferTimer; } set { _jumpBufferTimer = value; } }
    public float MaxJumpHeight { get { return _maxJumpHeight; } set { _maxJumpHeight = value; } }
    public float MaxJumpTime { get { return _maxJumpTime; } set { _maxJumpTime = value; } }
    public float FractionOfMaxVel { get { return _fractionOfMaxVel; } set { _fractionOfMaxVel = value; } }
    public float LazyDirection { get { return _lazyDirection; } set { _lazyDirection = value; } }
    public float CounterForAcc { get { return _counterForAcc; } set { _counterForAcc = value; } }
    public float AccelerationTime { get { return _accelerationTime; } set { _accelerationTime = value; } }
    public float DeAccelerationTimeReleaseInput { get { return _deAccelerationTimeReleaseInput; } set { _deAccelerationTimeReleaseInput = value; } }
    public float DeAccelerationTimeTurning { get { return _deAccelerationTimeTurning; } set { _deAccelerationTimeTurning = value; } }
    public float AccGround { get { return _accGround; } set { _accGround = value; } }
    public float DeAccGround { get { return _deAccGround; } set { _deAccGround = value; } }
    public float DeAccGroundTurning { get { return _deAccGroundTurning; } set { _deAccGroundTurning = value; } }
    public float AccAir { get { return _accAir; } set { _accAir = value; } }
    public float DeAccAir { get { return _deAccAir; } set { _deAccAir = value; } }
    public float DeAccAirTurning { get { return _deAccAirTurning; } set { _deAccAirTurning = value; } }
    public float JumpToFallEarly { get { return _jumpToFallEarly; } set { _jumpToFallEarly = value; } }
    public bool WallOnLeft { get { return _wallOnLeft; } }
    public bool WallOnRight { get {  return _wallOnRight; } }
    public bool HasJustWallJumped { get { return _hasJustWallJumped; } set { _hasJustWallJumped = value; } }
    public bool MonitorLeftInput { get { return _monitorLeftInput; } }
    public bool MonitorRightInput { get { return _monitorRightInput; } }
    public float TimeToWallSlide { get { return _timeToWallSlide; } set { _timeToWallSlide = value; } }
    public float WallSlideSpeed { get { return _wallSlideSpeed; } set { _wallSlideSpeed = value; } }
    public bool WallPenetrationBug { get { return  _wallPenetrationBug; } }
    #endregion
    void Awake()
    {
        //get components
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _playerInput = new PlayerInput();
        //state machine initialized
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        //hash association for animator bool controlers
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isFallingHash = Animator.StringToHash("isFalling");
        //subscribe functions to imputs
        _playerInput.PlayerMovement.Move.started += OnMovementInput;
        _playerInput.PlayerMovement.Move.canceled += OnMovementInput;
        _playerInput.PlayerMovement.Move.performed += OnMovementInput;

        _playerInput.PlayerMovement.Jump.started += OnJump;
        _playerInput.PlayerMovement.Jump.canceled += OnJump;

        _playerInput.PlayerMovement.MonitoringMoveLeft.started += MonitorLeft;
        _playerInput.PlayerMovement.MonitoringMoveLeft.canceled += MonitorLeft;
        _playerInput.PlayerMovement.MonitoringMoveRight.started += MonitorRight;
        _playerInput.PlayerMovement.MonitoringMoveRight.canceled += MonitorRight;

        //_playerInput.PlayerMovement.Shoot.started += OnShoot;
        //_playerInput.PlayerMovement.Shoot.canceled += OnShoot;

        //for jump buffer | MACHETAZO?:
        _playerInput.PlayerMovement.Jump.started += NewJump;

        setupJumpVariables();
        _lateralMovementModified = transform.position.x;
    }   

    void Update() /*===|===|===|===|===|===|===|===|===[ U P D A T E ]===|===|===|===|===|===|===|===|===*/
    {
        Shoot();
        _currentState.UpdateStates(); 
        WallCheck();
        GroundedCheck();
        CeilingCheck();

        transform.position = new Vector3(_lateralMovementModified, transform.position.y, transform.position.z); //for lateral movement "snapy"
        transform.Translate(_verticalMovementCalculated * Time.deltaTime); //for vertical movement "realistic"

        TextDebug();//DEVELOPMENT
        if(_lateralMovementInput < 0) { _sprite.flipX = true; } else { _sprite.flipX = false; }//DEVELOPMENT IMPLEMENTATION?


    }

    #region Input to bool functions
    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requierNewJumpPress = false;
    }
    void NewJump(InputAction.CallbackContext context)
    {
        if (_currentState.ToString() == "PlayerFallingState") _jumpBufferTimer = _jumpBufferTime;
        DrawShadow();//FOR DEVELOPMENT ONLY
    } //for jump buffer

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _lateralMovementInput = context.ReadValue<float>();
        if (_lateralMovementInput != 0) _isLateralMovementPressed = true; else _isLateralMovementPressed = false;
    }

    void MonitorLeft(InputAction.CallbackContext context)
    {
        _monitorLeftInput = context.ReadValueAsButton();
    }
    void MonitorRight(InputAction.CallbackContext context)
    {
        _monitorRightInput = context.ReadValueAsButton();
    }
    #endregion

    public void setupJumpVariables() //SET PUBLIC FOR DEVELOPMENT / DEBUG
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    void OnEnable()
    {
        _playerInput.PlayerMovement.Enable();
    }

    void OnDisable()
    {
        _playerInput.PlayerMovement.Disable();
    }

    public void GroundedCheck()
    {
        BoxCast(new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.center.y - _boxCollider.bounds.extents.y), new Vector2(_boxCollider.bounds.size.x * 0.8f, 0.01f), 0, -Vector2.up, 0, _layerMask);
        _raycastHit = Physics2D.BoxCast(new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.center.y - _boxCollider.bounds.extents.y), new Vector2(_boxCollider.bounds.size.x * 0.8f, 0.01f), 0, -Vector2.up, 0, _layerMask);
        if (_raycastHit.collider != null) { _isGrounded = true; } else { _isGrounded = false; }

        //clamp:
        if (_raycastHit.collider != null)
        {
            
            _verticalMovementCalculated.y = Mathf.Clamp(_verticalMovementCalculated.y, 0, float.PositiveInfinity);
            SnapFeet();
        }


        BoxCast(new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.center.y - _boxCollider.bounds.extents.y * 0.95f), new Vector2(_boxCollider.bounds.size.x * 0.8f, 0.01f), 0, -Vector2.up, 0, _layerMask);

    }
    public void SnapFeet()//WORKS PERFECTLY!
    {
        if(_raycastHit.collider != null)
        {
            _surfaceDown = Physics2D.ClosestPoint(transform.position, _raycastHit.collider) + Vector2.up * _raycastHit.collider.bounds.size.y;
            transform.position = new Vector3(
                transform.position.x,
                _surfaceDown.y - (_raycastHit.collider.bounds.extents.y * 2) + _boxCollider.bounds.extents.y - _footAdjust,
                transform.position.z);
        }
    }

    public void WallCheck()
    {
        _leftWallRaycast = Physics2D.BoxCast(new Vector2(_lateralMovementModified - _boxCollider.bounds.extents.x, transform.position.y), new Vector2(0.01f, _boxCollider.bounds.size.y * 0.5f), 0, -Vector2.right, 0, _layerMask);
        _wallOnLeft = _leftWallRaycast;
        //DEBUG:
        BoxCast(new Vector2(_lateralMovementModified - _boxCollider.bounds.extents.x, transform.position.y), new Vector2(0.01f, _boxCollider.bounds.size.y * 0.5f), 0, -Vector2.right, 0, _layerMask);
        //
        _rightWallRaycast = Physics2D.BoxCast(new Vector2(_lateralMovementModified + _boxCollider.bounds.extents.x, transform.position.y), new Vector2(0.01f, _boxCollider.bounds.size.y * 0.5f), 0, Vector2.right, 0, _layerMask);
        _wallOnRight = _rightWallRaycast;
        //debug:
        BoxCast(new Vector2(_lateralMovementModified + _boxCollider.bounds.extents.x, transform.position.y), new Vector2(0.01f, _boxCollider.bounds.size.y * 0.5f), 0, Vector2.right, 0, _layerMask);



        //CLAMP:

        if (_leftWallRaycast.collider != null)
        {
            _lateralMovementModified = Mathf.Clamp(_lateralMovementModified, transform.position.x - 0.05f, float.PositiveInfinity);
        }
        //
        if (_rightWallRaycast.collider != null)
        {
            _lateralMovementModified = Mathf.Clamp(_lateralMovementModified, float.NegativeInfinity, transform.position.x + 0.05f);
        }
        //




        if (_raycastHit.collider != null && _leftWallRaycast.collider != null)
        {
            _wallPenetrationBug = _raycastHit.collider.transform == _leftWallRaycast.collider.transform ? true : false;
        }
        else if (_raycastHit.collider != null && _rightWallRaycast.collider != null)
        {
            _wallPenetrationBug = _raycastHit.collider.transform == _rightWallRaycast.collider.transform ? true : false;
        }
        else
        {
            _wallPenetrationBug = false;
        }
    }

    public void SnapToWall()
    {
        if (_wallOnLeft)
        {
            if(_leftWallRaycast.collider != null)
            {
                _surfaceLeft = Physics2D.ClosestPoint(transform.position, _leftWallRaycast.collider);
                _lateralMovementModified = _surfaceLeft.x + _boxCollider.bounds.extents.x;
            }
        }
        else if (_wallOnRight)
        {
            if(_rightWallRaycast.collider != null)
            {
                _surfaceRight = Physics2D.ClosestPoint(transform.position, _rightWallRaycast.collider);
                _lateralMovementModified = _surfaceRight.x - _boxCollider.bounds.extents.x;
            }
        }

    }

    public void CeilingCheck()
    {
        _ceilingRay = Physics2D.BoxCast(transform.position + new Vector3(0, _boxCollider.bounds.extents.y), new Vector2(_boxCollider.bounds.size.x, 0.1f), 0, Vector2.up, 0, _layerMask);
        BoxCast(transform.position + new Vector3(0, _boxCollider.bounds.extents.y), new Vector2(_boxCollider.bounds.size.x, 0.1f), 0, Vector2.up, 0, _layerMask);

        if (_ceilingRay.collider != null)
        {
            //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, float.NegativeInfinity, _ceilingRay.point.y - _boxCollider.bounds.extents.y), transform.position.z);
            _verticalMovementCalculated.y = Mathf.Clamp(_verticalMovementCalculated.y, float.NegativeInfinity, 0);
        }
        //


        RaycastHit2D ceilingLeftLateral = Physics2D.Raycast(transform.position + new Vector3(-_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y), Vector2.up);
        Debug.DrawRay(transform.position + new Vector3(-_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y), Vector2.up, Color.red);
        RaycastHit2D ceilingLeftMedial = Physics2D.Raycast(transform.position + new Vector3(-_boxCollider.bounds.extents.x * 0.8f, _boxCollider.bounds.extents.y), Vector2.up);
        Debug.DrawRay(transform.position + new Vector3(-_boxCollider.bounds.extents.x * 0.8f, _boxCollider.bounds.extents.y), Vector2.up, Color.blue);
        
        RaycastHit2D ceilingRightLateral = Physics2D.Raycast(transform.position + new Vector3(_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y), Vector2.up);
        Debug.DrawRay(transform.position + new Vector3(_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y), Vector2.up, Color.red);
        RaycastHit2D ceilingRightMedial = Physics2D.Raycast(transform.position + new Vector3(_boxCollider.bounds.extents.x * 0.8f, _boxCollider.bounds.extents.y), Vector2.up);
        Debug.DrawRay(transform.position + new Vector3(_boxCollider.bounds.extents.x * 0.8f, _boxCollider.bounds.extents.y), Vector2.up, Color.blue);
    }



    public void Shoot()
    {
        float dir = _lateralMovementInput != 0 ? _lateralMovementInput : _lazyDirection;
        //Debug.Log(dir);
        if (_playerInput.PlayerMovement.Shoot.WasPressedThisFrame())
        {
            EventManager.Dispatch(ENUM_PlayerActions.shoot, 0.4f);
            var projectil = Instantiate(_projectilePrefab, transform.position + new Vector3(_boxCollider.bounds.extents.x * dir, 0.2f, 0), Quaternion.identity);
            projectil.GetComponent<PlayerProjectile>()._direction = dir;
        }
    }


    static public RaycastHit2D BoxCast(Vector2 origen, Vector2 size, float angle, Vector2 direction, float distance, int mask)
    {
        RaycastHit2D hit = Physics2D.BoxCast(origen, size, angle, direction, distance, mask);

        //Setting up the points to draw the cast
        Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
        float w = size.x * 0.5f;
        float h = size.y * 0.5f;
        p1 = new Vector2(-w, h);
        p2 = new Vector2(w, h);
        p3 = new Vector2(w, -h);
        p4 = new Vector2(-w, -h);

        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        p1 = q * p1;
        p2 = q * p2;
        p3 = q * p3;
        p4 = q * p4;

        p1 += origen;
        p2 += origen;
        p3 += origen;
        p4 += origen;

        Vector2 realDistance = direction.normalized * distance;
        p5 = p1 + realDistance;
        p6 = p2 + realDistance;
        p7 = p3 + realDistance;
        p8 = p4 + realDistance;


        //Drawing the cast
        Color castColor = hit ? Color.red : Color.green;
        Debug.DrawLine(p1, p2, castColor);
        Debug.DrawLine(p2, p3, castColor);
        Debug.DrawLine(p3, p4, castColor);
        Debug.DrawLine(p4, p1, castColor);

        Debug.DrawLine(p5, p6, castColor);
        Debug.DrawLine(p6, p7, castColor);
        Debug.DrawLine(p7, p8, castColor);
        Debug.DrawLine(p8, p5, castColor);

        Debug.DrawLine(p1, p5, Color.grey);
        Debug.DrawLine(p2, p6, Color.grey);
        Debug.DrawLine(p3, p7, Color.grey);
        Debug.DrawLine(p4, p8, Color.grey);
        if (hit)
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
        }

        return hit;
    }


        ///debug:

        [SerializeField] Text t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23,t24,t25,t26,t27,t28,t29,t30,t31;

    void Start()
    {
        t1.text = "";
        t2.text = "";
        t3.text = "";
        t4.text = "";
        t5.text = "";
        t6.text = "";
        t7.text = "";
        t8.text = "";
        t9.text = "";
        t10.text = "";
        t11.text = "";
        t12.text = "";
        t13.text = "";
        //
        t14.text = "";
        t15.text = "";
        t16.text = "";
        t17.text = "";
        t18.text = "";
        t19.text = "";
        t20.text = "";
        t21.text = "";
        t22.text = "";
        t23.text = "";
        t24.text = "";
        t25.text = "";
        t26.text = "";
        t27.text = "";
        t28.text = "";
        t29.text = "";
        t30.text = "";
        t31.text = "";
    }
    void TextDebug()
    {
        t1.text = "movement composite vector = " + (Math.Round(_lateralMovementInput * _walkSpeed, 2)).ToString() + "," + (Math.Round(_verticalMovementCalculated.y,2)).ToString();
        t2.text = "current state = " + _currentState.ToString() + " | current sub-state = " + _currentState.SubState.ToString();
        t3.text = "fraction of acceleration = " + _fractionOfMaxVel;
        t4.text = "caught wall penetration bug = " + _wallPenetrationBug;

        //

        t14.text = "walk speed = " + _walkSpeed;
        t15.text = "max jump height = " + _maxJumpHeight;
        t16.text = "max jump time = " + _maxJumpTime;
        t17.text = "fall speed multiplier = " + _fallMultiplier;
        t18.text = "max fall speed clamp = " + _fallSpeedClamp;
        t19.text = "coyote time = " + _coyoteTime;
        t20.text = "jump buffer time " + _jumpBufferTime;

            t21.text = "ACCELERATION VARIABLES IN GROUND:";
        t22.text = "Acceleration (on ground) = " + _accGround;
        t23.text = "Deacceleration (button release) = " + _deAccGround;
        t24.text = "Deacc. while changing direction = " + _deAccGroundTurning;
            t25.text = "ACCELERATION VARIABLES IN AIR";
        t26.text = "Acceleration (in jump) = " + _accAir;
        t27.text = "Deacceleration (button release) = " + _deAccAir;
        t28.text = "Deacc. while (tryng) to turn = " + _deAccAirTurning;

        t29.text = "jump to fall early -> " + _jumpToFallEarly;

        t30.text = "time to start wall slide = " + _timeToWallSlide;
        t31.text = "velocity of wall slide = " + _wallSlideSpeed;
    }

    [SerializeField] public Sprite sprite2;
    void DrawShadow()
    {
        GameObject shadow = Instantiate(_prefab, transform.position, Quaternion.identity);
        if (!_isGrounded) { shadow.GetComponent<SpriteRenderer>().sprite = sprite2; }
        shadow.transform.parent = null;
        shadow.transform.localScale = transform.localScale;

    }



    
}
