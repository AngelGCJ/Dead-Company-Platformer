using UnityEngine;

public class PlayerJumpingState : PlayerBaseState, IRootState
{
    public PlayerJumpingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;

    }
    public override void EnterState()
    {
        InitializeSubstate();
        //Set acceleration and deacceleration values in jump
        //also in falling?
        Ctx.AccelerationTime = Ctx.AccAir;
        Ctx.DeAccelerationTimeTurning = Ctx.DeAccAirTurning;
        Ctx.DeAccelerationTimeReleaseInput = Ctx.DeAccAir;
        //
        Ctx.CoyoteTimer = Ctx.CoyoteTime + 1; //would never be able to use coyote time if has jumped
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        if (Ctx.HasJustWallJumped) HandleWallJump();
        HandleJump();
    }
    public override void UpdateState()
    {
        HandleGravity();
        //
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
    }
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
        if(!Ctx.IsJumpPressed || Ctx.VerticalMovementCalculated < Ctx.JumpToFallEarly) //zero?
        {
            SwitchState(Factory.Falling());
        }
        if ((Ctx.WallOnLeft && Ctx.MonitorLeftInput) || (Ctx.WallOnRight && Ctx.MonitorRightInput) ||
            Ctx.HasJustWallJumped && (Ctx.WallOnRight || Ctx.WallOnLeft))
        {
            SwitchState(Factory.OnWall());
        }
    }

    void HandleJump()
    {
        Ctx.VerticalMovementCalculated = Ctx.InitialJumpVelocity;
        //Ctx.AppliedMovementVector_y = Ctx.InitialJumpVelocity;
    }

    void HandleWallJump()
    {
        Ctx.LateralMovementModified += Ctx.WalkSpeed * Ctx.LateralMovementInput * -1 * Time.deltaTime;
    }

    public void HandleGravity()
    {
        float previousYVelocity = Ctx.VerticalMovementCalculated;
        Ctx.VerticalMovementCalculated = Ctx.VerticalMovementCalculated + (Ctx.Gravity * Time.deltaTime);
        Ctx.VerticalMovementCalculated = (previousYVelocity + Ctx.VerticalMovementCalculated) * 0.5f;
    }
}
