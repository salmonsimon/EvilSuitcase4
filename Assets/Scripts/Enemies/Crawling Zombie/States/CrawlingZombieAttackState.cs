using UnityEngine;

public class CrawlingZombieAttackState : CrawlingZombieBaseState
{
    public CrawlingZombieAttackState(CrawlingZombieStateMachine zombieStateMachine, CrawlingZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!context.FollowAttackCollider.IsColliding)
        {
            context.ChangeState(factory.Chase());
        }
    }

    public override void EnterState()
    {
        context.Animator.SetBool("IsAttacking", true);
        context.Animator.SetTrigger("Attack");
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsAttacking", false);
    }

    public override void UpdateState()
    {
        Vector3 playerPosition = context.Player.position;

        context.Agent.SetDestination(playerPosition);

        if(context.ReachAttackCollider.IsColliding)
            context.Animator.SetBool("OnReach", true);
        else
            context.Animator.SetBool("OnReach", false);

        UpdatePosition();

        CheckSwitchStates();
    }
}
