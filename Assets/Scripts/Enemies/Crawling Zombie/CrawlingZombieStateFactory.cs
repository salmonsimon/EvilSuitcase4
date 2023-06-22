using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CrawlingZombieStates
{
    Chase,
    Attack,
    Ragdoll,
    ResettingBones,
    Standing,
    Dead,
    PlayerDead
}

public class CrawlingZombieStateFactory
{
    private CrawlingZombieStateMachine context;
    private Dictionary<CrawlingZombieStates, CrawlingZombieBaseState> states = new Dictionary<CrawlingZombieStates, CrawlingZombieBaseState>();

    public CrawlingZombieStateFactory(CrawlingZombieStateMachine currentContext)
    {
        context = currentContext;

        states[CrawlingZombieStates.Chase] = new CrawlingZombieChaseState(context, this);
        states[CrawlingZombieStates.Attack] = new CrawlingZombieAttackState(context, this);
        states[CrawlingZombieStates.Ragdoll] = new CrawlingZombieRagdollState(context, this);
        states[CrawlingZombieStates.ResettingBones] = new CrawlingZombieResettingBonesState(context, this);
        states[CrawlingZombieStates.Standing] = new CrawlingZombieStandingState(context, this);
        states[CrawlingZombieStates.Dead] = new CrawlingZombieDeadState(context, this);
        states[CrawlingZombieStates.PlayerDead] = new CrawlingZombiePlayerDeadState(context, this);
    }

    public CrawlingZombieBaseState Chase()
    {
        return states[CrawlingZombieStates.Chase];
    }

    public CrawlingZombieBaseState Attack()
    {
        return states[CrawlingZombieStates.Attack];
    }

    public CrawlingZombieBaseState Ragdoll()
    {
        return states[CrawlingZombieStates.Ragdoll];
    }

    public CrawlingZombieBaseState ResettingBones()
    {
        return states[CrawlingZombieStates.ResettingBones];
    }

    public CrawlingZombieBaseState Standing()
    {
        return states[CrawlingZombieStates.Standing];
    }

    public CrawlingZombieBaseState Dead()
    {
        return states[CrawlingZombieStates.Dead];
    }

    public CrawlingZombieBaseState PlayerDead()
    {
        return states[CrawlingZombieStates.PlayerDead];
    }
}
