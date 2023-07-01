using UnityEngine;

public abstract class BaseState
{
    protected Vector2 velocity;
    protected Vector2 smoothDeltaPosition;

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void UpdateState();

    public abstract void CheckSwitchStates();

    protected virtual void SwitchState(BaseState newState)
    {
        newState.EnterState();
    }
}
