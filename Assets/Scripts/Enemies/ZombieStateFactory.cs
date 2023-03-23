using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ZombieStates
{
    Chase,
    Attack,
    Dead
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
        states[ZombieStates.Dead] = new ZombieDeadState(context, this);
    }

    public ZombieBaseState Chase()
    {
        return states[ZombieStates.Chase];
    }

    public ZombieBaseState Attack()
    {
        return states[ZombieStates.Attack];
    }

    public ZombieBaseState Dead()
    {
        return states[ZombieStates.Dead];
    }
}
