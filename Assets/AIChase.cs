using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 3.0f; // Enemy movement speed
    public float chaseDistance = 5.0f; // Distance at which enemy starts chasing
    public float stopChaseDistance = 9.0f; // Distance at which enemy stops chasing

    private EnemyPatrol patrol;
    private float distance;

    void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Find the player if not already assigned
        }

        distance = Vector3.Distance(transform.position, player.transform.position); // Calculate distance between enemy and player

        if (distance <= chaseDistance) // If the player is close enough, chase
        {
            patrol.enabled = false; // Disable patrol script when chasing
            ChasePlayer(); // Call the chase function
        }
        else if (distance >= stopChaseDistance) // If the player is far enough, stop chasing
        {
            patrol.enabled = true; // Enable patrol script when not chasing
        }
    }

    void ChasePlayer()
    {
        // Calculate direction towards the player
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0; // Ignore the Y axis to prevent tilting up/down when rotating

        // Rotate the enemy to face the player smoothly
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f); // Smooth rotation

        // Move the enemy towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
    //if (hiding == true) //If distance is 8 or closer. This object where this script is placed will start following the player. However if further away than distance 9 it will stop following you.
    //{
    // patrol.enabled = false; //EnemyPatrol Disables
    // //FindObjectOfType<AIChase>().enabled = true; //Ai chase script enables
    //transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);

    //}
    //else if (hiding == true)
    //{
    //patrol.enabled = true;
    //FindObjectOfType<AIChase>().enabled = false;
    //}


}