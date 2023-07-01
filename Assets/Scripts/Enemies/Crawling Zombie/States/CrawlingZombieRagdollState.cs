using UnityEngine;

public class CrawlingZombieRagdollState : CrawlingZombieBaseState
{
    private float maxTimeUntilStanding = 5f;
    [SerializeField] private float timeUntilStanding = -1f;

    [SerializeField] private bool readyToResetBones;

    private float timeToCheckSetup = 2f;

    public CrawlingZombieRagdollState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory crawlingZombieStateFactory) : base(crawlingZombieStateMachine, crawlingZombieStateFactory) { }

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
        if (timeToCheckSetup > 0)
            timeToCheckSetup -= Time.deltaTime;
        else
        {
            CheckStateSetup();
            timeToCheckSetup = 2f;
        }

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
        context.Agent.enabled = false;

        context.RagdollSystem.MapCollider.isTrigger = true;
        context.RagdollSystem.MapCollider.enabled = false;

        context.RagdollSystem.RagdollMode = true;
        context.RagdollSystem.SetRagdoll(false, true);
        readyToResetBones = false;

        context.DisableHitboxes();
    }

    private void CheckStateSetup()
    {
        if (context.Agent.enabled)
            context.Agent.enabled = false;

        if (context.Animator.enabled)
        {
            context.RagdollSystem.RagdollMode = true;
            context.RagdollSystem.SetRagdoll(false, true);
        }

        if (!context.RagdollSystem.MapCollider.isTrigger)
            context.RagdollSystem.MapCollider.isTrigger = true;

        if (context.RagdollSystem.MapCollider.enabled)
            context.RagdollSystem.MapCollider.enabled = false;
    }
}
