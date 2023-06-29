using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrawlingZombieDeadState : CrawlingZombieBaseState
{
    private bool onRagdollMode = false;

    public CrawlingZombieDeadState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory zombieStateFactory) : base(crawlingZombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {
        context.Agent.enabled = false;

        context.RagdollSystem.MapCollider.enabled = false;

        context.RagdollSystem.RagdollMode = true;

        context.RagdollSystem.ResetRagdoll();

        context.RagdollSystem.SetRagdoll(false, true);

        context.Agent.GetComponent<ZombieSFX>().PlayRandomDeathAudioClip();

        context.DisableHitboxes();
    }

    public override void ExitState()
    {
        context.RagdollSystem.MapCollider.enabled = true;
        context.RagdollSystem.MapCollider.isTrigger = false;

        context.RagdollSystem.RagdollMode = false;
    }

    public override void UpdateState()
    {
        if (!onRagdollMode && context.Animator.enabled == true)
        {
            context.RagdollSystem.RagdollMode = true;

            context.RagdollSystem.ResetRagdoll();

            context.RagdollSystem.SetRagdoll(false, true);

            context.Agent.enabled = false;

            onRagdollMode = true;
        }
    }
}
