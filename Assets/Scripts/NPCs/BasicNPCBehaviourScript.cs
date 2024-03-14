using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPCBehaviourScript : MonoBehaviour
{
    public Rigidbody player;

    private Rigidbody body;
    private Vector3 runDirection;
    private float speedMultiplier;
    private float fearRunningTime = 5.0f;
    private bool playerSeen = false;
    private bool playerInRange = false;
    private bool caught = false;


    IEnumerator RunningInFear()
    {
        yield return new WaitForSeconds(fearRunningTime);
        playerSeen = false;
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
            transform.LookAt(player.position);
        }

        if (playerSeen && !caught)
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
