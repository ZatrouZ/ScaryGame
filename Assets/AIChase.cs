using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 3.0f;            // Enemy movement speed
    public float chaseDistance = 5.0f;    // Distance at which enemy starts chasing
    public float stopChaseDistance = 9.0f;// Distance at which enemy stops chasing

    private EnemyPatrol patrol;
    private float distance;

    private Animator enemyAnimator;        // Reference to the Animator component

    void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Find player if not assigned
        }

        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= chaseDistance)
        {
            patrol.enabled = false; // Disable patrol when chasing
            ChasePlayer();

            enemyAnimator.SetBool("isRunning", true);   // Set isRunning to true
            enemyAnimator.SetBool("isMoving", false);   // Disable walking animation
        }
        else if (distance >= stopChaseDistance)
        {
            patrol.enabled = true; // Re-enable patrol script
            enemyAnimator.SetBool("isRunning", false);  // Set isRunning to false (stop running)
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;

        // Smooth rotation towards player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);

        // Move towards player
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
