public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubstate();
        //Set acceleration and deacceleration values on ground
        Ctx.AccelerationTime = Ctx.AccGround;
        Ctx.DeAccelerationTimeTurning = Ctx.DeAccGroundTurning;
        Ctx.DeAccelerationTimeReleaseInput = Ctx.DeAccGround;
        //
        Ctx.HasJustWallJumped = false;
        HandleGravity();
        Ctx.SnapToWall();
    }
    public override void UpdateState()
    {



        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubstate()
    {
        if (Ctx.HasJustWallJumped)
        {
            SetSubState(Factory.WallJump());
        }
        else if (Ctx.IsLateralMovementPressed)
        {
            SetSubState(Factory.Walking());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }
    public override void CheckSwitchStates()
    {
        if (Ctx.JumpBufferTimer > 0)
        {
            Ctx.JumpBufferTimer = 0;
            SwitchState(Factory.Jumping());
        }
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState (Factory.Jumping());
        }
        if (!Ctx.IsGrounded)
        {
            Ctx.CoyoteTimer = 0;
            SwitchState (Factory.Falling());
        }
    }
    public void HandleGravity()
    {
        Ctx.VerticalMovementCalculated = 0;

        //ALL THIS TO TRY TO REMOVE THE BUG:
        Ctx.SnapToWall();
        if (Ctx.IsGrounded)
        {
            if (!Ctx.WallPenetrationBug) Ctx.SnapFeet();
        }

    }



}