using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{

    public float forceStrength;     //speed
    public float stopDistance;      // how close before moving onto the next point
    public Vector2[] patrolPoints;  // List of patrol points the object will go between


    private int currentPoint = 0;       // Index of the current point we're moving towards
    private Rigidbody2D ourRigidbody;   // The rigidbody attached to this object
    private float distance;


    void Awake()
    {
        // The rigidbody for movement
        ourRigidbody = GetComponent<Rigidbody2D>();
    }



    // Update is called once per frame

    void FixedUpdate()
    {


        // distance from the target
        float distance = (patrolPoints[currentPoint] - (Vector2)transform.position).magnitude;


        if (distance <= stopDistance)
        {
            // Update to the next target
            currentPoint = currentPoint + 1;

            // If we've gone past the end of our list...  
            if (currentPoint >= patrolPoints.Length)
            {
                //...we start over from the start again (loop)
                currentPoint = 0;
            }
        }


        Vector2 direction = (patrolPoints[currentPoint] - (Vector2)transform.position).normalized;

        // Move in the correct direction with the set force strength
        ourRigidbody.AddForce(direction * forceStrength);
    }
}