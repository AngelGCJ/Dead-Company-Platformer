using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) { }
    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }
    public override void UpdateState()
    {
        Deaccelerate(Ctx.DeAccelerationTimeReleaseInput);
        CheckSwitchStates();
    }

    public override void ExitState() { }
    public override void InitializeSubstate() { }
    public override void CheckSwitchStates()
    {
        if (Ctx.IsLateralMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Running());
        }
        else if (Ctx.IsLateralMovementPressed)
        {
            SwitchState(Factory.Walking());
        }
    }
    private void Deaccelerate(float accRate)
    {
        if (Ctx.CounterForAcc > 0)
        {
            //Ctx.FractionOfMaxVel = CalculeDeacceleration(Ctx.CounterForAcc * -1);
            Ctx.FractionOfMaxVel = -Mathf.Exp(Ctx.CounterForAcc * -1) + 1;
            Ctx.CounterForAcc -= Time.deltaTime * 10 / accRate / 2;
        }
        else
        {
            Ctx.FractionOfMaxVel = 0;
        }

        Ctx.LateralMovementModified += Ctx.WalkSpeed * Ctx.FractionOfMaxVel * Ctx.LazyDirection * Time.deltaTime;
    }
    /*
    private float CalculeDeacceleration(float x)
    {
        return -Mathf.Exp(x) + 1;
    }
    */
}