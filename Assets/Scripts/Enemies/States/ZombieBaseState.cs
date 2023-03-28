public abstract class ZombieBaseState
{
    protected ZombieStateMachine context;
    protected ZombieStateFactory factory;

    public ZombieBaseState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory)
    {
        context = zombieStateMachine;
        factory = zombieStateFactory;
    }

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void UpdateState();

    public abstract void CheckSwitchStates();

    protected void SwitchState(ZombieBaseState newState)
    {
        newState.EnterState();

        context.CurrentState = newState;
    }
}
