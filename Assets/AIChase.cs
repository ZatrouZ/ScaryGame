using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIChase : MonoBehaviour
{

    public GameObject player;

    //hidingplace hiding;

    public float speed;

    EnemyPatrol patrol;

    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindWithTag("Player");



        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance <= 5) //If distance is 8 or closer. This object where this script is placed will start following the player. However if further away than distance 9 it will stop following you.
        {
            patrol.enabled = false; //EnemyPatrol Disables
            FindObjectOfType<AIChase>().enabled = true; //Ai chase script enables
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);

        }
        else if (distance >= 9)
        {
            patrol.enabled = true;
            FindObjectOfType<AIChase>().enabled = false;
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
}