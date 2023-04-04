using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    public ZombieAttackState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!context.FollowAttackCollider.IsColliding)
        {
            context.ChangeState(factory.Chase());
            Debug.Log("Into chase state");
        }
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

        context.Agent.SetDestination(playerPosition);

        if(context.CloseAttackCollider.IsColliding)
            context.Animator.SetBool("OnReach", true);
        else
            context.Animator.SetBool("OnReach", false);

        UpdatePosition();

        CheckSwitchStates();
    }
}
