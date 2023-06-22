using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingZombieDeadState : CrawlingZombieBaseState
{
    public CrawlingZombieDeadState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory zombieStateFactory) : base(crawlingZombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {
        context.RagdollSystem.MapCollider.enabled = false;

        context.RagdollSystem.RagdollMode = true;

        context.RagdollSystem.ResetRagdoll();

        context.RagdollSystem.SetRagdoll(false, true);

        context.Agent.GetComponent<ZombieSFX>().PlayRandomDeathAudioClip();
    }

    public override void ExitState()
    {
        context.RagdollSystem.MapCollider.enabled = true;
        context.RagdollSystem.MapCollider.isTrigger = false;

        context.RagdollSystem.RagdollMode = false;
    }

    public override void UpdateState()
    {

    }
}
