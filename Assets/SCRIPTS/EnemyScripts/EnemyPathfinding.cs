using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    public Transform player;            // The player's transform
    public float chaseDistance = 10f;   // Distance within which the enemy starts chasing
    public float stoppingDistance = 2f; // Distance where the enemy stops moving toward the player
    public float patrolSpeed = 2f;      // Speed of the enemy while patrolling
    public float chaseSpeed = 3.5f;     // Speed of the enemy when chasing
    public Transform[] patrolPoints;    // Patrol points for the enemy to move between
    public LayerMask obstacleLayer;     // Layer mask to check for obstacles like walls

    private int currentPatrolIndex = 0; // Patrol point index
    private NavMeshAgent agent;         // NavMeshAgent for pathfinding
    private Animator anim;              // Animator for controlling animations
    private EnemyPatrol enemyPatrol;    // Reference to the EnemyPatrol script
    private bool isChasing = false;     // Whether the enemy is currently chasing the player
    private bool isPatrolling = true;   // Whether the enemy is currently patrolling
    private bool isObstacleBlocking = false;  // Whether an obstacle is blocking the path

    public float rotationSpeed = 5f;    // Speed at which the enemy rotates towards the player

    void Start()
    {
        // Initialize components
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponent<EnemyPatrol>();  // Get reference to EnemyPatrol script

        agent.speed = patrolSpeed;      // Set patrol speed initially
        PatrolNextPoint();              // Start patrolling
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within chase distance and there are no obstacles in the way, start chasing
        if (distanceToPlayer <= chaseDistance && !ObstacleInWay())
        {
            isChasing = true;
            isPatrolling = false;
            agent.speed = chaseSpeed;   // Set chase speed

            // Disable the EnemyPatrol script when chasing the player
            if (enemyPatrol != null && enemyPatrol.enabled)
            {
                enemyPatrol.enabled = false;
            }

            // Set destination to the player's position
            agent.SetDestination(player.position);

            // Rotate the enemy to face the player
            RotateTowards(player.position);
        }
        // If the player is too far away, stop chasing and resume patrol
        else if (distanceToPlayer > chaseDistance || ObstacleInWay())
        {
            isChasing = false;
            isPatrolling = true;
            agent.speed = patrolSpeed;  // Reset to patrol speed

            // Enable the EnemyPatrol script when returning to patrol
            if (enemyPatrol != null && !enemyPatrol.enabled)
            {
                enemyPatrol.enabled = true;
            }

            PatrolNextPoint();
        }

        // Handle animations based on state
        HandleAnimations();

        // Stop chasing if the player is close enough
        if (isChasing && distanceToPlayer <= stoppingDistance)
        {
            agent.isStopped = true; // Stop the NavMeshAgent from moving further
        }
        else
        {
            agent.isStopped = false; // Allow movement
        }
    }

    void HandleAnimations()
    {
        // Set the animations based on the state of the enemy
        if (isChasing)
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isMoving", false);
        }
        else if (isPatrolling)
        {
            anim.SetBool("isMoving", true);
            anim.SetBool("isRunning", false);
        }
    }

    void PatrolNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        // Set the NavMeshAgent's next destination to the next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        // Loop through patrol points
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    bool ObstacleInWay()
    {
        // Check if there's an obstacle between the enemy and the player
        RaycastHit hit;
        Vector3 direction = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, direction, out hit, chaseDistance, obstacleLayer))
        {
            // If the ray hits something that isn't the player, return true (obstacle detected)
            if (hit.transform != player)
            {
                isObstacleBlocking = true;  // Update this field based on raycast result
                return true;
            }
        }

        isObstacleBlocking = false;  // No obstacle detected
        return false;
    }

    void RotateTowards(Vector3 target)
    {
        // Calculate the direction to the target (player)
        Vector3 direction = (target - transform.position).normalized;

        // Ignore the Y-axis (height) to ensure only horizontal rotation
        direction.y = 0;

        // Calculate the target rotation based on the direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the chase distance in the Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
