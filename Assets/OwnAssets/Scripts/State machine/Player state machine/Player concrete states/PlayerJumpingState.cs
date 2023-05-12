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
        Ctx.CoyoteTimer = Ctx.CoyoteTime + 1;
        InitializeSubstate();
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        HandleJump();
    }
    public override void UpdateState()
    {

        HandleGravity();
        //###############3
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
    }
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
        if(!Ctx.IsJumpPressed || Ctx.VerticalMovementCalculated < 0.0f)
        {
            SwitchState(Factory.Falling());
        }
    }

    void HandleJump()
    {
        Ctx.VerticalMovementCalculated = Ctx.InitialJumpVelocity;
        //Ctx.AppliedMovementVector_y = Ctx.InitialJumpVelocity;
    }

    public void HandleGravity()
    {
        float previousYVelocity = Ctx.VerticalMovementCalculated;
        Ctx.VerticalMovementCalculated = Ctx.VerticalMovementCalculated + (Ctx.Gravity * Time.deltaTime);
        Ctx.VerticalMovementCalculated = (previousYVelocity + Ctx.VerticalMovementCalculated) * 0.5f;
    }
}
