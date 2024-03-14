using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    public const float SPEED_MULTIPLIER = 5.0f;
    public Camera Camera;

    private Rigidbody body;
    private Ray ray;
    private RaycastHit hitPos;
    private Vector3 LookAtPosition = Vector3.zero;
    private bool hit;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(":3");
        }
    }

    private void FixedUpdate()
    {
        float horizInput = Input.GetAxis("Horizontal"); // -1, 0, or 1 depending on user input
        float vertInput = Input.GetAxis("Vertical"); // -1, 0, or 1 depending on user input
        
        if (horizInput != 0 || vertInput != 0)
        {
            //set velocity
            body.velocity = new Vector3(horizInput * SPEED_MULTIPLIER, body.velocity.y, vertInput * SPEED_MULTIPLIER);

            //transform look in direction of velocity
            transform.LookAt(transform.position + Vector3.Normalize(body.velocity));
        }
        else
        {
            ////looking around
            //ray = Camera.ScreenPointToRay(Input.mousePosition);
            //hit = Physics.Raycast(ray, out hitPos, 50.0f, 1 << LayerMask.NameToLayer("Ground"));

            //LookAtPosition = new Vector3(hitPos.point.x, transform.position.y, hitPos.point.z);
            //transform.LookAt(LookAtPosition);
        }
       
        
    }
}
