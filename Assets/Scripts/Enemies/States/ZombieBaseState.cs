using UnityEngine;

public abstract class ZombieBaseState
{
    protected ZombieStateMachine context;
    protected ZombieStateFactory factory;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

    public ZombieBaseState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory)
    {
        context = zombieStateMachine;
        factory = zombieStateFactory;
    }

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void UpdateState();

    public abstract void CheckSwitchStates();

    protected void SwitchState(ZombieBaseState newState)
    {
        newState.EnterState();

        context.CurrentState = newState;
    }

    protected void UpdatePosition()
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
