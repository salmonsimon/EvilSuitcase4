using UnityEngine;

public class ZombieStateMachine : StateMachine
{
    private ZombieStateFactory stateFactory;

    private RagdollSystem ragdollSystem;

    [SerializeField] private AttackCollider reachAttackCollider;
    [SerializeField] private AttackCollider closeAttackCollider;
    [SerializeField] private AttackCollider followAttackCollider;
    [SerializeField] private float surroundRadius;

    [SerializeField] private GroundCheck fellCheck;

    [SerializeField] private AnimationClip standUpFromBellyAnimation;
    [SerializeField] private AnimationClip standUpFromBackAnimation;

    private Transform hipsBone;
    private Transform[] bones;
    private BoneTransform[] standUpFromBellyBoneTransforms;
    private BoneTransform[] standUpFromBackBoneTransforms;
    private BoneTransform[] ragdollBoneTransforms;

    protected override void Awake()
    {
        base.Awake();

        stateFactory = new ZombieStateFactory(this);

        ragdollSystem = GetComponent<RagdollSystem>();

        hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);

        bones = hipsBone.GetComponentsInChildren<Transform>();

        standUpFromBellyBoneTransforms = new BoneTransform[bones.Length];
        standUpFromBackBoneTransforms = new BoneTransform[bones.Length];
        ragdollBoneTransforms = new BoneTransform[bones.Length];

        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            standUpFromBellyBoneTransforms[boneIndex] = new BoneTransform();
            standUpFromBackBoneTransforms[boneIndex] = new BoneTransform();
            ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        PopulateAnimationStartBoneTransforms(standUpFromBellyAnimation, standUpFromBellyBoneTransforms);
        PopulateAnimationStartBoneTransforms(standUpFromBackAnimation, standUpFromBackBoneTransforms);
    }

    protected override void Start()
    {
        base.Start();

        currentState = stateFactory.Chase();
        currentState.EnterState();

        ragdollSystem.OnRagdollActivate += ActivateRagdoll;

        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
        {
            currentState = stateFactory.Chase();
            currentState.EnterState();
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;

        rootPosition.y = agent.nextPosition.y;

        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    private void ActivateRagdoll()
    {
        ChangeState(stateFactory.Ragdoll());
    }

    public void ChangeState(BaseState newState)
    {
        currentState.ExitState();

        currentState = newState;
        currentState.EnterState();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        ChangeState(stateFactory.Dead());
    }

    protected override void OnRevival()
    {
        base.OnRevival();

        ChangeState(stateFactory.Chase());
    }

    protected override void OnPlayerDeath()
    {
        base.OnPlayerDeath();

        if (HealthManager.IsAlive)
            ChangeState(stateFactory.PlayerDead());
    }

    public void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = bones[boneIndex].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransforms(AnimationClip animationClip, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        animationClip.SampleAnimation(gameObject, 0);
        PopulateBoneTransforms(boneTransforms);

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

    #region Getters and Setters

    public RagdollSystem RagdollSystem { get { return ragdollSystem; } }

    public AttackCollider ReachAttackCollider { get { return reachAttackCollider; } }

    public AttackCollider CloseAttackCollider { get { return closeAttackCollider; } }

    public AttackCollider FollowAttackCollider { get { return followAttackCollider; } }

    public float SurroundRadius { get { return surroundRadius; } }

    public GroundCheck FellCheck { get { return fellCheck; } }

    public AnimationClip StandUpFromBellyAnimation { get { return standUpFromBellyAnimation; } }

    public AnimationClip StandUpFromBackAnimation { get { return standUpFromBackAnimation; } }

    public Transform HipsBone { get { return hipsBone; } }

    public Transform[] Bones { get { return bones; } } 

    public BoneTransform[] StandUpFromBellyBoneTransforms { get { return standUpFromBellyBoneTransforms; } }
    public BoneTransform[] StandUpFromBackBoneTransforms { get { return standUpFromBackBoneTransforms; } }
    public BoneTransform[] RagdollBoneTransforms { get { return ragdollBoneTransforms; } }

    #endregion
}
