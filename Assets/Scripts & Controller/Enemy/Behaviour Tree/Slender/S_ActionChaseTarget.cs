using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class S_ActionChaseTarget : Node
{
    #region Fields

    // References to components
    private Animator animator;
    private NavMeshAgent agent;
    private Enemy enemy;
    private Transform transform;
    private AISensor aiSensor;

    // Timer variables for chasing
    private float chaseTimer = 0f;
    private float chaseDuration = 10f; // Same as in Enemy.cs

    // Debug mode flag
    private bool debugMode;

    private int enemyType;

    #endregion

    #region Constructors

    // Constructor to initialize references
    public S_ActionChaseTarget(bool debugMode, Transform transform, NavMeshAgent agent, int enemyType)
    {
        this.transform = transform;
        animator = transform.GetComponent<Animator>();
        this.agent = agent;
        enemy = transform.GetComponent<Enemy>();
        this.debugMode = debugMode;
        aiSensor = transform.GetComponent<AISensor>();
        this.enemyType = enemyType;
    }

    #endregion

    #region Public Methods

    // Evaluate method to determine the state of the node
    public override NodeState Evaluate()
    {

        ////////////////////////////////////////////////////////////////////////
        // PAUSE GAME
        ////////////////////////////////////////////////////////////////////////
        if (GameManager.Instance.isPaused)
        {
            // Return RUNNING to indicate that the action is ongoing
            if (debugMode) Debug.Log("A - ChaseTarget: RUNNING (game is paused)");
            state = NodeState.RUNNING;
            return state;
        }
        ////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////
        // FAILURE CHECKS
        ////////////////////////////////////////////////////////////////////////
        object obj = GetData("target");

        if(obj == null)
        {
            // Set state to FAILURE and return
            if (debugMode) Debug.Log("A - ChaseTarget: FAILURE (No Target)");
            state = NodeState.FAILURE;
            return state;
        }
        else if (aiSensor.hidden)
        {
            // Set state to FAILURE and return
            if (debugMode) Debug.Log("A - ChaseTarget: FAILURE (Hidden)");
            ClearData("target");
            state = NodeState.FAILURE;
            return state;
        }
        ////////////////////////////////////////////////////////////////////////

        Transform target = (Transform)obj;

        agent.speed = SlenderBT.runSpeed;
        agent.SetDestination(target.position);
        animator.SetBool("walk", false);
        animator.SetBool("run", true);

        AudioManager.Instance.ToggleEnemyAudio(transform.gameObject, true, enemyType);

        // Update the chase timer
        chaseTimer += Time.deltaTime;

        // Check if the chase duration is over
        if (chaseTimer >= chaseDuration)
        {
            // Clear the target data, reset timer, set state to SUCCESS, and stop running animation
            ClearData("target");
            chaseTimer = 0f;
            animator.SetBool("run", false);
            animator.SetBool("walk", true);
            if (debugMode) Debug.Log("A - ChaseTarget: SUCCESS (chase duration over)");
            state = NodeState.SUCCESS;
            return state;
        }

        // If the chase is still ongoing, return RUNNING
        if (debugMode) Debug.Log("A - ChaseTarget: RUNNING (chasing)");
        state = NodeState.RUNNING;
        return state;
    }

    #endregion
}