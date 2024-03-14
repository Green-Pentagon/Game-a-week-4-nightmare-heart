using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPCBehaviourScript : MonoBehaviour
{
    public Rigidbody player;
    public float speedMultiplier = 0.75f;
    

    private Rigidbody body;
    private Vector3 runDirection;        //direction which the NPC runs away in.
    private float ActorSpeed;            //how fast the NPC runs, relative to speed of player multiplied by the speedMultiplier
    private float fearRunningTime = 5.0f;//how long the NPC runs away after they spotted the player
    private bool playerSeen = false;     //has the NPC seen the player?
    private bool playerInRange = false;  //is the player inside of the visibility collider?
    private bool caught = false;         //was the NPC caught by the player?
    private int pointsWorth;             //Points that the NPC is worth when caught, calculated in the Start.

    IEnumerator RunningInFear()
    {
        yield return new WaitForSeconds(fearRunningTime);
        
        if (!playerInRange)//inRange check prevents potential bug where NPC stops if player re-enters collision range before fear running is complete.
        {
            playerSeen = false;
        }
        
    }

    public int GetValue()
    {
        return pointsWorth;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        ActorSpeed = PlayerBehaviourScript.SPEED_MULTIPLIER * speedMultiplier; //grabs the speed of player and multiplies it by a factor.

        //calculate how much this NPC is worth in points.
        // x = 10 + 10 * s
        // s = speed factor relative to the player
        // if s == 1.0f, the NPC will be worth 20 points.
        pointsWorth = 10 + Mathf.RoundToInt(10*speedMultiplier); 
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
            body.velocity = runDirection * ActorSpeed;
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
