using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class CrawlingZombieRagdollState : CrawlingZombieBaseState
{
    private float maxTimeUntilStanding = 5f;
    [SerializeField] private float timeUntilStanding = -1f;

    [SerializeField] private bool readyToResetBones;

    public CrawlingZombieRagdollState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory crawlingZombieStateFactory) : base(crawlingZombieStateMachine, crawlingZombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (readyToResetBones)
        {
            context.ChangeState(factory.ResettingBones());
        }
    }

    public override void EnterState()
    {
        context.Agent.enabled = false;

        context.RagdollSystem.MapCollider.isTrigger = true;
        context.RagdollSystem.MapCollider.enabled = false;

        context.RagdollSystem.RagdollMode = true;
        context.RagdollSystem.SetRagdoll(false, true);
        readyToResetBones = false;

        timeUntilStanding = Random.Range(3f, maxTimeUntilStanding);

        Debug.Log("Entering ragdoll state");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting ragdoll state");
    }

    public override void UpdateState()
    {
        if (context.FellCheck.IsColliding)
            timeUntilStanding -= Time.deltaTime;

        if (timeUntilStanding < 0)
        {
            readyToResetBones = true;
        }

        CheckSwitchStates();
    }
}
