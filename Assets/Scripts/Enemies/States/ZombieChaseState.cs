using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    public ZombieChaseState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        // Set navmesh agent stuff
    }

    public override void UpdateState()
    {
        // follow player

        // check if entered attack collider and change to attack state
    }
}
