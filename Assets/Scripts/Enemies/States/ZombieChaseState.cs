using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : ZombieBaseState
{
    

    float randomDeviation;

    public ZombieChaseState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (context.ReachAttackCollider.IsColliding)
            context.ChangeState(factory.Attack());

    }

    public override void EnterState()
    {
        context.Animator.SetTrigger("Chase");
        context.Animator.SetBool("IsChasing", true);

        randomDeviation = Random.Range(0, 1);
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsChasing", false);
    }

    public override void UpdateState()
    {
        Vector3 playerPosition = context.Player.position;

        Vector3 randomSurroundPosition = new Vector3
            (
                playerPosition.x + (context.SurroundRadius * Mathf.Cos(2 * Mathf.PI * randomDeviation)),
                playerPosition.y,
                playerPosition.z + (context.SurroundRadius * Mathf.Sin(2 * Mathf.PI * randomDeviation))
            );

        context.Agent.SetDestination(randomSurroundPosition);

        UpdatePosition();

        CheckSwitchStates();
    }
}
