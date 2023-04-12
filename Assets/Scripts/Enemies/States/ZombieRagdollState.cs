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
        Debug.Log("Entering ragdoll state");

        context.Agent.enabled = false;

        context.RagdollSystem.SetRagdoll(false, true);
        readyToResetBones = false;

        timeUntilStanding = Random.Range(3f, maxTimeUntilStanding);
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
