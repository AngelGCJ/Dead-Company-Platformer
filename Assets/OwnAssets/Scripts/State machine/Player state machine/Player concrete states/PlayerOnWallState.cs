using UnityEngine;

public class PlayerOnWallState : PlayerBaseState, IRootState
{
    public PlayerOnWallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    float _counter = 0;

    public override void EnterState()
    {
        Ctx.SnapToWall();
        if (Ctx.IsJumpPressed) { Ctx.RequireNewJumpPress = true; }
        InitializeSubstate();
        HandleGravity();
        _counter = 0;
    }
    public override void UpdateState()
    {
        WallSlide();
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        if (Ctx.IsJumpPressed) Ctx.HasJustWallJumped = true;
        Ctx.VerticalMovementCalculated = 0;
        Ctx.SnapToWall();
    }
    public override void InitializeSubstate()
    {
        SetSubState(Factory.Idle());
    }
    public override void CheckSwitchStates()
    {
        if (
            (!Ctx.IsGrounded &&
            ((Ctx.WallOnRight && !Ctx.MonitorRightInput) || (Ctx.WallOnLeft && !Ctx.MonitorLeftInput)))
            || (!Ctx.WallOnRight && !Ctx.WallOnLeft)
            )
        {
            SwitchState(Factory.Falling());
        }
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress &&
            !(Ctx.MonitorLeftInput && Ctx.MonitorRightInput))
        {
            SwitchState(Factory.Jumping());
        }
        if (Ctx.IsGrounded && !Ctx.WallPenetrationBug && _counter >= Ctx.TimeToWallSlide)
        {
            SwitchState(Factory.Grounded());
        }
    }
    public void HandleGravity()
    {
        Ctx.VerticalMovementCalculated = 0;
    }

    void WallSlide()
    {
        if (_counter < Ctx.TimeToWallSlide)
        {
            _counter += Time.deltaTime;
        }
        else
        {
            Ctx.VerticalMovementCalculated = -Ctx.WallSlideSpeed;
        }
    }
}
