using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float patrolSpeed = 3f;               // Patrol movement speed
    public float stopDistance = 1f;              // Distance to stop at patrol points
    public Vector3[] patrolPoints;               // Patrol points
    public float maxSpeed = 5f;                  // Maximum movement speed

    private int currentPoint = 0;                // Current patrol point index
    private Rigidbody ourRigidbody;              // Rigidbody for movement
    public bool isPatrolling = false;            // Flag to indicate patrolling

    void Awake()
    {
        ourRigidbody = GetComponent<Rigidbody>();
        ourRigidbody.useGravity = true;
        ourRigidbody.isKinematic = false;
    }

    void FixedUpdate()
    {
        if (patrolPoints.Length == 0) return;    // No patrol points set

        float distance = Vector3.Distance(transform.position, patrolPoints[currentPoint]);

        if (distance <= stopDistance) // If close enough to patrol point, move to next
        {
            currentPoint++;
            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0; // Loop back to the first patrol point
            }
        }

        Vector3 direction = (patrolPoints[currentPoint] - transform.position).normalized;

        if (direction.magnitude > 0.1f) // If moving towards patrol point
        {
            // Set the patrolling flag to true
            isPatrolling = true;

            // Move towards the patrol point
            ourRigidbody.AddForce(direction * patrolSpeed);

            // Clamp velocity to prevent overshooting
            if (ourRigidbody.velocity.magnitude > maxSpeed)
            {
                ourRigidbody.velocity = ourRigidbody.velocity.normalized * maxSpeed;
            }

            // Smooth rotation towards the patrol direction
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            // If not moving, set isPatrolling to false
            isPatrolling = false;
        }
    }

    // Visualize patrol points in the Editor
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