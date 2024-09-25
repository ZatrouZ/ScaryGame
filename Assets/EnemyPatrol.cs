using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float forceStrength = 10f;      // Speed or movement force
    public float stopDistance = 1f;        // How close the enemy needs to be to the patrol point before moving to the next one
    public Vector3[] patrolPoints;         // List of patrol points the enemy will patrol between
    public float maxSpeed = 5f;            // Maximum speed of the enemy

    private int currentPoint = 0;          // Index of the current patrol point
    private Rigidbody ourRigidbody;        // The Rigidbody attached to the enemy

    private Animator enemyAnimator;         // Reference to the Animator component

    void Awake()
    {
        ourRigidbody = GetComponent<Rigidbody>();
        ourRigidbody.useGravity = true;
        ourRigidbody.isKinematic = false;
    }

    void FixedUpdate()
    {
        if (patrolPoints.Length == 0) return;

        float distance = Vector3.Distance(transform.position, patrolPoints[currentPoint]);

        if (distance <= stopDistance)
        {
            currentPoint++;
            if (currentPoint >= patrolPoints.Length) currentPoint = 0;
        }

        Vector3 direction = (patrolPoints[currentPoint] - transform.position).normalized;

        if (direction.magnitude > 0.1f) // If the enemy is moving
        {
            enemyAnimator.SetBool("isMoving", true); // Set isMoving to true
        }
        else
        {
            enemyAnimator.SetBool("isMoving", false); // Set isMoving to false (idle)
        }

        // Apply force and clamp speed
        ourRigidbody.AddForce(direction * forceStrength);
        if (ourRigidbody.velocity.magnitude > maxSpeed)
        {
            ourRigidbody.velocity = ourRigidbody.velocity.normalized * maxSpeed;
        }

        // Smooth rotation towards patrol direction
        if (direction != Vector3.zero)
        {
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void OnDrawGizmos()
    {
        if (patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(patrolPoints[i], 0.5f);

                if (i > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(patrolPoints[i - 1], patrolPoints[i]);
                }
            }
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1], patrolPoints[0]);
        }
    }
}
