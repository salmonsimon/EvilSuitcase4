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

    [SerializeField] private AttackCollider attackCollider;

    // Add here all necessary data, components and gameobjects

    private void Awake()
    {
        stateFactory = new ZombieStateFactory(this);

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG).transform;
    }

    private void Start()
    {
        currentState = stateFactory.Chase();
        currentState.EnterState();
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

    #region Getters and Setters

    public ZombieBaseState CurrentState { get { return currentState; } set { currentState = value; } }

    public NavMeshAgent Agent { get { return agent; } }

    public Animator Animator { get { return animator; } } 

    public Transform Player { get { return player; } }

    public AttackCollider AttackCollider { get { return attackCollider; } }

    #endregion
}
