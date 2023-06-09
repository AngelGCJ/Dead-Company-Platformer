using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public PlayerRunningState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) { }
    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
    }   
    public override void UpdateState()
    {
        Ctx.LateralMovementModified += Ctx.LateralMovementInput * Ctx.RunSpeed * Time.deltaTime;

        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubstate() { }
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsLateralMovementPressed)
        {
            SwitchState(Factory.Idle());
        }

    }
}

