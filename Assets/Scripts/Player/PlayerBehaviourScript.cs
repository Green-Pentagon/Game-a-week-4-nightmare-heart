using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    private Rigidbody body;
    private float velocityMultiplier = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float horizInput = Input.GetAxis("Horizontal"); // -1, 0, or 1 depending on user input
        float vertInput = Input.GetAxis("Vertical"); // -1, 0, or 1 depending on user input
        
        if (horizInput != vertInput)
        {
            body.velocity = new Vector3(horizInput * velocityMultiplier, body.velocity.y, vertInput * velocityMultiplier);
        }
       
        
    }
}
