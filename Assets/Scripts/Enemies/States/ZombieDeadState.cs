using UnityEngine;

public class ZombieDeadState : ZombieBaseState
{
    public ZombieDeadState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) {}

    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {
        //context.Animator.SetTrigger("Death");
        context.RagdollSystem.SetRagdoll(false, true);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
