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
    private CameraBehaviourScript CameraBehaviour;
    private int score = 0;
    private bool alive = true;

    private Ray ray;
    private RaycastHit hitPos;
    private Vector3 LookAtPosition = Vector3.zero;
    private bool hit;
    private bool offsetCamera = false;


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
        CameraBehaviour = Camera.GetComponent<CameraBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();

        if (Input.GetMouseButtonDown(0) && alive)
        {
            //Debug.Log("Trans Rights!");
            offsetCamera = true;
            CameraBehaviour.SnapToPlayer(!CameraBehaviour.IsSnappingToPlayer());
            CameraBehaviour.SetLocation(hitPos.point);
        }
    }

    private void FixedUpdate()
    {
        if (alive)
        {
            float horizInput = Input.GetAxis("Horizontal"); // -1, 0, or 1 depending on user input
            float vertInput = Input.GetAxis("Vertical"); // -1, 0, or 1 depending on user input

            if (horizInput != 0 || vertInput != 0)
            {
                CameraBehaviour.SnapToPlayer(true);

                //set velocity
                body.velocity = new Vector3(horizInput * SPEED_MULTIPLIER, body.velocity.y, vertInput * SPEED_MULTIPLIER);

                //transform look in direction of velocity
                transform.LookAt(transform.position + Vector3.Normalize(body.velocity));
            }
            else
            {
                //looking around
                ray = Camera.ScreenPointToRay(Input.mousePosition);
                hit = Physics.Raycast(ray, out hitPos, 50.0f, 1 << LayerMask.NameToLayer("Ground"));
                if (!hit)
                {
                    hitPos.point = transform.position;
                }

                LookAtPosition = new Vector3(hitPos.point.x, transform.position.y, hitPos.point.z);
                transform.LookAt(LookAtPosition);

                ////accidentally madea  buggy free-roaming camera, not useful for this project but keeping it in if it may come in useful for any future re-working.
                //if (!CameraBehaviour.IsSnappingToPlayer())
                //{
                //    CameraBehaviour.SetLocation(hitPos.point);
                //}
            }
        }
        else
        {
            CameraBehaviour.SnapToPlayer(true);
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
