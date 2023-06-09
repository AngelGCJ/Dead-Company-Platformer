using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.SnapToWall();
        Ctx.HasJustWallJumped = false;
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }
    public override void UpdateState()
    {
        Deaccelerate(Ctx.DeAccelerationTimeReleaseInput);
        if (Ctx.WallPenetrationBug) Ctx.SnapToWall();
        CheckSwitchStates();
    }
    public override void ExitState()
    {
    }
    public override void InitializeSubstate() { }
    public override void CheckSwitchStates()
    {
        if (Ctx.IsLateralMovementPressed &&
            !(Ctx.WallOnLeft && Ctx.MonitorLeftInput || Ctx.WallOnRight && Ctx.MonitorRightInput)
            && !Ctx.HasJustWallJumped)
        {
            SwitchState(Factory.Walking());
        }
        if (Ctx.HasJustWallJumped && !Ctx.IsGrounded)
        {
            SwitchState(Factory.WallJump());
        }
    }
    private void Deaccelerate(float accRate)
    {
        if (!Ctx.WallOnLeft && !Ctx.WallOnRight)
        {
            if (Ctx.FractionOfMaxVel > 1)
            {
                Ctx.FractionOfMaxVel -= Time.deltaTime;
            }
            else if (Ctx.CounterForAcc > 0)
            {
                Ctx.FractionOfMaxVel = -Mathf.Exp(Ctx.CounterForAcc * -1) + 1;
                Ctx.CounterForAcc -= Time.deltaTime * 10 / accRate / 2;
            }
            else
            {
                Ctx.FractionOfMaxVel = 0;
            }
            Ctx.LateralMovementModified += Ctx.WalkSpeed * Ctx.FractionOfMaxVel * Ctx.LazyDirection * Time.deltaTime;
        }
        else
        {
            Ctx.FractionOfMaxVel = 0;
            Ctx.CounterForAcc = 0;
        }
    }
}