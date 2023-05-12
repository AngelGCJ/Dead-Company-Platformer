using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        HandleGravity();
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubstate()
    {
        if (!Ctx.IsLateralMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsLateralMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walking());
        }
        else
        {
            SetSubState(Factory.Running());
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
        Ctx.SnapFeet();
    }
}
