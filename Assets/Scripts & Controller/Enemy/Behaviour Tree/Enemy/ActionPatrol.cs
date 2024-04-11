using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class ActionPatrol : Node
{
    #region Fields

    // References to components
    private Transform transform;
    private Animator animator;
    private Transform[] waypoints;
    private NavMeshAgent agent;
    private Enemy enemy;
    private AISensor sensor;

    // Patrol-related variables
    private int currentWaypointIndex = 0;
    private float waitTime = 10f;
    private float waitCounter = 0f;
    private bool isWaiting = true;
    private int stepID;

    // Debug mode flag
    private bool debugMode;

    private int enemyType;

    #endregion

    #region Constructors

    // Constructor to initialize references and waypoints
    public ActionPatrol(bool debugMode, Transform transform, Transform[] waypoints, NavMeshAgent agent, int enemyType)
    {
        this.transform = transform;
        this.animator = transform.GetComponent<Animator>();
        this.waypoints = waypoints;
        this.agent = agent;
        enemy = transform.GetComponent<Enemy>();
        sensor = transform.GetComponent<AISensor>();
        this.debugMode = debugMode;
        this.enemyType = enemyType;
        stepID = transform.GetChild(0).gameObject.GetInstanceID();
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
            if (debugMode) Debug.Log("A - Patrol: RUNNING (game is paused)");
            state = NodeState.RUNNING;
            return state;
        }
        ////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////
        // FAILURE CHECKS
        ////////////////////////////////////////////////////////////////////////
        object obj = GetData("target");
        object lastKnownPos = GetData("lastKnownPosition");
        object noisePos = GetData("noisePosition");

        if (obj != null)
        {
            if (debugMode) Debug.Log("A - Patrol: FAILURE (target != null)");
            state = NodeState.FAILURE;
            return state;
        }
        else if (sensor.playerInSight && !sensor.hidden)
        {
            // Set state to FAILURE and return
            parent.SetData("target", GameManager.Instance.player.transform);
            if (debugMode) Debug.Log("A - Patrol: FAILURE (Player in Sight)");
            state = NodeState.FAILURE;
            return state;
        }
        else if (lastKnownPos != null)
        {
            // Set state to FAILURE and return
            if (debugMode) Debug.Log("A - Patrol: FAILURE (lastKnownPosition != null)");
            state = NodeState.FAILURE;
            return state;
        }
        else if (noisePos != null)
        {
            // Set state to FAILURE and return
            if (debugMode) Debug.Log("A - Patrol: FAILURE (noisePosition != null)");
            state = NodeState.FAILURE;
            return state;
        }
        ////////////////////////////////////////////////////////////////////////

        // Check if waiting
        if (isWaiting)
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
            waitCounter += Time.deltaTime;

            // Check if waiting time is over
            if (waitCounter >= waitTime)
            {
                isWaiting = false;
                animator.SetBool("walk", true);
                animator.SetBool("run", false);
            }
        }
        else
        {
            animator.SetBool("run", false);
            animator.SetBool("walk", true);

            Transform currentWaypoint = waypoints[currentWaypointIndex];

            // Check if reached the current waypoint
            if (Vector3.Distance(transform.position, currentWaypoint.position) < 0.1f)
            {
                transform.position = new Vector3(currentWaypoint.position.x, transform.position.y, currentWaypoint.position.z);
                waitCounter = 0f;
                isWaiting = true;

                // Switch to the next waypoint in a loop
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

                // Reset animation states
                animator.SetBool("run", false);
                animator.SetBool("walk", false);
                AudioManager.Instance.StopAudio(stepID);
            }
            else
            {
                // Move towards the current waypoint
                agent.speed = EnemyBT.walkSpeed;
                agent.SetDestination(currentWaypoint.position);
                animator.SetBool("walk", true);
                animator.SetBool("run", false);

                if (!AudioManager.Instance.IsPlaying(stepID)) AudioManager.Instance.PlayAudio(stepID);
                AudioManager.Instance.ToggleEnemyAudio(transform.gameObject, false, enemyType);
            }
        }

        // Return RUNNING to indicate that the action is ongoing
        if (debugMode) Debug.Log("A - Patrol: RUNNING (Patrolling)");
        state = NodeState.RUNNING;
        return state;
    }

    #endregion
}