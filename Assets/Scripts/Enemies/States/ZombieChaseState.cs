using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : ZombieBaseState
{
    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

    public ZombieChaseState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (context.AttackCollider.IsColliding)
            context.ChangeState(factory.Attack());

    }

    public override void EnterState()
    {
        context.Animator.SetTrigger("Chase");
        context.Animator.SetBool("IsChasing", true);
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsChasing", false);
    }

    public override void UpdateState()
    {
        Vector3 playerPosition = context.Player.position;

        float randomDeviation = Random.Range(0, 1);

        Vector3 randomSurroundPosition = new Vector3
            (
                playerPosition.x + (context.SurroundRadius * Mathf.Cos(2 * Mathf.PI * randomDeviation)),
                playerPosition.y,
                playerPosition.z + (context.SurroundRadius * Mathf.Sin(2 * Mathf.PI * randomDeviation))
            );

        //context.Agent.SetDestination(playerPosition - (playerPosition - context.Agent.transform.position).normalized * .5f);
        context.Agent.SetDestination(randomSurroundPosition);

        UpdatePosition();

        CheckSwitchStates();
    }

    private void UpdatePosition()
    {
        Vector3 worldDeltaPosition = context.Agent.nextPosition - context.transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(context.transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(context.transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / .1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;

        if (context.Agent.remainingDistance <= context.Agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, context.Agent.remainingDistance / context.Agent.stoppingDistance);
        }

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > context.Agent.radius / 2f)
        {
            context.transform.position = Vector3.Lerp(context.Animator.rootPosition, context.Agent.nextPosition, smooth);
        }
    }
}
