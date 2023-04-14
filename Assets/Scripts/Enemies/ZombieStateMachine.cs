using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class ZombieStateMachine : MonoBehaviour
{
    [SerializeField] private ZombieBaseState currentState;
    private ZombieStateFactory stateFactory;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private HealthManager healthManager;
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

    private void Awake()
    {
        stateFactory = new ZombieStateFactory(this);

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        healthManager = GetComponent<HealthManager>();

        ragdollSystem = GetComponent<RagdollSystem>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG).transform;

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

    private void Start()
    {
        currentState = stateFactory.Chase();
        currentState.EnterState();

        healthManager.OnAliveStatusChange += OnAliveStatusChange;
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;

        rootPosition.y = agent.nextPosition.y;

        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    private void Update()
    {
        currentState.UpdateState();

        if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        {
            ChangeState(stateFactory.Ragdoll());
        }
    }

    public void ChangeState(ZombieBaseState newState)
    {
        currentState.ExitState();

        currentState = newState;
        currentState.EnterState();
    }

    private void OnAliveStatusChange()
    {
        if (healthManager.IsAlive)
            ChangeState(stateFactory.Chase());
        else
            ChangeState(stateFactory.Dead());
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

    public ZombieBaseState CurrentState { get { return currentState; } set { currentState = value; } }

    public NavMeshAgent Agent { get { return agent; } }

    public Animator Animator { get { return animator; } }

    public RagdollSystem RagdollSystem { get { return ragdollSystem; } }

    public Transform Player { get { return player; } }

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
