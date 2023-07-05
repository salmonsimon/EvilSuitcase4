using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRagdollState : ZombieBaseState
{
    private float maxTimeUntilStanding = 5f;
    [SerializeField] private float timeUntilStanding = -1f;

    [SerializeField] private bool readyToResetBones;

    public ZombieRagdollState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (readyToResetBones)
        {
            context.ChangeState(factory.ResettingBones());
        }
    }

    public override void EnterState()
    {
        StateSetup();

        timeUntilStanding = Random.Range(3f, maxTimeUntilStanding);
    }

    public override void ExitState()
    {

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

    private void StateSetup()
    {
        context.RagdollSystem.ResetRagdoll();

        context.Agent.enabled = false;

        context.RagdollSystem.MapCollider.isTrigger = true;
        context.RagdollSystem.MapCollider.enabled = false;

        context.RagdollSystem.RagdollMode = true;
        context.RagdollSystem.SetRagdoll(false, true);
        readyToResetBones = false;

        context.DisableHitboxes();
    }
}