using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{
    [SerializeField] protected BaseState currentState;
    public BaseState CurrentState { get { return currentState; } set { currentState = value; } }

    protected NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    protected Transform player;
    public Transform Player { get { return player; } }

    protected Animator animator;
    public Animator Animator { get { return animator; } }

    protected HealthManager healthManager;
    public HealthManager HealthManager { get { return healthManager; } }

    protected bool initialized = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        healthManager = GetComponent<HealthManager>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG).transform;
    }

    protected virtual void Start()
    {
        healthManager.OnDeath += OnDeath;
        healthManager.OnRevival += OnRevival;

        player.GetComponent<HealthManager>().OnDeath += OnPlayerDeath;
    }

    protected virtual void Update()
    {
        currentState.UpdateState();
    }

    protected virtual void OnDeath()
    {
        
    }

    protected virtual void OnRevival()
    {
        Animator.enabled = false;
        Animator.enabled = true;
    }

    protected virtual void OnPlayerDeath()
    {

    }
}
