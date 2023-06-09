using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
: base(currentContext, playerStateFactory) { }

    bool _jumpedFromLeftWall;

    public override void EnterState()
    {
        _jumpedFromLeftWall = Ctx.WallOnLeft ? true : false;
        Ctx.FractionOfMaxVel = 2;
        Ctx.LazyDirection = Ctx.LateralMovementInput * -1;
    }
    public override void UpdateState()
    {

        //normalize:
        if(Ctx.FractionOfMaxVel >= 1)
        {
            Ctx.FractionOfMaxVel -= Time.deltaTime;
        }
        else
        {
            Ctx.FractionOfMaxVel = 0.999999f;
        }

        ManageCollision();

        float notZero = Ctx.LateralMovementInput == 0 ? Ctx.LazyDirection * -1 : Ctx.LateralMovementInput;

        //Ctx.LateralMovementModified += notZero * -1 * Ctx.WalkSpeed * Ctx.FractionOfMaxVel * Time.deltaTime;
        Ctx.LateralMovementModified += Ctx.LazyDirection * Ctx.WalkSpeed * Ctx.FractionOfMaxVel * Time.deltaTime;
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.SnapToWall();
        if ( Ctx.FractionOfMaxVel < 1)
        {
            Ctx.CounterForAcc = -Mathf.Log(-(Ctx.FractionOfMaxVel - 1f));
        }
    }
    public override void InitializeSubstate() { }
    public override void CheckSwitchStates()
    {
        /*
        if (!Ctx.IsLateralMovementPressed)
        {
            Ctx.HasJustWallJumped = false;
        }
        if (!Ctx.HasJustWallJumped)
        {
            SwitchState(Factory.Idle());
        }*/

        if (!Ctx.IsLateralMovementPressed ||
            (_jumpedFromLeftWall && Ctx.WallOnRight) ||
            (!_jumpedFromLeftWall && Ctx.WallOnRight))
        {
            SwitchState(Factory.Idle());
        }

    }

    private void ManageCollision()
    {
        if (Ctx.WallOnRight || Ctx.WallOnLeft)
        {
            //Ctx.LazyDirection = 0;
            Ctx.FractionOfMaxVel = 0;

        }
    }










}
