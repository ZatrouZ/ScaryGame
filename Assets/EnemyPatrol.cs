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

    void Awake()
    {
        // Get the Rigidbody component attached to this object
        ourRigidbody = GetComponent<Rigidbody>();

        // Ensure the Rigidbody is set to use gravity and is not kinematic
        ourRigidbody.useGravity = true;
        ourRigidbody.isKinematic = false;
    }

    void FixedUpdate()
    {
        // Check if there are patrol points defined
        if (patrolPoints.Length == 0)
            return;

        // Calculate the distance between the enemy and the current patrol point
        float distance = Vector3.Distance(transform.position, patrolPoints[currentPoint]);

        // If the enemy is close enough to the current patrol point
        if (distance <= stopDistance)
        {
            // Move to the next patrol point
            currentPoint++;

            // If we've reached the end of the patrol points, loop back to the start
            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0;
            }
        }

        // Calculate the direction to the current patrol point
        Vector3 direction = (patrolPoints[currentPoint] - transform.position).normalized;

        // Apply force in the direction of the current patrol point
        ourRigidbody.AddForce(direction * forceStrength);

        // Clamp the velocity to prevent overshooting and maintain a maximum speed
        if (ourRigidbody.velocity.magnitude > maxSpeed)
        {
            ourRigidbody.velocity = ourRigidbody.velocity.normalized * maxSpeed;
        }

        // Smooth rotation to face the direction of movement
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    // Visualize patrol points and the paths between them in the Unity Editor
    void OnDrawGizmos()
    {
        if (patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(patrolPoints[i], 0.5f); // Draw a small sphere at each patrol point

                // Draw a line connecting patrol points to visualize the path
                if (i > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(patrolPoints[i - 1], patrolPoints[i]);
                }
            }

            // Optional: Close the loop by connecting the last point to the first
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1], patrolPoints[0]);
        }
    }
}