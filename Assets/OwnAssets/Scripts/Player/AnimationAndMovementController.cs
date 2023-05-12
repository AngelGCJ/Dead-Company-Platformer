using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    //FOR DEVELOPMENT:
    [SerializeField] public GameObject _prefab;
    //

    PlayerInput _playerInput;
    Animator _animator;
    SpriteRenderer _sprite;
    BoxCollider2D _boxCollider;
    [SerializeField] ContactFilter2D _filter;
    Collider2D[] _results = new Collider2D[1];

    bool _isLateralMovementPressed = false;
    bool _isJumpPressed = false;
    bool _isRunPressed = false;

    bool _isGrounded = true;
    bool _isFalling = false;
    bool _isJumping = false;

    Vector3 _movementVector;
    Vector3 _appliedMovementVector;

    
    float _walkSpeed = 2f;
    float _runSpeed = 5f;
    float _gravity;

    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _isFallingHash;


    float _initialJumpVelocity;
    float _maxJumpHeight= 2f;
    float _maxJumpTime = 0.75f;

    [SerializeField] float _floorHeight = 0.61f;
    [SerializeField] float _footAdjust = -0.1f;
    [SerializeField] float _fallMultiplier = 4f;
    [SerializeField] float _fallSpeedClamp = -20f;


    void Awake()
    {
        //get components
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _playerInput = new PlayerInput();
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

        setupJumpVariables();
    }

    #region Input to bool functions
    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    } 

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _appliedMovementVector.x = context.ReadValue<float>();
        _isLateralMovementPressed = _appliedMovementVector.x != 0;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }
    #endregion

    void setupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }
    
    void HandleJump()
    {
        if (!_isJumping && _isGrounded && _isJumpPressed)
        {
            _isJumping = true;
            _appliedMovementVector.y = _initialJumpVelocity;

            //FOR DEVELOPMENT:
            GameObject shadow = Instantiate(_prefab, transform);
            shadow.transform.parent = null;
        }
        else if (_isJumping && _isGrounded && !_isJumpPressed)
        {
            _isJumping = false;
        }
    }
    
    void handleGravity()
    {
        //this IF works as am isGrounded
        _isFalling = _appliedMovementVector.y <= 0.0f || !_isJumpPressed;
        //Debug.Log("falling" + isFalling + " | movVecty" + movementVector.y + " | grounded" + isGrounded);
        if (Physics2D.OverlapArea
            (new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f),
            new Vector2(transform.position.x + 0.5f, transform.position.y - (0.7f + _footAdjust)),
            _filter, _results) > 0 && _movementVector.y <= 0)
        {//is grounded
            Vector2 surface = Physics2D.ClosestPoint(transform.position, _results[0]) + Vector2.up * _floorHeight;
            transform.position = new Vector3(transform.position.x, surface.y, transform.position.z);
            _appliedMovementVector.y = 0;
            _isGrounded = true;
        }
        else if (_isFalling)
        {
            float previousYVelocity = _appliedMovementVector.y;
            _appliedMovementVector.y = _appliedMovementVector.y + (_gravity * _fallMultiplier * Time.deltaTime);
            _appliedMovementVector.y = Mathf.Max((previousYVelocity + _appliedMovementVector.y) * 0.5f, _fallSpeedClamp);

            _isGrounded = false;
        }
        else
        {
            float previousYVelocity = _appliedMovementVector.y;
            _appliedMovementVector.y = _appliedMovementVector.y + (_gravity * Time.deltaTime);
            _appliedMovementVector.y = (previousYVelocity + _appliedMovementVector.y) * 0.5f;

            _isGrounded = false;
        }
    }

    void handleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        //facing direction
        if (_movementVector.x >= 0) _sprite.flipX = false;
        else if (_movementVector.x < 0) _sprite.flipX = true;

        //walking
        if (_isLateralMovementPressed && !isWalking)
        {
            _animator.SetBool(_isWalkingHash, true);
        }
        else if (!_isLateralMovementPressed && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false);
        }

        //running
        if((_isLateralMovementPressed && _isRunPressed) && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        else if ((!_isLateralMovementPressed || !_isRunPressed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }

        //jumping
        if (_appliedMovementVector.y >= 0.0f && !_isGrounded)
        {
            _animator.SetBool(_isJumpingHash, true);
        }
        else
        {
            _animator.SetBool(_isJumpingHash, false);
        }

        //falling
        if (_isGrounded)
        {
            _animator.SetBool(_isFallingHash, false);
        }
        else if (!_isGrounded && _appliedMovementVector.y < 0.0f)
        {
            _animator.SetBool(_isFallingHash, true);
        }
    }

    void handleHorizontalSpeed()
    {
        if (_isRunPressed)
        {
            transform.position += new Vector3(_appliedMovementVector.x * Time.deltaTime * _runSpeed, 0, 0);
        }
        else
        {
            transform.position += new Vector3(_appliedMovementVector.x * Time.deltaTime * _walkSpeed, 0, 0);
        }
    }

    void Update()
    {
        handleAnimation();
        handleGravity();
        handleHorizontalSpeed();
        HandleJump();
        transform.Translate(_appliedMovementVector * Time.deltaTime);
    }

    void OnEnable()
    {
        _playerInput.PlayerMovement.Enable();
    }

    void OnDisable()
    {
        _playerInput.PlayerMovement.Disable();
    }
}

//OTHER CHECK GROUNDED METHODS
/*RAYCAST METHOD
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, boxCollider.bounds.extents.y + extraHeightCollider, groundLayers);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider.bounds.center, Vector2.down * (boxCollider.bounds.extents.y + extraHeightCollider), rayColor);
        return raycastHit.collider != null;
    }
*/
/* OVERLAP AREA METHOD
    private bool IsGrounded()
    {
        isGrounded = Physics2D.OverlapArea(new Vector2(transform.position.x -0.5f, transform.position.y - 0.5f),
        new Vector2(transform.position.x + 0.5f, transform.position.y - 0.51f), groundLayers);
    }
*/

/*
    private bool IsGrounded()
    {
        isGrounded = (Physics2D.OverlapBox(feet.position, feet.localScale, 0, filter, results) > 0 && movementVector.y < 0);
        return isGrounded;
    }
    */
//OVERLAP AREA METHOD
//  private bool IsGrounded()
//{
//isGrounded = Physics2D.OverlapArea(new Vector2(transform.position.x -0.5f, transform.position.y - 0.5f),
//new Vector2(transform.position.x + 0.5f, transform.position.y - (0.51f + extraHeightCollider)), groundLayers);
//return isGrounded;
//}
/*
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeightCollider, groundLayers);
        
        //FROM HERE DOWN IS JUST FOR DEVELOPMENT
        Color rayColor;
        if (raycastHit.collider != null) rayColor = Color.green;
        else rayColor = Color.red;
        Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + extraHeightCollider), rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + extraHeightCollider), rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y + extraHeightCollider), Vector2.right * (boxCollider.bounds.extents.x * 2), rayColor);
        return raycastHit.collider != null;
    }
*/
/*if (IsGrounded())
{
    movementVector.y = 0;
    Physics2D.OverlapBox(feet.position, feet.localScale, 0, filter, results);
    Vector2 surface = Physics2D.ClosestPoint(transform.position, results[0]) + Vector2.up * floorHeight;
    transform.position = new Vector3(transform.position.x, surface.y, transform.position.z);
}
*/

/*public void GroundedCheck()
{
    if (Physics2D.OverlapArea (new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f),
        new Vector2(transform.position.x + 0.5f, transform.position.y - (0.7f + _footAdjust)),
       _filter, _results) > 0 && _movementVector.y <= 0)
    {
        surface = Physics2D.ClosestPoint(transform.position, _results[0]) + Vector2.up * _floorHeight;
        //AppliedMovementVector_y = surface.y;
        _isGrounded = true;
    }
    else
    {
        _isGrounded = false;
    }

    public void GroundedCheck()
    {
        _raycastHit = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, _footAdjust, _layerMask);
        if(_raycastHit.collider != null) { _isGrounded = true; } else { _isGrounded = false; }

        Debug.Log(_isGrounded);

        //FROM HERE DOWN IS JUST FOR DEVELOPMENT
        Color rayColor;
        if (_raycastHit.collider != null) rayColor = Color.green;
        else rayColor = Color.red;
        Debug.DrawRay(_boxCollider.bounds.center + new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + _footAdjust), rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + _footAdjust), rayColor);
        Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y + _footAdjust), Vector2.right * (_boxCollider.bounds.extents.x * 2), rayColor);
        //Debug.Log(_raycastHit.point);
    }

    public void SnapFeet()
    {
        transform.position = new Vector3(transform.position.x, _raycastHit.point.y + _boxCollider.bounds.extents.y, transform.position.z);
    }
}*/