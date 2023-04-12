using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class ZombieStandingState : ZombieBaseState
{
    private bool finishedStanding = false;

    private bool isFacingDown = false;
    private string animationToUse = string.Empty;

    public ZombieStandingState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (finishedStanding && !context.ReachAttackCollider.IsColliding)
            context.ChangeState(factory.Chase());
        else if (finishedStanding && context.ReachAttackCollider.IsColliding)
            context.ChangeState(factory.Attack());
    }

    public override void EnterState()
    {
        Debug.Log("Entering standing state");

        finishedStanding = false;

        isFacingDown = context.FellCheck.transform.forward.y < 0;

        if (isFacingDown)
            animationToUse = context.StandUpFromBellyAnimation.name;
        else
            animationToUse = context.StandUpFromBackAnimation.name;

        context.RagdollSystem.SetRagdoll(true, true);

        context.Animator.Play(animationToUse, -1, 0);
    }

    public override void ExitState()
    {
        Debug.Log("Exiting standing state");

        context.Agent.enabled = true;
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
