using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStateMachine : MonoBehaviour
{
    private ZombieBaseState currentState;
    private ZombieStateFactory stateFactory;

    // Add here all necessary data, components and gameobjects

    private void Awake()
    {
        stateFactory = new ZombieStateFactory(this);
    }

    private void Start()
    {
        currentState = stateFactory.Chase();
        currentState.EnterState();
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    #region Getters and Setters

    public ZombieBaseState CurrentState { get { return currentState; } set { currentState = value; } }

    #endregion
}
