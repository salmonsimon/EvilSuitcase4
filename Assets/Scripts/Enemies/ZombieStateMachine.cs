using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStateMachine : MonoBehaviour
{
    private ZombieBaseState currentState;
    private ZombieStateFactory stateFactory;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private HealthManager damageable;

    [SerializeField] private AttackCollider reachAttackCollider;
    [SerializeField] private AttackCollider closeAttackCollider;
    [SerializeField] private AttackCollider followAttackCollider;
    [SerializeField] private float surroundRadius;

    private void Awake()
    {
        stateFactory = new ZombieStateFactory(this);

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        damageable = GetComponent<HealthManager>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG).transform;
    }

    private void Start()
    {
        currentState = stateFactory.Chase();
        currentState.EnterState();

        damageable.OnAliveStatusChange += OnAliveStatusChange;
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
    }

    public void ChangeState(ZombieBaseState newState)
    {
        currentState.ExitState();

        currentState = newState;
        currentState.EnterState();
    }

    private void OnAliveStatusChange()
    { 
        if (damageable.IsAlive)
            ChangeState(stateFactory.Chase());
        else
            ChangeState(stateFactory.Dead());
    }

    #region Getters and Setters

    public ZombieBaseState CurrentState { get { return currentState; } set { currentState = value; } }

    public NavMeshAgent Agent { get { return agent; } }

    public Animator Animator { get { return animator; } } 

    public Transform Player { get { return player; } }

    public AttackCollider ReachAttackCollider { get { return reachAttackCollider; } }

    public AttackCollider CloseAttackCollider { get { return closeAttackCollider; } }

    public AttackCollider FollowAttackCollider { get { return followAttackCollider; } }

    public float SurroundRadius { get { return surroundRadius; } }

    #endregion
}
