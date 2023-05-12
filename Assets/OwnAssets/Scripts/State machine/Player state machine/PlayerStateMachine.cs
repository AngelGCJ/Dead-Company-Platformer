using System;
using UnityEditor.Search;
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
    bool _isRunPressed = false;
        //vertical:
    Vector3 _verticalMovementCalculated;
    bool _isJumpPressed = false;
    bool _requierNewJumpPress = false;

    //HORIZONTAL MOVEMENT:
    [SerializeField] float _walkSpeed = 2f;
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
    float _maxJumpTime = 0.75f;

    //FALL FASTER:
    [SerializeField] float _fallMultiplier = 4f;
    [SerializeField] float _fallSpeedClamp = -20f;

    //FLOOR SNAP:
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _footAdjust = 0.01f;
    RaycastHit2D _raycastHit;
    bool _isGrounded;
    Vector2 surface;

    //COYOTE TIME:
    float _coyoteTimer = 0f;
    [SerializeField] float _coyoteTime = 0.13f;

    //JUMP BUFFER:
    [SerializeField] float _jumpBufferTime = 0.1f;
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

    //GETTERS AND SETTERS:
    public Animator Animator { get { return _animator; } }
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public float WalkSpeed { get { return _walkSpeed; } set { _walkSpeed = value; } }
    public float RunSpeed { get { return _runSpeed; } set { _runSpeed = value; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }
    public bool IsLateralMovementPressed { get { return _isLateralMovementPressed; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireNewJumpPress { get { return _requierNewJumpPress; } set { _requierNewJumpPress = value; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
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
        _playerInput.PlayerMovement.Run.started += OnRun;
        _playerInput.PlayerMovement.Run.canceled += OnRun;
        _playerInput.PlayerMovement.Jump.started += OnJump;
        _playerInput.PlayerMovement.Jump.canceled += OnJump;
        //for jump buffer | MACHETAZO?:
        _playerInput.PlayerMovement.Jump.started += NewJump;
        //last direction faced | MACHETAZO?
        _playerInput.PlayerMovement.Move.started += OnMovementInputLazy;

        setupJumpVariables();
        _lateralMovementModified = transform.position.x;
    }   

    void Update() /*===|===|===|===|===|===|===|===|===[ U P D A T E ]===|===|===|===|===|===|===|===|===*/
    {
        GroundedCheck();
        _currentState.UpdateStates();
        transform.position = new Vector3(_lateralMovementModified, transform.position.y, transform.position.z); //for lateral movement "snapy"
        transform.Translate(_verticalMovementCalculated * Time.deltaTime); //for vertical movement "realistic"
        TextDebug();//DEVELOPMENT
        if(_lateralMovementInput < 0) { _sprite.flipX = true; } else { _sprite.flipX = false; }//DEVELOPMENT IMPLEMENTATION?

        //Debug.Log(FractionOfMaxVel);
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

    void OnMovementInputLazy(InputAction.CallbackContext context)
    {
        //_lazyDirection = context.ReadValue<float>();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
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
        _raycastHit = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, _footAdjust, _layerMask);
        if (_raycastHit.collider != null) { _isGrounded = true; } else { _isGrounded = false; }

        //FROM HERE DOWN IS JUST FOR DEVELOPMENT
        Color rayColor;
        if (_raycastHit.collider != null) rayColor = Color.green;
        else rayColor = Color.red;
        Debug.DrawRay(_boxCollider.bounds.center + new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + _footAdjust), rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + _footAdjust), rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y + _footAdjust), Vector2.right * (_boxCollider.bounds.extents.x * 2), rayColor);
        //Debug.Log(_raycastHit.point);
    }
    public void SnapFeet()//WORKS PERFECTLY!
    {
        if(_raycastHit.collider != null)
        {
            surface = Physics2D.ClosestPoint(transform.position, _raycastHit.collider) + Vector2.up * _raycastHit.collider.bounds.size.y;
            transform.position = new Vector3(
                transform.position.x,
                surface.y - (_raycastHit.collider.bounds.extents.y * 2) + _boxCollider.bounds.extents.y - _footAdjust,
                transform.position.z);
        }
    }

    ///debug:

    [SerializeField] Text t1,t2,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23;
    float speedTextDebug;
    string coyoteTimeDebug;
    string jumpBufferDebug;

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
    }
    void TextDebug()
    {
        t1.text =
            "|PRESSED|: " +
            "jump = " + _isJumpPressed.ToString() +
            " || run = " + _isRunPressed.ToString() +
            " || lateral = " + _isLateralMovementPressed.ToString() + " (raw = " + _lateralMovementInput + ")";

        if( _isRunPressed ) { speedTextDebug = _lateralMovementInput * _runSpeed; } else { speedTextDebug = _lateralMovementInput * _walkSpeed; }
        t2.text = "movement composite vector = " + (Math.Round(speedTextDebug,2)).ToString() + "," + (Math.Round(_verticalMovementCalculated.y,2)).ToString();
        
        t3.text = "current state = " + _currentState.ToString() + " | current sub-state = " + _currentState.SubState.ToString();
        t4.text = "ground detected = " + _isGrounded.ToString();

        if (_isGrounded) { coyoteTimeDebug = "Grounded"; } else if (_coyoteTime > _coyoteTimer) { coyoteTimeDebug = (Math.Round(_coyoteTime - _coyoteTimer, 2)).ToString() + "s remaining"; } else { coyoteTimeDebug = "Not able"; };
        t5.text = "coyote time status: " + coyoteTimeDebug;

        if ( _jumpBufferTimer > 0) { jumpBufferDebug = "YES"; } else { jumpBufferDebug = "NO"; }
        t6.text = "jump buffered: " + jumpBufferDebug;

        t7.text = "fraction of acc = " + _fractionOfMaxVel.ToString();
        t8.text = "counter for acc = " + _counterForAcc.ToString();
        t9.text = "lazy dir = " + _lazyDirection.ToString();


        //

        t14.text = "walk speed = " + _walkSpeed.ToString();
        t15.text = "run speed = " + _runSpeed.ToString();
        t16.text = "max jump height = " + _maxJumpHeight.ToString();
        t17.text = "max jump time = " + _maxJumpTime.ToString();
        t18.text = "fall speed multiplier = " + _fallMultiplier.ToString();
        t19.text = "max fall speed clamp = " + _fallSpeedClamp.ToString();
        t20.text = "coyote time = " + _coyoteTime.ToString();
        t21.text = "jump buffer time " + _jumpBufferTime.ToString();



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