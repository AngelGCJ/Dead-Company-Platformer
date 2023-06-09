using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) { }

    bool _withMomentum = false;
    Queue<float> _lazyInputQueue = new Queue<float>();

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);

        if (Ctx.FractionOfMaxVel > 0f)
        {
            _withMomentum = true;
        }
        if (Ctx.WallOnLeft || Ctx.WallOnRight)
        {
            _withMomentum = false;
            Ctx.FractionOfMaxVel = 0f;
            Ctx.CounterForAcc = 0f;
        }
    }
    public override void UpdateState()
    {
        if (_withMomentum && (Ctx.LazyDirection != Ctx.LateralMovementInput))
        {
            Deaccelerate(Ctx.DeAccelerationTimeTurning);
        }
        else
        {
            Accelerate(Ctx.AccelerationTime);
        }
        ActualizeQueue();    
        //check should remain at the end
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.LazyDirection = _lazyInputQueue.Peek();
    }
    public override void InitializeSubstate() { }
    public override void CheckSwitchStates()
    {
        if ( !Ctx.IsLateralMovementPressed ||
            Ctx.LateralMovementInput != _lazyInputQueue.Peek() ||
            (Ctx.WallOnRight && Ctx.MonitorRightInput || Ctx.WallOnLeft && Ctx.MonitorLeftInput))
        {
            SwitchState(Factory.Idle());
        }
    }
    /*===|===|===|===|===|===|===|===|===[ LOCAL BEHAVIOUR ]===|===|===|===|===|===|===|===|===*/
    float _targetSpeed;
    private void Accelerate(float accRate)
    {
        _targetSpeed = Ctx.LateralMovementInput * Ctx.WalkSpeed;
        if (Ctx.CounterForAcc < 5)
        {
            Ctx.FractionOfMaxVel = -Mathf.Exp(-Ctx.CounterForAcc) + 1;
            Ctx.CounterForAcc += Time.deltaTime * 10 / accRate / 2;
        }
        else
        {
            Ctx.FractionOfMaxVel = 1;
        }
        ManageWallCollision();
        Ctx.LateralMovementModified += _targetSpeed * Ctx.FractionOfMaxVel * Time.deltaTime;
    }
    private void Deaccelerate(float accRate)
    {
        if (Ctx.CounterForAcc > 0)
        {
            Ctx.FractionOfMaxVel = -Mathf.Exp(Ctx.CounterForAcc * -1) + 1;
            Ctx.CounterForAcc -= Time.deltaTime * 10 / accRate / 2;
        }
        else
        {
            Ctx.FractionOfMaxVel = 0;
        }
        ManageWallCollision();
        Ctx.LateralMovementModified += Ctx.WalkSpeed * Ctx.FractionOfMaxVel * Ctx.LateralMovementInput * -1 * Time.deltaTime;
        if(Ctx.FractionOfMaxVel < 0.3f) { _withMomentum = false; }
    }
    private void ActualizeQueue()
    {
        _lazyInputQueue.Enqueue(Ctx.LateralMovementInput != 0 ? Ctx.LateralMovementInput :
            (_lazyInputQueue.Count <1 ? 1 : _lazyInputQueue.Peek()));
        Ctx.StartCoroutine(Dequeue());
    }
    public IEnumerator Dequeue()
    {
        yield return new WaitForSeconds(Time.deltaTime * 5);
        if (_lazyInputQueue.Count > 0) _lazyInputQueue.Dequeue();
    }
    public void ManageWallCollision()
    {
        if (Ctx.WallOnLeft && _targetSpeed < 0)
        {
            Ctx.FractionOfMaxVel = 0f;
            _targetSpeed = 0;

        }
        if (Ctx.WallOnRight && _targetSpeed > 0)
        {
            Ctx.FractionOfMaxVel = 0f;
            _targetSpeed = 0;
        }
    }
}
