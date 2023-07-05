using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingZombieStandingState : CrawlingZombieBaseState
{
    private bool finishedStanding = false;

    private string animationToUse = string.Empty;

    public CrawlingZombieStandingState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory crawlingZombieStateFactory) : base(crawlingZombieStateMachine, crawlingZombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (finishedStanding && !context.ReachAttackCollider.IsColliding)
            context.ChangeState(factory.Chase());
        else if (finishedStanding && context.ReachAttackCollider.IsColliding)
            context.ChangeState(factory.Attack());
    }

    public override void EnterState()
    {
        finishedStanding = false;

        animationToUse = context.StandUpAnimation.name;

        context.RagdollSystem.RagdollMode = false;
        context.RagdollSystem.SetRagdoll(true, true);

        context.Animator.Play(animationToUse);
    }

    public override void ExitState()
    {
        context.Agent.enabled = true;

        context.RagdollSystem.MapCollider.isTrigger = false;
        context.RagdollSystem.MapCollider.enabled = true;
    }

    public override void UpdateState()
    {
        if (!context.Animator.GetCurrentAnimatorStateInfo(0).IsName(animationToUse))
        {
            finishedStanding = true;
        }

        CheckSwitchStates();
    }
}
