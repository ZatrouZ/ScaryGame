using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChase : MonoBehaviour
{
    public GameObject player;
    public float chaseSpeed = 6.0f;               // Chasing speed
    public float chaseDistance = 5.0f;            // Distance to start chasing
    public float stopChaseDistance = 9.0f;        // Distance to stop chasing

    private EnemyPatrol patrol;                   // Reference to patrol script
    private float distance;                       // Distance to player

    public Animator enemyAnimator;                // Reference to Animator

    private Vector3 initialPosition;              // To store initial Y position of the enemy

    void Start()
    {
        patrol = GetComponent<EnemyPatrol>();     // Reference the patrol script
        initialPosition = transform.position;     // Store the initial position of the enemy
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Find the player if not already assigned
        }

        // Calculate distance between enemy and player
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= chaseDistance) // If close enough, start chasing
        {
            patrol.enabled = false;              // Disable patrol while chasing
            ChasePlayer();                       // Start chasing

            // Set animation parameters
            enemyAnimator.SetBool("isRunning", true);
            enemyAnimator.SetBool("isMoving", false);  // Ensure walking is false when chasing
        }
        else if (distance >= stopChaseDistance) // If far enough, stop chasing
        {
            patrol.enabled = true;               // Enable patrol
            enemyAnimator.SetBool("isRunning", false); // Stop running
        }
        else if (patrol != null && patrol)//.isPatrolling//) // If patrolling
        {
            enemyAnimator.SetBool("isMoving", true);   // Walk
            enemyAnimator.SetBool("isRunning", false); // Ensure running is false
        }
        else // Idle if not walking or running
        {
            enemyAnimator.SetBool("isMoving", false);
            enemyAnimator.SetBool("isRunning", false);
        }
    }

    void ChasePlayer()
    {
        // Calculate direction to the player
        Vector3 direction = player.transform.position - transform.position;

        // Keep the enemy at the same Y position to prevent floating or jumping
        direction.y = 0;

        // Rotate to face the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);

        // Move towards the player without affecting the Y position
        Vector3 newPosition = Vector3.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);

        // Ensure the Y position stays constant (keep enemy grounded)
        newPosition.y = initialPosition.y;

        transform.position = newPosition;
    }
}
