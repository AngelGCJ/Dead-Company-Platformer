using UnityEngine;
public class PlayerFallingState : PlayerBaseState, IRootState
{
    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubstate();
        Ctx.Animator.SetBool(Ctx.IsFallingHash, true);
    }
    public override void UpdateState()
    {

        HandleGravity();
        Ctx.CoyoteTimer += Time.deltaTime; //for CoyoteTime
        if(Ctx.JumpBufferTimer >= 0) Ctx.JumpBufferTimer -= Time.deltaTime; //for jumpBuffer

        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsFallingHash, false);
        if (Ctx.IsJumpPressed) { Ctx.RequireNewJumpPress = true; }
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
        if (Ctx.IsGrounded && !Ctx.WallPenetrationBug)
        {
            SwitchState(Factory.Grounded());
        }
        if (Ctx.CoyoteTimer < Ctx.CoyoteTime && Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jumping());
        }
        if ((Ctx.WallOnLeft && Ctx.MonitorLeftInput) || (Ctx.WallOnRight && Ctx.MonitorRightInput))
        {
            SwitchState(Factory.OnWall());
        }
    }
    public void HandleGravity()
    {
        float previousYVelocity = Ctx.VerticalMovementCalculated;
        Ctx.VerticalMovementCalculated = Ctx.VerticalMovementCalculated + (Ctx.Gravity * Ctx.FallMultiplier * Time.deltaTime);
        Ctx.VerticalMovementCalculated = Mathf.Max((previousYVelocity + Ctx.VerticalMovementCalculated) * 0.5f, Ctx.FallSpeedClamp);
    }
}
