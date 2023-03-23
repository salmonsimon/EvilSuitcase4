using UnityEngine;

public class ZombieDeadState : ZombieBaseState
{
    public ZombieDeadState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) {}

    public override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        // Add dead animation and disable unneeded stuff
    }

    public override void UpdateState()
    {
        // don't do anything);
    }
}
