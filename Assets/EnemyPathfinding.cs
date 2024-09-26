using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  // Required for NavMesh

public class EnemyPathfinding : MonoBehaviour
{
    public Transform player;                // Reference to the player
    public float chaseDistance = 10f;       // Distance at which the enemy starts chasing
    public float stopChaseDistance = 15f;   // Distance at which the enemy stops chasing
    public float patrolSpeed = 3.0f;        // Speed during patrol
    public float chaseSpeed = 6.0f;         // Speed while chasing

    private NavMeshAgent navMeshAgent;      // NavMeshAgent for pathfinding
    private EnemyPatrol patrol;             // Reference to the patrol script (if applicable)
    private Animator enemyAnimator;         // Reference to the Animator

    private bool isChasing = false;         // To track if the enemy is chasing the player
    private float distanceToPlayer;         // Distance to the player

    void Start()
    {
        // Get the NavMeshAgent component attached to this enemy
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Get the patrol script and Animator if applicable
        patrol = GetComponent<EnemyPatrol>();
        enemyAnimator = GetComponent<Animator>();

        // Set the initial speed for the agent
        navMeshAgent.speed = patrolSpeed;
    }

    void Update()
    {
        if (player == null)
        {
            // Find the player object by tag if not already assigned
            player = GameObject.FindWithTag("Player").transform;
        }

        // Calculate the distance to the player
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            StartChasing();
        }
        else if (distanceToPlayer >= stopChaseDistance)
        {
            StopChasing();
        }

        // Update animations based on movement
        UpdateAnimations();
    }

    void StartChasing()
    {
        isChasing = true;
        navMeshAgent.speed = chaseSpeed;  // Increase speed while chasing
        navMeshAgent.SetDestination(player.position);  // Pathfinding to player's position

        // Stop patrol if active
        if (patrol != null)
        {
            patrol.enabled = false;
        }
    }

    void StopChasing()
    {
        isChasing = false;
        navMeshAgent.speed = patrolSpeed;  // Reset speed to patrol speed

        // If a patrol script exists, re-enable it when the enemy stops chasing
        if (patrol != null)
        {
            patrol.enabled = true;
        }
    }

    void UpdateAnimations()
    {
        // Set the "isMoving" boolean based on whether the agent is moving
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;

        // Set the "isRunning" boolean based on whether the enemy is chasing
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("isMoving", !isChasing && isMoving);   // Walking animation if patrolling
            enemyAnimator.SetBool("isRunning", isChasing && isMoving);   // Running animation if chasing
        }
    }
}
