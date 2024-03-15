using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    public const float SPEED_MULTIPLIER = 5.0f;
    public Camera Camera;
    public TextMeshProUGUI scoreText;

    private Rigidbody body;
    private TimerBehaviourScript timer;
    private int score = 0;
    private bool alive = true;
    
    //private Ray ray;
    //private RaycastHit hitPos;
    //private Vector3 LookAtPosition = Vector3.zero;
    //private bool hit;
    

    // Start is called before the first frame update

    public void Kill()
    {
        alive = false;
    }

    public bool IsAlive()
    {
        return alive;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        timer = GetComponent<TimerBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    private void FixedUpdate()
    {
        if (alive)
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

                //if (Input.GetMouseButtonDown(0))
                //{
                //    Debug.Log("Trans Rights!");
                //}
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            int value = collision.gameObject.GetComponent<BasicNPCBehaviourScript>().GetValue();
            score += value;
            timer.AppendTime(value);
            value = -1;
            Destroy(collision.gameObject);
        }
    }
}
