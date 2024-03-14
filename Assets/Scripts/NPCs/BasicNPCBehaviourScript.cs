using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPCBehaviourScript : MonoBehaviour
{
    public Rigidbody player;
    private Rigidbody body;
    private bool playerSeen = false;
    private float speedMultiplier;
    private bool caught = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        speedMultiplier = PlayerBehaviourScript.SPEED_MULTIPLIER * 0.75f; //grabs the 
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 runDirection = Vector3.Normalize(body.position - player.position);
        if (playerSeen && !caught)
        {
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerSeen = false;
        }
    }
}
