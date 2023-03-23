using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    public ZombieAttackState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        // start attacking
    }

    public override void UpdateState()
    {
        // update time until next attack

        // check if player left attack collider to get back to chase state
    }
}
