using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePlayerDeadState : ZombieBaseState
{
    public ZombiePlayerDeadState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    Transform playerSpine;

    public override void CheckSwitchStates()
    {

    }

    public override void EnterState()
    {
        context.Animator.SetTrigger("DeadPlayer");

        playerSpine = context.Player.GetComponent<PlayerHealthAnimations>().PlayerSpine;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        context.Agent.SetDestination(playerSpine.position);

        if (Vector3.Distance(context.Agent.transform.position, playerSpine.position) < 1f)
            context.Animator.SetBool("OnReach", true);
        else
            context.Animator.SetBool("OnReach", false);

        UpdatePosition();

        CheckSwitchStates();
    }
}
