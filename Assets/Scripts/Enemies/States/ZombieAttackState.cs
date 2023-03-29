using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    public ZombieAttackState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!context.AttackCollider.IsColliding)
            context.ChangeState(factory.Chase());
    }

    public override void EnterState()
    {
        context.Animator.SetTrigger("Attack");
        context.Animator.SetBool("IsAttacking", true);
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsAttacking", false);
    }

    public override void UpdateState()
    {
        Vector3 playerPosition = context.Player.position;

        context.Agent.SetDestination(playerPosition - (playerPosition - context.Agent.transform.position).normalized * .5f);

        CheckSwitchStates();
    }
}