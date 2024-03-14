using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPCBehaviourScript : MonoBehaviour
{
    public Rigidbody player;

    private Rigidbody body;
    private Vector3 runDirection;        //direction which the NPC runs away in.
    private float speedMultiplier;       //how fast the NPC runs, relative to speed of player.
    private float fearRunningTime = 5.0f;//how long the NPC runs away after they spotted the player
    private bool playerSeen = false;     //has the NPC seen the player?
    private bool playerInRange = false;  //is the player inside of the visibility collider?
    private bool caught = false;         //was the NPC caught by the player?


    IEnumerator RunningInFear()
    {
        yield return new WaitForSeconds(fearRunningTime);
        
        if (!playerInRange)//inRange check prevents potential bug where NPC stops if player re-enters collision range before fear running is complete.
        {
            playerSeen = false;
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        speedMultiplier = PlayerBehaviourScript.SPEED_MULTIPLIER * 0.75f; //grabs the speed of player and multiplies it by a factor.
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (playerInRange && !caught)
        {
            //update running direction & rotation of transform
            runDirection = Vector3.Normalize(body.position - player.position);
            transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z)); //look at player, ignoring Z position.
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f)); //face away from player
        }

        if ((playerSeen) && !caught)
        {
            
            //run in the last known run direction
            body.velocity = runDirection * speedMultiplier;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            caught = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerSeen = true;
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
            StartCoroutine(RunningInFear());
        }
    }
}
