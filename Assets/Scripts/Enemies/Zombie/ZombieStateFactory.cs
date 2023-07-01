using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ZombieStates
{
    Chase,
    Attack,
    Ragdoll,
    ResettingBones,
    Standing,
    Dead,
    PlayerDead
}

public class ZombieStateFactory
{

    private ZombieStateMachine context;
    private Dictionary<ZombieStates, ZombieBaseState> states = new Dictionary<ZombieStates, ZombieBaseState>();

    public ZombieStateFactory(ZombieStateMachine currentContext)
    {
        context = currentContext;

        states[ZombieStates.Chase] = new ZombieChaseState(context, this);
        states[ZombieStates.Attack] = new ZombieAttackState(context, this);
        states[ZombieStates.Ragdoll] = new ZombieRagdollState(context, this);
        states[ZombieStates.ResettingBones] = new ZombieResettingBonesState(context, this);
        states[ZombieStates.Standing] = new ZombieStandingState(context, this);
        states[ZombieStates.Dead] = new ZombieDeadState(context, this);
        states[ZombieStates.PlayerDead] = new ZombiePlayerDeadState(context, this);
    }

    public ZombieBaseState Chase()
    {
        return states[ZombieStates.Chase];
    }

    public ZombieBaseState Attack()
    {
        return states[ZombieStates.Attack];
    }

    public ZombieBaseState Ragdoll()
    {
        return states[ZombieStates.Ragdoll];
    }

    public ZombieBaseState ResettingBones()
    {
        return states[ZombieStates.ResettingBones];
    }

    public ZombieBaseState Standing()
    {
        return states[ZombieStates.Standing];
    }

    public ZombieBaseState Dead()
    {
        return states[ZombieStates.Dead];
    }

    public ZombieBaseState PlayerDead()
    {
        return states[ZombieStates.PlayerDead];
    }
}
