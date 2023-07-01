using UnityEngine;

public class CrawlingZombieChaseState : CrawlingZombieBaseState
{
    private float randomDeviation;

    private float timeToCheckSetup = 2f;

    public CrawlingZombieChaseState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory crawlingZombieStateFactory) : base(crawlingZombieStateMachine, crawlingZombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (context.ReachAttackCollider.IsColliding)
            context.ChangeState(factory.Attack());

    }

    public override void EnterState()
    {
        StateSetup();

        randomDeviation = Random.Range(0, 1);
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsChasing", false);
    }

    public override void UpdateState()
    {
        if (timeToCheckSetup > 0)
            timeToCheckSetup -= Time.deltaTime;
        else
        {
            CheckStateSetup();
            timeToCheckSetup = 2f;
        }

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

    private void StateSetup()
    {
        context.Agent.enabled = false;
        context.Agent.enabled = true;

        context.Animator.enabled = false;
        context.Animator.enabled = true;

        context.Animator.SetTrigger("Chase");
        context.Animator.SetBool("IsChasing", true);

        context.RagdollSystem.RagdollMode = false;
        context.RagdollSystem.SetRagdoll(true, true);
    }

    private void CheckStateSetup()
    {
        if (!context.Agent.enabled) context.Agent.enabled = true;

        if (!context.Animator.enabled) context.Animator.enabled = true;

        if (!context.Animator.GetBool("IsChasing"))
        {
            context.Animator.SetTrigger("Chase");
            context.Animator.SetBool("IsChasing", true);
        }

        if (context.RagdollSystem.RagdollMode)
        {
            context.RagdollSystem.RagdollMode = false;
            context.RagdollSystem.SetRagdoll(true, true);
        }
    }
}
