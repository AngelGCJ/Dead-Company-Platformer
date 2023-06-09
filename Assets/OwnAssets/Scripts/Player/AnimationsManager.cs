using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsManager : MonoBehaviour
{
    [SerializeField] GameObject _idle, _run;
    [SerializeField] PlayerStateMachine _playerStateMachine;
    [SerializeField] ShootAnimationManager _inIdle, _WhileRunning;
    ShootAnimationManager _currentAnimManager;


    private void Awake()
    {
        EventManager.AddListener<float>(ENUM_PlayerActions.shoot, ShootAnim);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<float>(ENUM_PlayerActions.shoot, ShootAnim);
    }

    private void Update()
    {
        if(_playerStateMachine.FractionOfMaxVel != 0) //running
        {
            _idle.SetActive(false);
            _run.SetActive(true);
            _currentAnimManager = _WhileRunning;
        }
        else //idle
        {
            _idle.SetActive(true);
            _run.SetActive(false);
            _currentAnimManager = _inIdle;
        }

        if(_playerStateMachine.LateralMovementInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _currentAnimManager._facingRight = true;
        }
        else if (_playerStateMachine.LateralMovementInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _currentAnimManager._facingRight = false;
        }
    }

    void ShootAnim(float time)
    {
        if(_currentAnimManager != null)
        {
            _currentAnimManager.Shoot(time);
        }
    }
}
