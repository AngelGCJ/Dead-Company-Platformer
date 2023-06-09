using System.Collections.Generic;

enum PlayerStates
{
    idle,
    walking,
    wallJump,
    grounded,
    jumping,
    falling,
    onWall
}
public class PlayerStateFactory
{
    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
    public PlayerStateFactory (PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.walking] = new PlayerWalkingState(_context, this);
        _states[PlayerStates.wallJump] = new PlayerWallJumpState(_context, this);
        _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.jumping] = new PlayerJumpingState(_context, this);
        _states[PlayerStates.falling] = new PlayerFallingState(_context, this);
        _states[PlayerStates.onWall] = new PlayerOnWallState(_context, this);
    }
    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }
    public PlayerBaseState Walking()
    {
        return _states[PlayerStates.walking];
    }
    public PlayerBaseState WallJump()
    {
        return _states[PlayerStates.wallJump];
    }
    public PlayerBaseState Jumping()
    {
        return _states[PlayerStates.jumping];
    }
    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }
    public PlayerBaseState Falling()
    {
        return _states[PlayerStates.falling];
    }
    public PlayerBaseState OnWall()
    {
        return _states[PlayerStates.onWall];
    }
}
