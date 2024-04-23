using System;
using Unity.Mathematics;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action<bool, float> LeftWallGrabChanged;
        public event Action<bool, float> RightWallGrabChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>(); 
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetKeyDown(KeyCode.Space),
                JumpHeld = Input.GetKey(KeyCode.Space),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (_frameInput.Move.x != 0)
            {
                _timeAtHoldingWall = _time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();
            HandleWallInteraction();
            HandleJump();
            HandleDirection();
            HandleGravity();
            ApplyMovement();
        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private bool _touchingLeft;
        private bool _touchingRight;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            // Lateral contact
            bool LeftHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool RightHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Has Touched left
            if (!_touchingLeft && LeftHit)
            {
                _touchingLeft = true;
            }
            // Stoped touching left
            else if (_touchingLeft && !LeftHit)
            {
                _touchingLeft = false;
            }

            // Has Touched right
            if (!_touchingRight && RightHit)
            {
                _touchingRight = true;
            }
            // Stoped touching right
            else if (_touchingRight && !RightHit)
            {
                _touchingRight = false;
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region WallInteraction

        private bool _grabbingLeftWall;
        private bool _grabbingRightWall;
        private float _timeAtGrabbingWall;
        private float _timeAtHoldingWall;

        private void HandleWallInteraction()
        {
            bool holdingGrab = _time < _timeAtHoldingWall + _stats.WallGrabBuffer;

            bool wallGrabbingHeight = !Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.wallGrabHeight, ~_stats.PlayerLayer);

            //Grabs left wall
            if (!_grabbingLeftWall && _touchingLeft && holdingGrab && wallGrabbingHeight)
            {
                _grabbingLeftWall = true;
                LeftWallGrabChanged?.Invoke(true, Mathf.Abs(_frameVelocity.x));
                OnGrabWall();
            }
            //Left wall released
            else if (_grabbingLeftWall && (!_touchingLeft || _grounded))
            {
                _grabbingLeftWall = false;
                LeftWallGrabChanged?.Invoke(false, 0);
            }

            //Grabs right wall
            if (!_grabbingRightWall && _touchingRight && holdingGrab && wallGrabbingHeight)
            {
                _grabbingRightWall = true;
                RightWallGrabChanged?.Invoke(true, Mathf.Abs(_frameVelocity.x));
                OnGrabWall();
            }
            //Right wall released
            else if (_grabbingRightWall && (!_touchingRight || _grounded))
            {
                _grabbingRightWall = false;
                RightWallGrabChanged?.Invoke(false, 0);
            }
        }



        private void OnGrabWall()
        {
            _timeAtGrabbingWall = _time;
            _frameVelocity.y = MathF.Sign(_frameVelocity.y) * _stats.WallSliperyIndex;
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump)
            {
                return;
            }
            else
            {
                if (_grabbingLeftWall) ExecuteWallJump(1);
                else if (_grabbingRightWall) ExecuteWallJump(-1);

                if (_grounded || CanUseCoyote) ExecuteJump();

                _jumpToConsume = false;
            }
        }

        private void ExecuteJump()
        {
            JumpCommonBehaviour();
            _frameVelocity.y = _stats.JumpPower;
        }

        private void ExecuteWallJump(float direction = 1f)
        {
            JumpCommonBehaviour();
            _frameVelocity.y = _stats.JumpPower;
            _frameVelocity.x = _stats.JumpPower / 2 * direction;
        }

        private void JumpCommonBehaviour()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            Jumped?.Invoke();
        }

        #endregion




        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity
        private bool HasGripToWall => _time < _timeAtGrabbingWall + _stats.WallSlideTime;
        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f) _frameVelocity.y = _stats.GroundingForce;
            else
            {
                if (_grabbingLeftWall || _grabbingRightWall)
                {
                    if (HasGripToWall) _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, 0, _stats.FallAcceleration * _stats.FallWallMult * Time.fixedDeltaTime);
                    else _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed * _stats.WallSlideDownSpeed, _stats.FallAcceleration * _stats.WallSlideDownSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    var inAirGravity = _stats.FallAcceleration;
                    if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                    _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
                }
            }
        }
        #endregion

        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;

        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action<bool, float> LeftWallGrabChanged;

        public event Action<bool, float> RightWallGrabChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}